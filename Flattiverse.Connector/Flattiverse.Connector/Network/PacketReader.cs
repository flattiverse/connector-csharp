using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector.Network;

internal class PacketReader
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

    public byte[] ReadBytes(int amount)
    {
        Debug.Assert(position + amount <= end, "Can't read out of bounds.");

        position += amount;

        byte[] result = new byte[amount];
        
        Unsafe.CopyBlock(ref result[0], ref data[position - amount], (uint)amount);
        
        return result;
    }

    public byte ReadByte()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return data[position - 1];
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

    public double ReadDouble()
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        float number = Unsafe.As<byte, float>(ref data[position]); 
        
        position += 4;

        if (float.IsFinite(number))
            return number;

        return 1E40;
    }

    public bool ReadBoolean()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        position += 1;

        return data[position - 1] == 1;
    }

    public string ReadString()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        int length = data[position];

        Debug.Assert(position + 1 + length <= end, "Can't read out of bounds.");

        position += 1 + length;

        return Encoding.UTF8.GetString(data, position - length, length);
    }

    public string PeekString()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        int length = data[position];

        Debug.Assert(position + 1 + length <= end, "Can't read out of bounds.");

        return Encoding.UTF8.GetString(data, position + 1, length);
    }
    
    public void JumpOverString()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        int length = data[position];

        Debug.Assert(position + 1 + length <= end, "Can't read out of bounds.");

        position += 1 + length;
    }

    internal byte? ReadNullableByte()
    {
        Debug.Assert(position + 1 <= end, "Can't read out of bounds.");

        if (Unsafe.As<byte, byte>(ref data[position]) == 1)
        {
            Debug.Assert(position + 2 <= end, "Can't read out of bounds.");

            position += 2;

            return data[position - 1];
        }

        position += 1;
        return null;
    }
}