namespace Flattiverse.Connector.Network;

enum CommandParameterKind
{
    SByte = 0x00,
    Byte = 0x01,
    Short = 0x02,
    UShort = 0x03,
    Int = 0x04,
    UInt = 0x05,
    Long = 0x06,
    ULong = 0x07,
    Float = 0x10,
    String = 0x20,
    GameMode = 0x40,
    PlayerKind = 0x41,
    Team = 0x80
}