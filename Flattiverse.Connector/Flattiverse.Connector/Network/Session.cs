using System.Diagnostics;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Internal request/reply correlation slot for one outstanding protocol session.
/// </summary>
class Session
{
    private readonly TaskCompletionSource _waiter;
    private readonly bool _largeReply;

    private PacketReaderCopy _reader;
    private PacketReaderLarge _largeReader;
    private GameException? _exception;
    
    private byte _id;

    /// <summary>
    /// Creates one pending request/reply correlation slot.
    /// </summary>
    /// <param name="largeReply">
    /// Whether this session expects a <see cref="PacketReaderLarge" /> instead of a <see cref="PacketReaderCopy" />.
    /// </param>
    public Session(bool largeReply)
    {
        _waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _largeReply = largeReply;
    }
    
    /// <summary>
    /// Assigns the protocol session id after the session has been registered in the connection table.
    /// </summary>
    /// <param name="id">Allocated protocol session id.</param>
    public void Setup(byte id)
    {
        Debug.Assert(_id == 0, "Can't initialize id twice.");
        
        _id = id;
    }

    /// <summary>
    /// Completes the session with a copied small reply packet.
    /// </summary>
    /// <param name="reader">Reader positioned at the reply packet.</param>
    public void SetResult(PacketReader reader)
    {
        Debug.Assert(!_largeReply, "Expected large session reply.");
        _reader = reader.Copy;
        _waiter.TrySetResult();
    }

    /// <summary>
    /// Completes the session with a copied large reply packet.
    /// </summary>
    /// <param name="reader">Reader positioned at the reply packet.</param>
    public void SetLargeResult(PacketReader reader)
    {
        Debug.Assert(_largeReply, "Expected small session reply.");
        _largeReader = reader.LargeCopy;
        _waiter.TrySetResult();
    }

    /// <summary>
    /// Completes the session with an exception instead of a normal reply.
    /// </summary>
    /// <param name="exception">Transported or locally synthesized failure.</param>
    public void SetResult(GameException exception)
    {
        _exception = exception;
        _waiter.TrySetResult();
    }
    
    /// <summary>
    /// Waits for a small reply packet or throws the session's failure.
    /// </summary>
    /// <returns>Copied small reply packet.</returns>
    public async Task<PacketReaderCopy> GetReplyOrThrow()
    {
        Debug.Assert(!_largeReply, "Expected large session reply.");
        await _waiter.Task;

        if (_exception is not null)
            throw _exception;
        
        return _reader;
    }

    /// <summary>
    /// Waits for a large reply packet or throws the session's failure.
    /// </summary>
    /// <returns>Copied large reply packet.</returns>
    public async Task<PacketReaderLarge> GetLargeReplyOrThrow()
    {
        Debug.Assert(_largeReply, "Expected small session reply.");
        await _waiter.Task;

        if (_exception is not null)
            throw _exception;

        return _largeReader;
    }
    
    /// <summary>
    /// Protocol session id.
    /// Remains <c>0</c> until the connection assigns the session slot.
    /// </summary>
    public byte Id => _id;

    /// <summary>
    /// Whether this session expects a large reply packet copy.
    /// </summary>
    public bool ExpectsLargeReply => _largeReply;
}
