namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The live score state of a player or team inside one galaxy session.
/// </summary>
public class Score
{
    private uint _kills;
    private uint _deaths;
    private uint _mission;

    internal Score()
    {
    }

    internal void Update(uint kills, uint deaths, uint mission)
    {
        _kills = kills;
        _deaths = deaths;
        _mission = mission;
    }

    /// <summary>
    /// Number of kills in the current galaxy runtime.
    /// </summary>
    public uint Kills => _kills;

    /// <summary>
    /// Number of deaths in the current galaxy runtime.
    /// </summary>
    public uint Deaths => _deaths;

    /// <summary>
    /// Number of mission points in the current galaxy runtime.
    /// </summary>
    public uint Mission => _mission;
}
