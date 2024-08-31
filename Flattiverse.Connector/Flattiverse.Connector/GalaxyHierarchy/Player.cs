namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Represents a player in the galaxy.
/// </summary>
public class Player : INamedUnit
{
    /// <summary>
    /// The Id of the player.
    /// </summary>
    public readonly byte Id;

    /// <summary>
    /// The kind of the player.
    /// </summary>
    public readonly PlayerKind Kind;
    
    /// <summary>
    /// The team the player belongs to.
    /// </summary>
    public readonly Team Team;

    private readonly string _name;
    
    private float _ping;

    private bool _active;

    internal Player(byte id, PlayerKind kind, Team team, string name, float ping)
    {
        Id = id;

        Kind = kind;
        Team = team;

        _name = name;
        
        _ping = ping;
        _active = true;
    }
    
    /// <summary>
    /// The account name.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// The ping in ms of the player.
    /// </summary>
    public float Ping => _ping;

    internal void Update(float ping)
    {
        _ping = ping;
    }

    internal void Deactivate()
    {
        _ping = -1;
        
        _active = false;
    }
}