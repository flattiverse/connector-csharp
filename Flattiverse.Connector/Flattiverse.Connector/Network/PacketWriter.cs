using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

struct PacketWriter : IDisposable
{
    private readonly SendBuffer _buffer;
    private int _size;

    public PacketWriter(SendBuffer buffer)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = 0;
        _buffer.Data[_buffer.Position + 1] = 0;
    }

    public PacketWriter(SendBuffer buffer, byte command, byte session)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = command;
        _buffer.Data[_buffer.Position + 1] = session;
    }

    public PacketWriter(SendBuffer buffer, byte command)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = command;
        _buffer.Data[_buffer.Position + 1] = 0;
    }

    public byte Command
    {
        get => _buffer.Data[_buffer.Position];
        set => _buffer.Data[_buffer.Position] = value;
    }

    public byte Session
    {
        get => _buffer.Data[_buffer.Position + 1];
        set => _buffer.Data[_buffer.Position + 1] = value;
    }

    public override string ToString()
    {
        return $"SEND: cmd=0x{Command:X02} sess=0x{Session:X02}.";
    }

    public void Write(byte data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 1 <= _buffer.Data.Length, "Packet too long.");

        _buffer.Data[_buffer.Position + 4 + _size++] = data;
    }

    public void Write(ushort data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 2 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, ushort>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 2;
    }

    public void Write(int data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, int>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    public void Write(uint data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, uint>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    public void Write(long data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 8 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, long>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 8;
    }

    public void Write(float data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, float>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    public void Write(string? data)
    {
        if (string.IsNullOrEmpty(data))
        {
            Debug.Assert(_buffer.Position + 4 + _size + 1 <= _buffer.Data.Length, "Packet too long.");

            _buffer.Data[_buffer.Position + 4 + _size++] = 0x00;

            return;
        }

        int size = Encoding.UTF8.GetByteCount(data);

        if (size < 255)
        {
            Debug.Assert(_buffer.Position + 4 + _size + 1 + size <= _buffer.Data.Length, "Packet too long.");

            _buffer.Data[_buffer.Position + 4 + _size++] = (byte)size;
            Encoding.UTF8.GetBytes(data, 0, data.Length, _buffer.Data, _buffer.Position + 4 + _size);
            _size += size;
        }
        else
        {
            Debug.Assert(_buffer.Position + 4 + _size + 3 + size <= _buffer.Data.Length, "Packet too long.");

            _buffer.Data[_buffer.Position + 4 + _size++] = 255;
            Unsafe.As<byte, ushort>(ref _buffer.Data[_buffer.Position + 4 + _size]) = (ushort)size;
            _size += 2;
            Encoding.UTF8.GetBytes(data, 0, data.Length, _buffer.Data, _buffer.Position + 4 + _size);
            _size += size;
        }
    }

    public void Write(byte[] data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + data.Length <= _buffer.Data.Length, "Packet too long.");

        Buffer.BlockCopy(data, 0, _buffer.Data, _buffer.Position + 4 + _size, data.Length);

        _size += data.Length;
    }

    public void Dispose()
    {
        Unsafe.As<byte, ushort>(ref _buffer.Data[_buffer.Position + 2]) = (ushort)_size;
        _buffer.Position += 4 + _size;
    }
}
