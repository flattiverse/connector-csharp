using System.Diagnostics;

namespace Flattiverse.Connector.Network;

class Session
{
    private readonly TaskCompletionSource _waiter;

    private PacketReaderCopy _reader;
    private GameException? _exception;
    
    private byte _id;

    public Session()
    {
        _waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    
    public void Setup(byte id)
    {
        Debug.Assert(_id == 0, "Can't initialize id twice.");
        
        _id = id;
    }

    public void SetResult(PacketReader reader)
    {
        _reader = reader.Copy;
        _waiter.SetResult();
    }

    public void SetResult(GameException exception)
    {
        _exception = exception;
        _waiter.SetResult();
    }
    
    public async Task<PacketReaderCopy> GetReplyOrThrow()
    {
        await _waiter.Task;

        if (_exception is not null)
            throw _exception;
        
        return _reader;
    }
    
    public byte Id => _id;
}