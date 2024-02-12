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
}