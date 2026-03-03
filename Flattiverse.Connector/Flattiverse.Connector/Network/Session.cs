using System.Diagnostics;

namespace Flattiverse.Connector.Network;

class Session
{
    private readonly TaskCompletionSource _waiter;
    private readonly bool _largeReply;

    private PacketReaderCopy _reader;
    private PacketReaderLarge _largeReader;
    private GameException? _exception;
    
    private byte _id;

    public Session(bool largeReply)
    {
        _waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _largeReply = largeReply;
    }
    
    public void Setup(byte id)
    {
        Debug.Assert(_id == 0, "Can't initialize id twice.");
        
        _id = id;
    }

    public void SetResult(PacketReader reader)
    {
        Debug.Assert(!_largeReply, "Expected large session reply.");
        _reader = reader.Copy;
        _waiter.TrySetResult();
    }

    public void SetLargeResult(PacketReader reader)
    {
        Debug.Assert(_largeReply, "Expected small session reply.");
        _largeReader = reader.LargeCopy;
        _waiter.TrySetResult();
    }

    public void SetResult(GameException exception)
    {
        _exception = exception;
        _waiter.TrySetResult();
    }
    
    public async Task<PacketReaderCopy> GetReplyOrThrow()
    {
        Debug.Assert(!_largeReply, "Expected large session reply.");
        await _waiter.Task;

        if (_exception is not null)
            throw _exception;
        
        return _reader;
    }

    public async Task<PacketReaderLarge> GetLargeReplyOrThrow()
    {
        Debug.Assert(_largeReply, "Expected small session reply.");
        await _waiter.Task;

        if (_exception is not null)
            throw _exception;

        return _largeReader;
    }
    
    public byte Id => _id;
    public bool ExpectsLargeReply => _largeReply;
}
