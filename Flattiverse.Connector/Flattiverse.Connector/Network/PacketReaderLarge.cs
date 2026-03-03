using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

struct PacketReaderLarge
{
    public readonly byte Command;
    public readonly byte Session;
    public readonly ushort Length;

    private int _position;
    private readonly byte[] _data;

    public PacketReaderLarge(byte[] data, int basePosition)
    {
        ushort size = Unsafe.As<byte, ushort>(ref data[basePosition + 2]);

        Command = data[basePosition];
        Session = data[basePosition + 1];
        Length = size;
        _position = 0;
        _data = GC.AllocateUninitializedArray<byte>(size, false);

        if (size > 0)
            Buffer.BlockCopy(data, basePosition + 4, _data, 0, size);
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

    public bool Read(out uint data)
    {
        if (_position + 3 >= Length)
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, uint>(ref _data[_position]);
        _position += 4;
        return true;
    }

    public bool Read(out float data)
    {
        if (_position + 3 >= Length)
        {
            data = default;
            return false;
        }

        data = Unsafe.As<byte, float>(ref _data[_position]);
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

        if (_data[_position] == 0xFF)
        {
            if (_position + 2 >= Length)
            {
                data = default;
                return false;
            }

            size = Unsafe.As<byte, ushort>(ref _data[_position + 1]);

            if (_position + 2 + size >= Length)
            {
                data = default;
                return false;
            }

            _position += 3;
        }
        else
        {
            size = _data[_position];

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
            data = Encoding.UTF8.GetString(_data, _position, size);

        _position += size;
        return true;
    }

    public bool Read(byte[] data)
    {
        if (_position + data.Length > Length)
            return false;

        if (data.Length == 0)
            return true;

        Buffer.BlockCopy(_data, _position, data, 0, data.Length);
        _position += data.Length;
        return true;
    }
}
