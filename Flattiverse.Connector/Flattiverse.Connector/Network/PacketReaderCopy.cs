using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Flattiverse.Connector.Network;

[StructLayout(LayoutKind.Explicit, Size = 32, Pack = 1)]
unsafe struct PacketReaderCopy
{
    [FieldOffset(0)]
    public readonly byte Command;

    [FieldOffset(1)]
    public readonly byte Session;

    [FieldOffset(2)]
    public readonly byte Length;

    [FieldOffset(3)]
    private byte _position;

    [FieldOffset(4)]
    private fixed byte _data[28];

    public PacketReaderCopy(byte[] data, int basePosition)
    {
        ushort size = Unsafe.As<byte, ushort>(ref data[basePosition + 2]);
        
        Debug.Assert(size <= 28, "Packet content is too big.");
        
        Command = data[basePosition];
        Session = data[basePosition + 1];
        
        Length = (byte)size;

        _position = 0;
        
        Unsafe.CopyBlock(ref _data[0], ref data[basePosition + 4], size);
    }
    
    public bool Read(out byte data)
    {
        if (_position >= Length)
        {
            data = default;
            return false;
        }

        data = _data[_position];
        _position++;
        return true;
    }

    public bool Read(out ushort data)
    {
        if (_position + 1 >= Length)
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, ushort>(ref _data[_position]);
        _position += 2;
        return true;
    }

    public bool Read(out int data)
    {
        if (_position + 3 >= Length)
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, int>(ref _data[_position]);
        _position += 4;
        return true;
    }

    public bool Read([NotNullWhen(true)] out string? data)
    {
        int size;

        if (_position >= Length)
        {
            data = default;
            return false;
        }
        
        size = _data[_position];

        if (_position + size >= Length)
        {
            data = default;
            return false;
        }

        _position++;

        if (size == 0)
            data = string.Empty;
        else
            fixed(byte* pData = _data)
                data = Encoding.UTF8.GetString(new Span<byte>(pData + _position, size));

        _position += (byte)size;
        
        return true;
    }

    public bool Read(byte[] data)
    {
        if (_position + data.Length > Length)
            return false;

        if (data.Length == 0)
            return true;
        
        Unsafe.CopyBlock(ref data[0], ref _data[_position], (uint)data.Length);

        _position += (byte)data.Length;
        return true;
    }
}