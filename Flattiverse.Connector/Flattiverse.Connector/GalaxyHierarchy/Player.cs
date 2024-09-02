using Flattiverse.Connector.Network;

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
    
    internal ControllableInfo?[] _controllableInfos;
    
    /// <summary>
    /// Holds infos about all PlayerUnits this player controls.
    /// </summary>
    public readonly UniversalHolder<ControllableInfo> ControllableInfos;
    
    /// <summary>
    /// The galaxy (connection to flattiverse) the player belongs to.
    /// </summary>
    public readonly Galaxy Galaxy;

    internal Player(Galaxy galaxy, byte id, PlayerKind kind, Team team, string name, float ping)
    {
        Galaxy = galaxy;
        
        Id = id;

        Kind = kind;
        Team = team;

        _name = name;
        
        _ping = ping;
        _active = true;

        _controllableInfos = new ControllableInfo?[256];
        
        ControllableInfos = new UniversalHolder<ControllableInfo>(_controllableInfos);
    }
    
    /// <summary>
    /// Sends a private chat message to the player.
    /// </summary>
    /// <param name="message">The message ot send.</param>
    public async Task Chat(string message)
    {
        PacketWriter writer = new PacketWriter(new byte[2052]);

        writer.Command = 0xC6;
        
        writer.Write(Id);
        writer.Write(message);
        
        await Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }
    
    /// <summary>
    /// The account name.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// true, if the player is still used/connected to the game server.
    /// </summary>
    public bool Active => _active;
    
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