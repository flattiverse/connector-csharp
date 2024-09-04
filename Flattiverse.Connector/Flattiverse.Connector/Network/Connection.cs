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
    
    public async Task<PacketReaderCopy> SendSessionRequestAndGetReply(PacketWriter packet)
    {
        if (_disconnected)
            throw new ConnectionTerminatedGameException(_closeReason);

        Session session = new Session();
        
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

        packet.Session = session.Id;

        Debug.WriteLine($" => SENDING REQUEST FOR SESSION #{packet.Session:X02}: COMMAND 0x{packet.Command:X02}.");
        
        lock (_sync)
        {
            if (!_sendBufferA.Send(packet))
            {
                Close("Send buffer overflow: Check your internet connection.");
                throw new ConnectionTerminatedGameException(_closeReason);
            }

            if (!_flushSignalled)
                if (_sendSignal is not null)
                {
                    _sendSignal.SetResult();
                
                    _sendSignal = null;
                }
                else
                    _flushSignalled = true;
        }

        return await session.GetReplyOrThrow().ConfigureAwait(false);
    }
    
    public bool Disposed => _disposed;

    public string? CloseReason => _closeReason;
    
    public void Send(PacketWriter packet)
    {
        lock (_sync)
        {
            if (_disconnected)
                return;

            if (!_sendBufferA.Send(packet))
                Close("Send buffer overflow: Check your internet connection.");
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
        
        Session session = new Session();
        
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
            session.SetResult(reader);
        
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
                Disconnected?.Invoke(reason);
                
                _disposed = true;
                _disconnected = true;
                _closeReason ??= reason;
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