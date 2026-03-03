using System.Diagnostics;
using System.Net.WebSockets;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Network;

class Connection
{
    private readonly WebSocket _socket;

    private readonly object _sync = new object();
    
    private SendBuffer _sendBufferA;
    private SendBuffer _sendBufferB;

    private bool _flushSignalled;
    private TaskCompletionSource? _sendSignal;

    private bool _disposed;
    
    private bool _disconnected;
    private readonly CancellationTokenSource _cancellationSource;
    private CancellationToken _cancellation;

    private string? _closeReason;

    private readonly Session?[] _sessions;
    private byte _sessionPosition;
    
    public delegate void PacketWriterAction(ref PacketWriter writer);
    public delegate void DisconnectedHandler(string reason);
    public event DisconnectedHandler? Disconnected;
    
    public Connection(WebSocket socket, int sendBufferSize)
    {
        _socket = socket;
        
        _sessions = new Session[256];
        
        _cancellationSource = new CancellationTokenSource();
        _cancellation = _cancellationSource.Token;
        
        _sendBufferA = new SendBuffer(sendBufferSize);
        _sendBufferB = new SendBuffer(sendBufferSize);

        ThreadPool.QueueUserWorkItem(async delegate { await SendWorker(); });
    }
    
    public async Task<PacketReaderCopy> SendSessionRequestAndGetReply(PacketWriterAction action)
    {
        Session session = StartSessionRequest(action, false);
        return await session.GetReplyOrThrow().ConfigureAwait(false);
    }

    public async Task<PacketReaderLarge> SendSessionRequestAndGetReplyLarge(PacketWriterAction action)
    {
        Session session = StartSessionRequest(action, true);
        return await session.GetLargeReplyOrThrow().ConfigureAwait(false);
    }
    
    public bool Disposed => _disposed;

    public string? CloseReason => _closeReason;

    private Session StartSessionRequest(PacketWriterAction action, bool largeReply)
    {
        if (_disconnected)
            throw new ConnectionTerminatedGameException(_closeReason);

        Session session = new Session(largeReply);

        for (int @try = 0; @try < _sessions.Length; @try++)
        {
            _sessionPosition++;

            byte sessionId = _sessionPosition;

            if (sessionId > 0 && Interlocked.CompareExchange(ref _sessions[sessionId], session, null) is null)
            {
                session.Setup(sessionId);
                break;
            }
        }

        if (session.Id == 0)
            throw new SessionsExhaustedException();

        lock (_sync)
        {
            PacketWriter writer = _sendBufferA.Write();
            action(ref writer);
            writer.Session = session.Id;
            writer.Dispose();

            if (!_flushSignalled)
                if (_sendSignal is not null)
                {
                    _sendSignal.SetResult();
                    _sendSignal = null;
                }
                else
                    _flushSignalled = true;
        }

        return session;
    }
    
    public void Send(PacketWriterAction action)
    {
        lock (_sync)
        {
            if (_disconnected)
                return;

            PacketWriter writer = _sendBufferA.Write();
            action(ref writer);
            writer.Dispose();
        }
    }

    public void Send(SendBuffer buffer)
    {
        lock (_sync)
        {
            if (_disconnected)
                return;

            if (!_sendBufferA.Send(buffer))
                Close("Send buffer overflow: Check your internet connection.");
        }
    }

    public void Flush()
    {
        lock (_sync)
        {
            if (_disconnected || _flushSignalled || !_sendBufferA.HasData)
                return;

            if (_sendSignal is not null)
            {
                _sendSignal.SetResult();
                
                _sendSignal = null;

                return;
            }
            
            _flushSignalled = true;
        }
    }

    /// <summary>
    /// Injects the initial session which will result in an exception if something went wrong while logging on or
    /// returns properly when everything is Ok and all meta information 
    /// </summary>
    /// <returns>The session.</returns>
    public Session InitializeInitialSession()
    {
        Debug.Assert(_sessions[1] is null, "Can't initialize the initial session when a session is already there on session slot 1.");
        
        Session session = new Session(false);
        
        _sessions[1] = session;

        return session;
    }
    
