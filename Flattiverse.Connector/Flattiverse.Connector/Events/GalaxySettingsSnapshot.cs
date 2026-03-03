using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Snapshot of all server-driven galaxy setting values.
/// </summary>
public class GalaxySettingsSnapshot
{
    /// <summary>
    /// Active game mode.
    /// </summary>
    public readonly GameMode GameMode;

    /// <summary>
    /// Galaxy name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Galaxy description.
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// Maximum connected players.
    /// </summary>
    public readonly byte MaxPlayers;

    /// <summary>
    /// Maximum connected spectators.
    /// </summary>
    public readonly ushort MaxSpectators;

    /// <summary>
    /// Maximum total ships for the whole galaxy.
    /// </summary>
    public readonly ushort GalaxyMaxTotalShips;

    /// <summary>
    /// Maximum classic ships for the whole galaxy.
    /// </summary>
    public readonly ushort GalaxyMaxClassicShips;

    /// <summary>
    /// Maximum new ships for the whole galaxy.
    /// </summary>
    public readonly ushort GalaxyMaxNewShips;

    /// <summary>
    /// Maximum bases for the whole galaxy.
    /// </summary>
    public readonly ushort GalaxyMaxBases;

    /// <summary>
    /// Maximum total ships per team.
    /// </summary>
    public readonly ushort TeamMaxTotalShips;

    /// <summary>
    /// Maximum classic ships per team.
    /// </summary>
    public readonly ushort TeamMaxClassicShips;

    /// <summary>
    /// Maximum new ships per team.
    /// </summary>
    public readonly ushort TeamMaxNewShips;

    /// <summary>
    /// Maximum bases per team.
    /// </summary>
    public readonly ushort TeamMaxBases;

    /// <summary>
    /// Maximum total ships per player.
    /// </summary>
    public readonly byte PlayerMaxTotalShips;

    /// <summary>
    /// Maximum classic ships per player.
    /// </summary>
    public readonly byte PlayerMaxClassicShips;

    /// <summary>
    /// Maximum new ships per player.
    /// </summary>
    public readonly byte PlayerMaxNewShips;

    /// <summary>
    /// Maximum bases per player.
    /// </summary>
    public readonly byte PlayerMaxBases;

    /// <summary>
    /// Maintenance mode flag.
    /// </summary>
    public readonly bool Maintenance;

    internal GalaxySettingsSnapshot(GameMode gameMode, string name, string description, byte maxPlayers, ushort maxSpectators,
        ushort galaxyMaxTotalShips, ushort galaxyMaxClassicShips, ushort galaxyMaxNewShips, ushort galaxyMaxBases,
        ushort teamMaxTotalShips, ushort teamMaxClassicShips, ushort teamMaxNewShips, ushort teamMaxBases,
        byte playerMaxTotalShips, byte playerMaxClassicShips, byte playerMaxNewShips, byte playerMaxBases, bool maintenance)
    {
        GameMode = gameMode;
        Name = name;
        Description = description;

        MaxPlayers = maxPlayers;
        MaxSpectators = maxSpectators;

        GalaxyMaxTotalShips = galaxyMaxTotalShips;
        GalaxyMaxClassicShips = galaxyMaxClassicShips;
        GalaxyMaxNewShips = galaxyMaxNewShips;
        GalaxyMaxBases = galaxyMaxBases;

        TeamMaxTotalShips = teamMaxTotalShips;
        TeamMaxClassicShips = teamMaxClassicShips;
        TeamMaxNewShips = teamMaxNewShips;
        TeamMaxBases = teamMaxBases;

        PlayerMaxTotalShips = playerMaxTotalShips;
        PlayerMaxClassicShips = playerMaxClassicShips;
        PlayerMaxNewShips = playerMaxNewShips;
        PlayerMaxBases = playerMaxBases;
        Maintenance = maintenance;
    }
}
