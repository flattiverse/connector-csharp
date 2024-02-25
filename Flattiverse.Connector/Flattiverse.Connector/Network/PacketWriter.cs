using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flattiverse.Connector.Network;

internal class PacketWriter : IDisposable
{
    private readonly Packet packet;

    private readonly byte[] data;
    private int position;

    private readonly int end;
    
    public PacketWriter(Packet packet)
    {
        this.packet = packet;

        data = packet.Payload;
        
        position = packet.Offset;
        end = position + 1040;
    }

    public void WriteZeroedBytes(int amount)
    {
        Debug.Assert(position + amount < end, "Can't write out of bounds.");

        for (int subPosition = 0; subPosition < amount; subPosition++)
            data[position++] = 0;
    }

    public void Write(sbyte number)
    {
        Debug.Assert(position + 1 < end, "Can't write out of bounds.");

        Unsafe.As<byte, sbyte>(ref data[position]) = number;

        position += 1;
    }

    public void Write(byte number)
    {
        Debug.Assert(position + 1 < end, "Can't write out of bounds.");

        Unsafe.As<byte, byte>(ref data[position]) = number;

        position += 1;
    }

    public void Write(short number)
    {
        Debug.Assert(position + 2 < end, "Can't write out of bounds.");

        Unsafe.As<byte, short>(ref data[position]) = number;

        position += 2;
    }

    public void Write(ushort number)
    {
        Debug.Assert(position + 2 < end, "Can't write out of bounds.");

        Unsafe.As<byte, ushort>(ref data[position]) = number;

        position += 2;
    }

    public void Write(int number)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");

        Unsafe.As<byte, int>(ref data[position]) = number;

        position += 4;
    }

    public void Write(uint number)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");

        Unsafe.As<byte, uint>(ref data[position]) = number;

        position += 4;
    }

    public void Write(long number)
    {
        Debug.Assert(position + 8 < end, "Can't write out of bounds.");

        Unsafe.As<byte, long>(ref data[position]) = number;

        position += 8;
    }

    public void Write(ulong number)
    {
        Debug.Assert(position + 8 < end, "Can't write out of bounds.");

        Unsafe.As<byte, ulong>(ref data[position]) = number;

        position += 8;
    }

    public void Write(double number)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");
        
        Unsafe.As<byte, float>(ref data[position]) = (float)number;
        
        position += 4;
    }

    /// <summary>
    /// Writes a boolean value.
    /// </summary>
    /// <param name="value">The boolean to write.</param>
    public void Write(bool value)
    {
        Debug.Assert(position + 1 < end, "Can't write out of bounds.");

        if (value)
            Unsafe.As<byte, byte>(ref data[position + 1]) = 1;
        else
            Unsafe.As<byte, byte>(ref data[position + 1]) = 0;

        position += 1;
    }

    /// <summary>
    /// Writes a string with a maximum length of 64 chars using UTF-8.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void Write(string text)
    {
        int len = Encoding.UTF8.GetBytes(text, data.AsSpan(position + 1, end - position - 1));

        data[position] = (byte)len;

        position += 1 + len;
    }

    internal void WriteNullable(byte? number)
    {
        if (number.HasValue)
        {
            Debug.Assert(position + 2 < end, "Can't write out of bounds.");

            Unsafe.As<byte, byte>(ref data[position]) = 1;
            Unsafe.As<byte, byte>(ref data[position + 1]) = number.Value;

            position += 2;
        }
        else
        {
            Debug.Assert(position + 1 < end, "Can't write out of bounds.");

            Unsafe.As<byte, byte>(ref data[position]) = 0;

            position += 1;
        }
    }

    public void Dispose()
    {
        packet.Header.Size = (ushort)(position - packet.Offset);
    }
}