    /// <summary>
    /// Answer a session with a packet.
    /// </summary>
    /// <param name="reader">The reader at the current packet position.</param>
    /// <returns>true, if everything is Ok, false if the corresponding session didn't exist.</returns>
    public bool SessionReply(PacketReader reader)
    {

        Session? session = _sessions[reader.Session];
        
        if (session is null)
            return false;

        if (reader.Command == 0xFF)
        {
            GameException? exception;
            
            if (GameException.TryParseGameException(reader, out exception))
                session.SetResult(exception);
            else
            {
                session.SetResult(new ConnectionTerminatedGameException("The received exception couldn't be parsed."));
                Close("Received ambiguous exception data.");
                return false;
            }
        }
        else
        {
            if (session.ExpectsLargeReply)
                session.SetLargeResult(reader);
            else if (reader.Size <= PacketReaderCopy.InlineCapacity)
                session.SetResult(reader);
            else
            {
                session.SetResult(new ConnectionTerminatedGameException("Received oversized session reply in small reply mode."));
                Close("Received oversized session reply in small reply mode.");
                return false;
            }
        }
        
        _sessions[reader.Session] = null;

        return true;
    }
    
    /// <summary>
    /// This is called from the outside to initiate receiving of packets.
    /// </summary>
    /// <param name="packets">The packets we receive into.</param>
    /// <returns>true, if the connection was open and everything could be read.</returns>
    public async Task<bool> TryRead(PacketReader packets)
    {
        if (_disconnected)
            return false;

        ValueWebSocketReceiveResult result;
        
        try
        {
            result = await _socket.ReceiveAsync(packets.FullMemory, _cancellation).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Close($"Error while receiving data: {e.Message}");
            return false;
        }

        switch (result.MessageType)
        {
            case WebSocketMessageType.Binary:
                packets.Reset(result.Count);
                return true;
            case WebSocketMessageType.Close:
                lock (_sync)
                    _disconnected = true;
                
                if (_socket.State == WebSocketState.CloseReceived)
                    try
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Goodbye.",
                            CancellationToken.None).ConfigureAwait(false);
                    }
                    catch
                    { }

                if (_socket.CloseStatus.HasValue)
                    if (_socket.CloseStatusDescription is null)
                        Close($"Flattiverse server disconnected with reason: {_socket.CloseStatus.Value}.");
                    else
                        Close(
                            $"Flattiverse server disconnected with reason: {_socket.CloseStatus.Value} and message \"{_socket.CloseStatusDescription}\".");
                else
                    Close("Flattiverse server disconnected without giving any reason.");
                return false;
            default: // Also WebSocketMessageType.Text
                lock (_sync)
                    _disconnected = true;
                
                if (_socket.State == WebSocketState.Open)
                    try
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Flattiverse protocol doesn't implement text messages.",
                            CancellationToken.None).ConfigureAwait(false);
                    }
                    catch
                    { }
                
                Close("The remote server violated the flattiverse protocol by sending a text message.");
                
                return false;
        }
    }

    public void Close(string reason)
    {
        lock (_sync)
            if (_disposed)
                return;
            else
            {
                _disposed = true;
                _disconnected = true;
                _closeReason ??= reason;

                if (_sendSignal is not null)
                {
                    _sendSignal.TrySetResult();
                    _sendSignal = null;
                }

                for (int sessionId = 1; sessionId < _sessions.Length; sessionId++)
                    if (_sessions[sessionId] is Session session)
                    {
                        _sessions[sessionId] = null;
                        session.SetResult(new ConnectionTerminatedGameException(_closeReason));
                    }

                Disconnected?.Invoke(reason);
            }

        _cancellationSource.Cancel();

        Thread.Sleep(1);

        _cancellationSource.Dispose();
        _socket.Dispose();
    }
    
    private async Task SendWorker()
    {
        TaskCompletionSource? tcs = null;
        
        while (!_disconnected)
        {
            lock (_sync)
            {
                if (_flushSignalled)
                    _flushSignalled = false;
                else
                {
                    tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                    _sendSignal = tcs;
                }
            }
            
            if (tcs is not null)
            {
                await tcs.Task.ConfigureAwait(false);
                tcs = null;

                if (_disconnected)
                    return;
            }

            lock (_sync)
                (_sendBufferA, _sendBufferB) = (_sendBufferB, _sendBufferA);

            if (!_sendBufferB.HasData)
                continue;
            
            try
            {
                await _socket.SendAsync(_sendBufferB.Buffer, WebSocketMessageType.Binary, true, _cancellation).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Close($"The server closed the connection: {exception}");
                return;
            }
            
            _sendBufferB.Reset();
        }
    }
}
