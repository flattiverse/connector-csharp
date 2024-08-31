using System.Runtime.CompilerServices;

namespace Flattiverse.Connector.Network;

class SendBuffer
{
    private int _position;
    private byte[] _data;

    public SendBuffer(int size)
    {
        _data = GC.AllocateUninitializedArray<byte>(size, true);
    }

    public bool Send(PacketWriter writer)
    {
        return writer.WriteToByteArray(_data, ref _position);
    }

    public bool Send(SendBuffer buffer)
    {
        if (_position + buffer._position > _data.Length)
            return false;

        Unsafe.CopyBlock(ref _data[_position], ref buffer._data[0], (uint)buffer._position);
        
        _position += buffer._position;

        return true;
    }
    
    public Memory<byte> Buffer => _data.AsMemory(0, _position);

    public bool HasData => _position > 0;
    
    public void Reset()
    {
        _position = 0;
    }
}