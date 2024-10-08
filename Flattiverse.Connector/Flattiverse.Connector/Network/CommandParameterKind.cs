﻿namespace Flattiverse.Connector.Network;

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
    UnitKind = 0x42,
    PlayerUnitDestroyedReason = 0x43,
    Team = 0x80,
    Player = 0x81,
    Cluster = 0x82,
    Controllable = 0x88,
    PacketReader = 0xFE,
    PacketWriter = 0xFF
}