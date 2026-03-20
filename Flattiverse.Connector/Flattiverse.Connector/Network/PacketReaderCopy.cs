using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

unsafe struct PacketReaderCopy
{
    public const int InlineCapacity = 28;

    public readonly byte Command;
    public readonly byte Session;
    public readonly ushort Length;

    private ushort _position;
    private fixed byte _data[InlineCapacity];

    public PacketReaderCopy(byte[] data, int basePosition)
    {
        ushort size = Unsafe.As<byte, ushort>(ref data[basePosition + 2]);

        Debug.Assert(size <= InlineCapacity, "PacketReaderCopy can only be used for small payloads.");

        Command = data[basePosition];
        Session = data[basePosition + 1];
        Length = size;
        _position = 0;

        if (size > 0)
            fixed (byte* source = &data[basePosition + 4])
            fixed (byte* target = _data)
                Unsafe.CopyBlockUnaligned(target, source, size);
    }

    public bool Read(out byte data)
    {
        if (_position >= Length)
        {
            data = default;
            return false;
        }

        fixed (byte* source = _data)
            data = source[_position];

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

        fixed (byte* source = _data)
            data = Unsafe.ReadUnaligned<ushort>(source + _position);

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

        fixed (byte* source = _data)
            data = Unsafe.ReadUnaligned<int>(source + _position);

        _position += 4;
        return true;
    }

    public bool Read(out uint data)
    {
        if (_position + 3 >= Length)
        {
            data = default;
            return false;
        }

        fixed (byte* source = _data)
            data = Unsafe.ReadUnaligned<uint>(source + _position);

        _position += 4;
        return true;
    }

    public bool Read(out long data)
    {
        if (_position + 7 >= Length)
        {
            data = default;
            return false;
        }

        fixed (byte* source = _data)
            data = Unsafe.ReadUnaligned<long>(source + _position);

        _position += 8;
        return true;
    }

    public bool Read(out float data)
    {
        if (_position + 3 >= Length)
        {
            data = default;
            return false;
        }

        fixed (byte* source = _data)
            data = Unsafe.ReadUnaligned<float>(source + _position);

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

        fixed (byte* source = _data)
        {
            if (source[_position] == 0xFF)
            {
                if (_position + 2 >= Length)
                {
                    data = default;
                    return false;
                }

                size = Unsafe.ReadUnaligned<ushort>(source + _position + 1);

                if (_position + 2 + size >= Length)
                {
                    data = default;
                    return false;
                }

                _position += 3;
            }
            else
            {
                size = source[_position];

                if (_position + size >= Length)
                {
                    data = default;
                    return false;
                }

                _position++;
            }

            if (size == 0)
                data = string.Empty;
            else
                data = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(source + _position, size));
        }

        _position += (ushort)size;
        return true;
    }

    public bool Read(byte[] data)
    {
        if (_position + data.Length > Length)
            return false;

        if (data.Length == 0)
            return true;

        fixed (byte* source = _data)
        fixed (byte* target = data)
            Unsafe.CopyBlockUnaligned(target, source + _position, (uint)data.Length);

        _position += (ushort)data.Length;
        return true;
    }
}
