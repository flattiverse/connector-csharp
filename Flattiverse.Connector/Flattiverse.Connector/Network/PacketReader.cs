using Flattiverse.Connector.Units;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flattiverse.Connector.Network;

class PacketReader
{
    private byte[] data;
    private int position;

    private readonly int end;

    public PacketReader(Packet packet)
    {
        data = packet.Payload;

        position = packet.Offset;
        end = position + packet.Header.Size;
    }

    public sbyte ReadSByte()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return Unsafe.As<byte, sbyte>(ref data[position - 1]);
    }

    public byte ReadByte()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return Unsafe.As<byte, byte>(ref data[position - 1]);
    }

    public short ReadInt16()
    {
        Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

        position += 2;

        return Unsafe.As<byte, short>(ref data[position - 2]);
    }

    public ushort ReadUInt16()
    {
        Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

        position += 2;

        return Unsafe.As<byte, ushort>(ref data[position - 2]);
    }

    public int ReadInt32()
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        position += 4;

        return Unsafe.As<byte, int>(ref data[position - 4]);
    }

    public uint ReadUInt32()
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        position += 4;

        return Unsafe.As<byte, uint>(ref data[position - 4]);
    }

    public long ReadInt64()
    {
        Debug.Assert(position + 8 <= end, "Can't read out of bounds.");

        position += 8;

        return Unsafe.As<byte, long>(ref data[position - 8]);
    }

    public ulong ReadUInt64()
    {
        Debug.Assert(position + 8 <= end, "Can't read out of bounds.");

        position += 8;

        return Unsafe.As<byte, ulong>(ref data[position - 8]);
    }

    public double Read1S(double shift)
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return Unsafe.As<byte, sbyte>(ref data[position - 1]) / shift;
    }

    public double Read1U(double shift)
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return Unsafe.As<byte, byte>(ref data[position - 1]) / shift;
    }

    public double Read2S(double shift)
    {
        Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

        position += 2;

        return Unsafe.As<byte, short>(ref data[position - 2]) / shift;
    }

    public double Read2U(double shift)
    {
        Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

        position += 2;

        return Unsafe.As<byte, ushort>(ref data[position - 2]) / shift;
    }

    public double Read4S(double shift)
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        position += 4;

        return Unsafe.As<byte, int>(ref data[position - 4]) / shift;
    }

    public double Read4U(double shift)
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        position += 4;

        return Unsafe.As<byte, uint>(ref data[position - 4]) / shift;
    }

    public double Read8S(double shift)
    {
        Debug.Assert(position + 8 <= end, "Can't read out of bounds.");

        position += 8;

        return Unsafe.As<byte, long>(ref data[position - 8]) / shift;
    }

    public double Read8U(double shift)
    {
        Debug.Assert(position + 8 <= end, "Can't read out of bounds.");

        position += 8;

        return Unsafe.As<byte, ulong>(ref data[position - 8]) / shift;
    }

    public bool ReadBoolean()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return Unsafe.As<byte, byte>(ref data[position - 1]) == 1;
    }

    public string ReadString()
    {
        Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

        short length = Unsafe.As<byte, short>(ref data[position]);

        Debug.Assert(position + 2 + length <= end, "Can't read out of bounds.");

        position += 2 + length;

        return Encoding.UTF8.GetString(Unsafe.As<byte, byte[]>(ref data[position - length]));
    }

    internal byte? ReadNullableByte()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        if (Unsafe.As<byte, byte>(ref data[position]) == 1)
        {
            Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

            position += 2;

            return Unsafe.As<byte, byte>(ref data[position - 1]);
        }

        position += 1;
        return null;
    }
}