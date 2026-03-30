using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Reader for one validated websocket message that may contain one or more Flattiverse packets.
/// </summary>
class PacketReader
{
    /// <summary>
    /// The byte[] which is the basis of all data.
    /// </summary>
    private byte[] _data;

    private int _basePosition;
    private int _position;
    private int _dataSize;

    /// <summary>
    /// Creates one packet-stream reader over a reusable raw receive buffer.
    /// </summary>
    /// <param name="data">Raw byte buffer used for websocket receives.</param>
    public PacketReader(byte[] data)
    {
        _data = data;
    }

    /// <summary>
    /// Creates a compact copied reply reader for the current packet.
    /// </summary>
    public PacketReaderCopy Copy => new PacketReaderCopy(_data, _basePosition);

    /// <summary>
    /// Creates a heap-backed copied reply reader for the current packet.
    /// </summary>
    public PacketReaderLarge LargeCopy => new PacketReaderLarge(_data, _basePosition);
    
    /// <summary>
    /// Full underlying receive buffer as writable memory for websocket reads.
    /// </summary>
    public Memory<byte> FullMemory => _data.AsMemory(); 

    /// <summary>
    /// Validates that the specified byte range contains a well-framed Flattiverse packet stream.
    /// </summary>
    /// <param name="data">Raw message bytes.</param>
    /// <param name="size">Number of valid bytes inside <paramref name="data" />.</param>
    /// <param name="malformedOffset">Offset of the malformed packet header or trailing bytes.</param>
    /// <param name="malformedPacketSize">Declared payload size of the malformed packet when available.</param>
    /// <returns><see langword="true" /> if the packet stream framing is valid; otherwise <see langword="false" />.</returns>
    public static bool TryValidatePacketStream(byte[] data, int size, out int malformedOffset, out ushort malformedPacketSize)
    {
        Debug.Assert(data is not null, "No packet data provided.");
        Debug.Assert(size >= 0 && size <= data.Length, "Invalid packet size specified.");

        int position = 0;

        while (position + 4 <= size)
        {
            ushort packetSize = Unsafe.As<byte, ushort>(ref data[position + 2]);

            if (position + 4 + packetSize > size)
            {
                malformedOffset = position;
                malformedPacketSize = packetSize;
                return false;
            }

            position += 4 + packetSize;
        }

        malformedOffset = position;
        malformedPacketSize = 0;
        return position == size;
    }
    
    /// <summary>
    /// Resets the reader to the first packet in the current receive buffer.
    /// </summary>
    /// <param name="size">Number of valid bytes currently stored in the receive buffer.</param>
    /// <returns><see langword="true" /> if the first packet header and payload fit into the buffer; otherwise <see langword="false" />.</returns>
    public bool Reset(int size)
    {
        Debug.Assert(size <= _data.Length, "Wrong size specified, size > byte[] size.");

        _basePosition = 0;
        _position = 0;
        _dataSize = size;

        return _basePosition + 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]) <= _dataSize;
    }

    /// <summary>
    /// Advances to the next packet within the same validated websocket message.
    /// </summary>
    /// <returns><see langword="true" /> if another complete packet exists; otherwise <see langword="false" />.</returns>
    public bool Next()
    {
        _basePosition += 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);
        _position = 0;

        return _basePosition + 4 <= _dataSize && _basePosition + 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]) <= _dataSize;
    }

    /// <summary>
    /// Command byte of the current packet.
    /// </summary>
    public byte Command => _data[_basePosition];

    /// <summary>
    /// Session byte of the current packet.
    /// </summary>
    public byte Session => _data[_basePosition + 1];

    /// <summary>
    /// Declared payload size of the current packet in bytes.
    /// </summary>
    public ushort Size => Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);

    /// <summary>
    /// Reads one <see cref="byte" /> from the current packet payload.
    /// </summary>
    public bool Read(out byte data)
    {
        if (_position >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = _data[_basePosition + 4 + _position];
        _position++;
        return true;
    }

    /// <summary>
    /// Reads one little-endian <see cref="ushort" /> from the current packet payload.
    /// </summary>
    public bool Read(out ushort data)
    {
        if (_position + 1 >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, ushort>(ref _data[_basePosition + 4 + _position]);
        _position += 2;
        return true;
    }

    /// <summary>
    /// Reads one little-endian <see cref="int" /> from the current packet payload.
    /// </summary>
    public bool Read(out int data)
    {
        if (_position + 3 >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, int>(ref _data[_basePosition + 4 + _position]);
        _position += 4;
        return true;
    }
    
    /// <summary>
    /// Reads one little-endian <see cref="uint" /> from the current packet payload.
    /// </summary>
    public bool Read(out uint data)
    {
        if (_position + 3 >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, uint>(ref _data[_basePosition + 4 + _position]);
        _position += 4;
        return true;
    }

    /// <summary>
    /// Reads one little-endian <see cref="long" /> from the current packet payload.
    /// </summary>
    public bool Read(out long data)
    {
        if (_position + 7 >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, long>(ref _data[_basePosition + 4 + _position]);
        _position += 8;
        return true;
    }

    /// <summary>
    /// Reads one IEEE-754 single-precision float from the current packet payload.
    /// </summary>
    public bool Read(out float data)
    {
        if (_position + 3 >= Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, float>(ref _data[_basePosition + 4 + _position]);
        _position += 4;
        return true;
    }

    /// <summary>
    /// Reads one protocol string from the current packet payload.
    /// Empty and <see langword="null" /> strings are encoded identically on the wire.
    /// </summary>
    public bool Read(out string data)
    {
        int size;

        int availableSize = Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);

        if (_position >= availableSize)
        {
            data = string.Empty;
            return false;
        }

        if (_data[_basePosition + 4 + _position] == 0xFF)
        {
            size = Unsafe.As<byte, ushort>(ref _data[_basePosition + 4 + _position + 1]);

            if (_position + 2 + size >= availableSize)
            {
                data = string.Empty;
                return false;
            }

            _position += 3;
        }
        else
        {
            size = _data[_basePosition + 4 + _position];

            if (_position + size >= availableSize)
            {
                data = string.Empty;
                return false;
            }

            _position++;
        }

        if (size == 0)
            data = string.Empty;
        else
            data = Encoding.UTF8.GetString(_data, _basePosition + 4 + _position, size);

        _position += size;
        
        return true;
    }

    /// <summary>
    /// Copies raw bytes from the current packet payload into the supplied target buffer.
    /// </summary>
    public bool Read(byte[] data)
    {
        if (_position + data.Length > Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
            return false;

        Unsafe.CopyBlock(ref data[0], ref _data[_basePosition + 4 + _position], (uint)data.Length);

        _position += data.Length;
        return true;
    }

    /// <summary>
    /// Returns a diagnostic hex dump of the current packet in receive notation.
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append($"RECV: cmd=0x{Command:X02} sess=0x{Session:X02} |");

        int size = Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);

        for (int position = 0; position < size; position++)
            builder.Append($" 0x{_data[_basePosition + 4 + position]:X02}");

        return builder.ToString();
    }

    /// <summary>
    /// Returns a diagnostic hex dump of the current packet in send notation.
    /// </summary>
    public string ClaimWriter()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append($"SEND: cmd=0x{Command:X02} sess=0x{Session:X02} |");

        int size = Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);

        for (int position = 0; position < size; position++)
            builder.Append($" 0x{_data[_basePosition + 4 + position]:X02}");

        return builder.ToString();
    }
}
