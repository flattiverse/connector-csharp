using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

class PacketReader
{
    /// <summary>
    /// The byte[] which is the basis of all data.
    /// </summary>
    private byte[] _data;

    private int _basePosition;
    private int _position;
    private int _dataSize;

    public PacketReader(byte[] data)
    {
        _data = data;
    }

    public PacketReaderCopy Copy => new PacketReaderCopy(_data, _basePosition);
    
    public Memory<byte> FullMemory => _data.AsMemory(); 
    
    public bool Reset(int size)
    {
        Debug.Assert(size <= _data.Length, "Wrong size specified, size > byte[] size.");

        _basePosition = 0;
        _position = 0;
        _dataSize = size;

        return _basePosition + 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]) <= _dataSize;
    }

    public bool Next()
    {
        _basePosition += 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);
        _position = 0;

        return _basePosition + 4 + Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]) <= _dataSize;
    }

    public byte Command => _data[_basePosition];

    public byte Session => _data[_basePosition + 1];

    public ushort Size => Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]);

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

    public bool Read(byte[] data)
    {
        if (_position + data.Length > Unsafe.As<byte, ushort>(ref _data[_basePosition + 2]))
            return false;

        Unsafe.CopyBlock(ref data[0], ref _data[_position], (uint)data.Length);

        _position += data.Length;
        return true;
    }
}