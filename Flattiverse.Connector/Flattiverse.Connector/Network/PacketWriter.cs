using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flattiverse.Connector.Network;

class PacketWriter : IDisposable
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

    /// <summary>
    /// Writes a double as signed byte.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write1S(double number, double shift)
    {
        Debug.Assert(position + 1 < end, "Can't write out of bounds.");

        Unsafe.As<byte, sbyte>(ref data[position]) = (sbyte)(number * shift + 0.5);

        position += 1;
    }

    /// <summary>
    /// Writes a double as unsigned byte.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write1U(double number, double shift)
    {
        Debug.Assert(position + 1 < end, "Can't write out of bounds.");

        Unsafe.As<byte, byte>(ref data[position]) = (byte)(number * shift + 0.5);

        position += 1;
    }

    /// <summary>
    /// Writes a double as 2 byte signed short.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write2S(double number, double shift)
    {
        Debug.Assert(position + 2 < end, "Can't write out of bounds.");

        Unsafe.As<byte, short>(ref data[position]) = (short)(number * shift + 0.5);

        position += 2;
    }

    /// <summary>
    /// Writes a double as 2 byte unsigned short.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write2U(double number, double shift)
    {
        Debug.Assert(position + 2 < end, "Can't write out of bounds.");

        Unsafe.As<byte, ushort>(ref data[position]) = (ushort)(number * shift + 0.5);

        position += 2;
    }

    /// <summary>
    /// Writes a double as 4 byte signed integer.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write4S(double number, double shift)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");

        Unsafe.As<byte, int>(ref data[position]) = (int)(number * shift + 0.5);

        position += 4;
    }

    /// <summary>
    /// Writes a double as 4 byte unsigned integer.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write4U(double number, double shift)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");

        Unsafe.As<byte, uint>(ref data[position]) = (uint)(number * shift + 0.5);

        position += 4;
    }

    /// <summary>
    /// Writes a double as 8 byte signed long.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write8S(double number, double shift)
    {
        Debug.Assert(position + 8 < end, "Can't write out of bounds.");

        Unsafe.As<byte, long>(ref data[position]) = (long)(number * shift + 0.5);

        position += 8;
    }

    /// <summary>
    /// Writes a double as 8 byte unsigned long.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. e.g. specify 100 to include the first two decimal places.</param>
    public void Write8U(double number, double shift)
    {
        Debug.Assert(position + 8 < end, "Can't write out of bounds.");

        Unsafe.As<byte, ulong>(ref data[position]) = (ulong)(number * shift + 0.5);

        position += 8;
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