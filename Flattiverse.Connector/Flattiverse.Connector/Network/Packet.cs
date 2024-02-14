using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Flattiverse.Connector.Network;

class Packet
{
    public PacketHeader Header;

    public byte[] Payload;

    public int Offset;

    //TODO MALUK CHECK
    public Packet(PacketHeader header)
    {
        Header = header;
        Payload = new byte[1048];
        Offset = 8;
    }

    /// <summary>
    /// Parses a packet from a byte[].
    /// </summary>
    /// <param name="src">The byte array we read the data from.</param>
    /// <param name="position">The position where we read data.</param>
    public Packet(byte[] src, ref int position)
    {
        Header.DirectAssign = Unsafe.As<byte, ulong>(ref src[position]);
        Payload = src;
        Offset = position + 8;

        position += 8 + Header.Size;
    }

    public PacketWriter Write()
    {
        return new PacketWriter(this);
    }

    public PacketReader Read()
    {
        return new PacketReader(this);
    }

    internal void Flush()
    {
        Unsafe.As<byte, ulong>(ref Payload[0]) = Header.DirectAssign;
    }

    public override string ToString()
    {
        return $"cmd=0x{Header.Command:X02}; session=0x{Header.Session:X02}; player=0x{Header.Player:X02}; controllable=0x{Header.Controllable:X02}; params=(0x{Header.Param0:X02}; 0x{Header.Param1:X02} | 0x{Header.Param:X04}); size=0x{Header.Size:X02}";
    }

}
