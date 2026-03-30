using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Serializer for one Flattiverse protocol packet inside a <see cref="SendBuffer" />.
/// </summary>
struct PacketWriter : IDisposable
{
    private readonly SendBuffer _buffer;
    private int _size;

    /// <summary>
    /// Opens a new packet at the current end of the target send buffer.
    /// Command and session can be assigned afterwards.
    /// </summary>
    /// <param name="buffer">Target send buffer.</param>
    public PacketWriter(SendBuffer buffer)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = 0;
        _buffer.Data[_buffer.Position + 1] = 0;
    }

    /// <summary>
    /// Opens a new packet at the current end of the target send buffer with explicit command and session bytes.
    /// </summary>
    /// <param name="buffer">Target send buffer.</param>
    /// <param name="command">Initial command byte.</param>
    /// <param name="session">Initial session byte.</param>
    public PacketWriter(SendBuffer buffer, byte command, byte session)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = command;
        _buffer.Data[_buffer.Position + 1] = session;
    }

    /// <summary>
    /// Opens a new packet at the current end of the target send buffer with an explicit command byte and session <c>0</c>.
    /// </summary>
    /// <param name="buffer">Target send buffer.</param>
    /// <param name="command">Initial command byte.</param>
    public PacketWriter(SendBuffer buffer, byte command)
    {
        _buffer = buffer;
        _size = 0;
        _buffer.Data[_buffer.Position] = command;
        _buffer.Data[_buffer.Position + 1] = 0;
    }

    /// <summary>
    /// Command byte of the currently open packet header.
    /// </summary>
    public byte Command
    {
        get => _buffer.Data[_buffer.Position];
        set => _buffer.Data[_buffer.Position] = value;
    }

    /// <summary>
    /// Session byte of the currently open packet header.
    /// </summary>
    public byte Session
    {
        get => _buffer.Data[_buffer.Position + 1];
        set => _buffer.Data[_buffer.Position + 1] = value;
    }

    /// <summary>
    /// Returns a compact diagnostic representation of the open packet header.
    /// </summary>
    public override string ToString()
    {
        return $"SEND: cmd=0x{Command:X02} sess=0x{Session:X02}.";
    }

    /// <summary>
    /// Writes one <see cref="byte" /> to the packet payload.
    /// </summary>
    public void Write(byte data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 1 <= _buffer.Data.Length, "Packet too long.");

        _buffer.Data[_buffer.Position + 4 + _size++] = data;
    }

    /// <summary>
    /// Writes one little-endian <see cref="ushort" /> to the packet payload.
    /// </summary>
    public void Write(ushort data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 2 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, ushort>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 2;
    }

    /// <summary>
    /// Writes one little-endian <see cref="int" /> to the packet payload.
    /// </summary>
    public void Write(int data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, int>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    /// <summary>
    /// Writes one little-endian <see cref="uint" /> to the packet payload.
    /// </summary>
    public void Write(uint data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, uint>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    /// <summary>
    /// Writes one little-endian <see cref="long" /> to the packet payload.
    /// </summary>
    public void Write(long data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 8 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, long>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 8;
    }

    /// <summary>
    /// Writes one IEEE-754 single-precision float to the packet payload.
    /// </summary>
    public void Write(float data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + 4 <= _buffer.Data.Length, "Packet too long.");

        Unsafe.As<byte, float>(ref _buffer.Data[_buffer.Position + 4 + _size]) = data;

        _size += 4;
    }

    /// <summary>
    /// Writes one protocol string to the packet payload.
    /// Empty and <see langword="null" /> strings are encoded identically as length <c>0</c>.
    /// </summary>
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

    /// <summary>
    /// Writes raw bytes to the packet payload.
    /// </summary>
    public void Write(byte[] data)
    {
        Debug.Assert(_buffer.Position + 4 + _size + data.Length <= _buffer.Data.Length, "Packet too long.");

        Buffer.BlockCopy(data, 0, _buffer.Data, _buffer.Position + 4 + _size, data.Length);

        _size += data.Length;
    }

    /// <summary>
    /// Finalizes the packet by writing its payload length into the header and advancing the target send buffer.
    /// </summary>
    public void Dispose()
    {
        Unsafe.As<byte, ushort>(ref _buffer.Data[_buffer.Position + 2]) = (ushort)_size;
        _buffer.Position += 4 + _size;
    }
}
