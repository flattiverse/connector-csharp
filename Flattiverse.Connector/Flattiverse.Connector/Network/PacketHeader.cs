using System.Runtime.InteropServices;

namespace Flattiverse.Connector.Network;

[StructLayout(LayoutKind.Explicit, Size = 8)]
struct PacketHeader
{
    [FieldOffset(0)]
    public byte Command;
        
    [FieldOffset(1)]
    public byte Session;
        
    [FieldOffset(2)]
    public byte Player;
        
    [FieldOffset(3)]
    public byte Controllable;
        
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

    public PacketHeader(byte command, byte session, byte player, byte controllable, byte param0, byte param1, ushort param)
    {
        Command = command;
        Session = session;
        Player = player;
        Controllable = controllable;
        Param0 = param0;
        Param1 = param1;
        Param = param;
    }
}