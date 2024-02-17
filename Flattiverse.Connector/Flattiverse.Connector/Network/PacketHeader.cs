using System.Runtime.InteropServices;

namespace Flattiverse.Connector.Network;

[StructLayout(LayoutKind.Explicit, Size = 8)]
internal struct PacketHeader
{
    [FieldOffset(0)]
    public byte Command;
        
    [FieldOffset(1)]
    public byte Session;
        
    [FieldOffset(2)]
    public byte Id0;
        
    [FieldOffset(3)]
    public byte Id1;

    [FieldOffset(2)]
    public ushort Id;

    [FieldOffset(4)]
    public byte Param0;
        
    [FieldOffset(5)]
    public byte Param1;
    
    [FieldOffset(4)]
    public ushort Param;
        
    [FieldOffset(6)]
    public ushort Size;
        
    [FieldOffset(0)]
    public ulong DirectAssign;

    public PacketHeader(byte command, byte session, byte id0, byte id1, byte param0, byte param1, ushort param)
    {
        Command = command;
        Session = session;
        Id0 = id0;
        Id1 = id1;
        Param0 = param0;
        Param1 = param1;
        Param = param;
    }
}