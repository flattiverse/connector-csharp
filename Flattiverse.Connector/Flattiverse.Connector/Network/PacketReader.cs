using System.Diagnostics;
using System.Runtime.CompilerServices;

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

    public int ReadInt32()
    {
        Debug.Assert(position + 4 <= end, "Can't read out of bounds.");

        position += 4;
        
        return Unsafe.As<byte, int>(ref data[position - 4]);
    }
}