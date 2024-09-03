using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Network;

class CommandParameter
{
    public readonly CommandParameterKind Kind;
    
    public readonly string Name;

    public CommandParameter(ParameterInfo info)
    {
        Name = info.Name!;

        switch (info.ParameterType)
        {
            case { } t when t == typeof(sbyte):
                Kind = CommandParameterKind.SByte;
                break;
            case { } t when t == typeof(byte):
                Kind = CommandParameterKind.Byte;
                break;
            case { } t when t == typeof(short):
                Kind = CommandParameterKind.Short;
                break;
            case { } t when t == typeof(ushort):
                Kind = CommandParameterKind.UShort;
                break;
            case { } t when t == typeof(int):
                Kind = CommandParameterKind.Int;
                break;
            case { } t when t == typeof(uint):
                Kind = CommandParameterKind.UInt;
                break;
            case { } t when t == typeof(long):
                Kind = CommandParameterKind.Long;
                break;
            case { } t when t == typeof(ulong):
                Kind = CommandParameterKind.ULong;
                break;
            case { } t when t == typeof(float):
                Kind = CommandParameterKind.Float;
                break;
            case { } t when t == typeof(string):
                Kind = CommandParameterKind.String;
                break;
            case { } t when t == typeof(GameMode):
                Kind = CommandParameterKind.GameMode;
                break;
            case { } t when t == typeof(PlayerKind):
                Kind = CommandParameterKind.PlayerKind;
                break;
            case { } t when t == typeof(UnitKind):
                Kind = CommandParameterKind.UnitKind;
                break;
            case { } t when t == typeof(PlayerUnitDestroyedReason):
                Kind = CommandParameterKind.PlayerUnitDestroyedReason;
                break;
            case { } t when t == typeof(Team):
                Kind = CommandParameterKind.Team;
                break;
            case { } t when t == typeof(Player):
                Kind = CommandParameterKind.Player;
                break;
            case { } t when t == typeof(Cluster):
                Kind = CommandParameterKind.Cluster;
                break;
            case { } t when t == typeof(Controllable):
                Kind = CommandParameterKind.Controllable;
                break;
            case { } t when t == typeof(PacketReader):
                Kind = CommandParameterKind.PacketReader;
                break;
            case { } t when t == typeof(PacketWriter):
                Kind = CommandParameterKind.PacketWriter;
                break;
            default:
                throw new ArgumentException($"Unknown parameter type: {info.ParameterType}");
        }
    }

    public bool TryRead(Galaxy galaxy, PacketReader reader, [NotNullWhen(true)] out object? value)
    {
        switch (Kind)
        {
            case CommandParameterKind.SByte:
                // if (reader.Read(out sbyte sb) && MinValue <= b && b <= MaxValue)
                // {
                //     value = sb;
                //     return true;
                // }

                value = null;
                return false;
            case CommandParameterKind.Byte:
                if (reader.Read(out byte b))
                {
                    value = b;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Short:
                // if (reader.Read(out short s) && MinValue <= s && s <= MaxValue)
                // {
                //     value = s;
                //     return true;
                // }

                value = null;
                return false;
            case CommandParameterKind.UShort:
                if (reader.Read(out ushort us))
                {
                    value = us;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Int:
                if (reader.Read(out int i))
                {
                    value = i;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.UInt:
                // if (reader.Read(out uint ui) && MinValue <= ui && ui <= MaxValue)
                // {
                //     value = ui;
                //     return true;
                // }

                value = null;
                return false;
            case CommandParameterKind.Long:
                // if (reader.Read(out long l) && MinValue <= l && l <= MaxValue)
                // {
                //     value = l;
                //     return true;
                // }

                value = null;
                return false;
            case CommandParameterKind.ULong:
                // if (reader.Read(out ulong ul) && MinValue <= ul && ul <= MaxValue)
                // {
                //     value = ul;
                //     return true;
                // }

                value = null;
                return false;
            case CommandParameterKind.Float:
                if (reader.Read(out float f))
                {
                    value = f;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.String:
                if (reader.Read(out string str))
                {
                    value = str;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.GameMode:
                if (reader.Read(out byte gm))
                {
                    value = (GameMode)gm;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.PlayerKind:
                if (reader.Read(out byte pk))
                {
                    value = (PlayerKind)pk;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.UnitKind:
                if (reader.Read(out byte uk))
                {
                    value = (UnitKind)uk;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.PlayerUnitDestroyedReason:
                if (reader.Read(out byte pudr))
                {
                    value = (PlayerUnitDestroyedReason)pudr;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Team:
                if (reader.Read(out byte tId) && galaxy.Teams.TryGet(tId, out Team? tValue))
                {
                    value = tValue;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Player:
                if (reader.Read(out byte pId) && galaxy.Players.TryGet(pId, out Player? pValue))
                {
                    value = pValue;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Cluster:
                if (reader.Read(out byte cId) && galaxy.Clusters.TryGet(cId, out Cluster? cValue))
                {
                    value = cValue;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.Controllable:
                if (reader.Read(out byte conId) && galaxy.Controllables.TryGet(conId, out Controllable? controllable))
                {
                    value = controllable;
                    return true;
                }

                value = null;
                return false;
            case CommandParameterKind.PacketWriter:
                value = null;
                return false;
            case CommandParameterKind.PacketReader:
                value = reader;
                return true;
            default:
                value = null;
                return false;
        }
    }
}