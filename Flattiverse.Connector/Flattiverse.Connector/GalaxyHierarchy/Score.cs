namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The live score state of a player or team inside one galaxy session.
/// </summary>
public class Score
{
    private uint _playerKills;
    private uint _playerDeaths;
    private uint _friendlyKills;
    private uint _friendlyDeaths;
    private uint _npcKills;
    private uint _npcDeaths;
    private uint _neutralDeaths;
    private int _mission;

    internal Score()
    {
    }

    internal void Update(uint playerKills, uint playerDeaths, uint friendlyKills, uint friendlyDeaths, uint npcKills, uint npcDeaths,
        uint neutralDeaths, int mission)
    {
        _playerKills = playerKills;
        _playerDeaths = playerDeaths;
        _friendlyKills = friendlyKills;
        _friendlyDeaths = friendlyDeaths;
        _npcKills = npcKills;
        _npcDeaths = npcDeaths;
        _neutralDeaths = neutralDeaths;
        _mission = mission;
    }

    /// <summary>
    /// Number of kills of enemy players in the current galaxy runtime.
    /// </summary>
    public uint PlayerKills => _playerKills;

    /// <summary>
    /// Number of deaths caused by enemy players in the current galaxy runtime.
    /// </summary>
    public uint PlayerDeaths => _playerDeaths;

    /// <summary>
    /// Number of kills of teammates in the current galaxy runtime.
    /// </summary>
    public uint FriendlyKills => _friendlyKills;

    /// <summary>
    /// Number of deaths caused by the same team, including self-inflicted deaths, in the current galaxy runtime.
    /// </summary>
    public uint FriendlyDeaths => _friendlyDeaths;

    /// <summary>
    /// Number of kills of hostile NPCs in the current galaxy runtime.
    /// </summary>
    public uint NpcKills => _npcKills;

    /// <summary>
    /// Number of deaths caused by hostile NPCs in the current galaxy runtime.
    /// </summary>
    public uint NpcDeaths => _npcDeaths;

    /// <summary>
    /// Number of deaths caused by neutral units or the environment in the current galaxy runtime.
    /// </summary>
    public uint NeutralDeaths => _neutralDeaths;

    /// <summary>
    /// Number of mission points in the current galaxy runtime.
    /// </summary>
    public int Mission => _mission;
}
