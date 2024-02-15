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

    public void Write(int number)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");
        
        Unsafe.As<byte, int>(ref data[position]) = number;
        position += 4;
    }
    
    /// <summary>
    /// Writes a double as 4 byte signed integer.
    /// </summary>
    /// <param name="number">The number to write.</param>
    /// <param name="shift">The shift of the decimal point. If you want to store 2 decimal places,
    /// you need to specify 100 here.</param>
    public void Write4S(double number, double shift)
    {
        Debug.Assert(position + 4 < end, "Can't write out of bounds.");
        
        Unsafe.As<byte, int>(ref data[position]) = (int)(number * shift + 0.5);
        position += 4;
    }
    
    public void Dispose()
    {
        packet.Header.Size = (ushort)(position - packet.Offset);
    }
}