using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

class PacketWriter
{
    private int _size;
    private byte[] _data;

    public byte Command;
    public byte Session;

    public PacketWriter(byte[] data)
    {
        _data = data;
    }

    public bool WriteToByteArray(byte[] destination, ref int position)
    {
        if (position + 4 + _size >= destination.Length)
            return false;

        destination[position++] = Command;
        destination[position++] = Session;

        Unsafe.As<byte, ushort>(ref destination[position]) = (ushort)_size;
        Buffer.BlockCopy(_data, 0, destination, position + 2, _size);

        position += 2 + _size;
        
        return true;
    }

    public void Write(byte data)
    {
        Debug.Assert(_size < _data.Length, "Packet too long.");

        _data[_size++] = data;
    }

    public void Write(ushort data)
    {
        Debug.Assert(_size + 1 < _data.Length, "Packet too long.");

        Unsafe.As<byte, ushort>(ref _data[_size]) = data;

        _size += 2;
    }

    public void Write(int data)
    {
        Debug.Assert(_size + 3 < _data.Length, "Packet too long.");

        Unsafe.As<byte, int>(ref _data[_size]) = data;

        _size += 4;
    }
    
    public void Write(float data)
    {
        Debug.Assert(_size + 3 < _data.Length, "Packet too long.");

        Unsafe.As<byte, float>(ref _data[_size]) = data;

        _size += 4;
    }

    public void Write(string? data)
    {
        if (string.IsNullOrEmpty(data))
        {
            Debug.Assert(_size < _data.Length, "Packet too long.");

            _data[_size++] = 0x00;

            return;
        }

        int size = Encoding.UTF8.GetByteCount(data);

        if (size < 255)
        {
            Debug.Assert(_size + size < _data.Length, "Packet too long.");

            _data[_size++] = (byte)size;
            Encoding.UTF8.GetBytes(data, 0, data.Length, _data, _size);
            _size += size;
        }
        else
        {
            Debug.Assert(_size + 2 + size < _data.Length, "Packet too long.");

            _data[_size++] = 255;
            Unsafe.As<byte, ushort>(ref _data[_size]) = (ushort)size;
            _size += 2;
            Encoding.UTF8.GetBytes(data, 0, data.Length, _data, _size);
            _size += size;
        }
    }

    public void Write(byte[] data)
    {
        Debug.Assert(_size + data.Length <= _data.Length, "Packet too long.");

        Buffer.BlockCopy(data, 0, _data, _size, data.Length);

        _size += data.Length;
    }
}