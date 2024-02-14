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

    //TODO MALUK CHECK
    internal void Write(string message)
    {
        byte[] msg = Encoding.ASCII.GetBytes(message);

        Debug.Assert(position + msg.Length < end, "Can't write out of bounds.");

        Unsafe.CopyBlock(ref data[position], ref msg[0], (uint)msg.Length);
        position += msg.Length;
    }
    
    public void Dispose()
    {
        packet.Header.Size = (ushort)(position - packet.Offset);
    }
}