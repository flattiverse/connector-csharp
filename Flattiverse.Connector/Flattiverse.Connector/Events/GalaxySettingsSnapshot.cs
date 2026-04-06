using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Immutable snapshot of all server-driven galaxy setting values mirrored by the connector.
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
    public readonly ushort GalaxyMaxModernShips;

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
    public readonly ushort TeamMaxModernShips;

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
    public readonly byte PlayerMaxModernShips;

    /// <summary>
    /// Maintenance mode flag.
    /// </summary>
    public readonly bool Maintenance;

    /// <summary>
    /// Whether regular player logins must provide runtime and build self-disclosure.
    /// </summary>
    public readonly bool RequiresSelfDisclosure;

    /// <summary>
    /// Optional achievement key required for regular player logins.
    /// </summary>
    public readonly string? RequiredAchievement;

    internal GalaxySettingsSnapshot(GameMode gameMode, string name, string description, byte maxPlayers, ushort maxSpectators,
        ushort galaxyMaxTotalShips, ushort galaxyMaxClassicShips, ushort galaxyMaxModernShips,
        ushort teamMaxTotalShips, ushort teamMaxClassicShips, ushort teamMaxModernShips,
        byte playerMaxTotalShips, byte playerMaxClassicShips, byte playerMaxModernShips, bool maintenance,
        bool requiresSelfDisclosure, string? requiredAchievement)
    {
        GameMode = gameMode;
        Name = name;
        Description = description;

        MaxPlayers = maxPlayers;
        MaxSpectators = maxSpectators;

        GalaxyMaxTotalShips = galaxyMaxTotalShips;
        GalaxyMaxClassicShips = galaxyMaxClassicShips;
        GalaxyMaxModernShips = galaxyMaxModernShips;

        TeamMaxTotalShips = teamMaxTotalShips;
        TeamMaxClassicShips = teamMaxClassicShips;
        TeamMaxModernShips = teamMaxModernShips;

        PlayerMaxTotalShips = playerMaxTotalShips;
        PlayerMaxClassicShips = playerMaxClassicShips;
        PlayerMaxModernShips = playerMaxModernShips;
        Maintenance = maintenance;
        RequiresSelfDisclosure = requiresSelfDisclosure;
        RequiredAchievement = requiredAchievement;
    }
}
