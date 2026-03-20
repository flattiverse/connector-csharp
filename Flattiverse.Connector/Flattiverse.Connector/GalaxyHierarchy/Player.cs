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
    private bool _admin;
    private int _rank;
    private long _playerKills;
    private long _playerDeaths;
    private long _friendlyKills;
    private long _friendlyDeaths;
    private long _npcKills;
    private long _npcDeaths;
    private long _neutralDeaths;
    private readonly Score _score;

    /// <summary>
    /// Session-level runtime self-disclosure, if provided by the player.
    /// </summary>
    public readonly RuntimeDisclosure? RuntimeDisclosure;

    /// <summary>
    /// Session-level build-assistance self-disclosure, if provided by the player.
    /// </summary>
    public readonly BuildDisclosure? BuildDisclosure;

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

    internal Player(Galaxy galaxy, byte id, PlayerKind kind, Team team, string name, float ping, bool admin, int rank,
        long playerKills, long playerDeaths, long friendlyKills, long friendlyDeaths, long npcKills, long npcDeaths,
        long neutralDeaths, RuntimeDisclosure? runtimeDisclosure, BuildDisclosure? buildDisclosure)
    {
        Galaxy = galaxy;
        _score = new Score();
        
        Id = id;

        Kind = kind;
        Team = team;

        _name = name;
        
        _ping = ping;
        _admin = admin;
        _rank = rank;
        _playerKills = playerKills;
        _playerDeaths = playerDeaths;
        _friendlyKills = friendlyKills;
        _friendlyDeaths = friendlyDeaths;
        _npcKills = npcKills;
        _npcDeaths = npcDeaths;
        _neutralDeaths = neutralDeaths;
        RuntimeDisclosure = runtimeDisclosure;
        BuildDisclosure = buildDisclosure;
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
        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xC6;
            writer.Write(Id);
            writer.Write(message);
        });
    }

    /// <summary>
    /// Downloads the player's cached small avatar image bytes.
    /// </summary>
    public async Task<byte[]> DownloadSmallAvatar()
    {
        PacketReaderLarge reader = await Galaxy.Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xC7;
            writer.Write(Id);
        }).ConfigureAwait(false);

        byte[] avatar = new byte[reader.Length];

        if (!reader.Read(avatar))
            throw new InvalidDataException("Couldn't read small avatar payload.");

        return avatar;
    }

    /// <summary>
    /// Downloads the player's cached big avatar image bytes.
    /// </summary>
    public async Task<byte[]> DownloadBigAvatar()
    {
        PacketReaderLarge reader = await Galaxy.Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xC8;
            writer.Write(Id);
        }).ConfigureAwait(false);

        byte[] avatar = new byte[reader.Length];

        if (!reader.Read(avatar))
            throw new InvalidDataException("Couldn't read big avatar payload.");

        return avatar;
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

    /// <summary>
    /// True, if the account has administrator privileges.
    /// </summary>
    public bool Admin => _admin;

    /// <summary>
    /// Global account rank.
    /// </summary>
    public int Rank => _rank;

    /// <summary>
    /// Total kills of other players.
    /// </summary>
    public long PlayerKills => _playerKills;

    /// <summary>
    /// Total deaths caused by other players.
    /// </summary>
    public long PlayerDeaths => _playerDeaths;

    /// <summary>
    /// Total kills of teammates.
    /// </summary>
    public long FriendlyKills => _friendlyKills;

    /// <summary>
    /// Total deaths caused by the same team, including self-inflicted deaths.
    /// </summary>
    public long FriendlyDeaths => _friendlyDeaths;

    /// <summary>
    /// Total kills of NPC enemies.
    /// </summary>
    public long NpcKills => _npcKills;

    /// <summary>
    /// Total deaths caused by NPC enemies.
    /// </summary>
    public long NpcDeaths => _npcDeaths;

    /// <summary>
    /// Total deaths caused by neutral units or the environment.
    /// </summary>
    public long NeutralDeaths => _neutralDeaths;

    /// <summary>
    /// Current live player score.
    /// </summary>
    public Score Score => _score;

    internal void Update(float ping, bool admin, int rank, long playerKills, long playerDeaths, long friendlyKills,
        long friendlyDeaths, long npcKills, long npcDeaths, long neutralDeaths)
    {
        _ping = ping;
        _admin = admin;
        _rank = rank;
        _playerKills = playerKills;
        _playerDeaths = playerDeaths;
        _friendlyKills = friendlyKills;
        _friendlyDeaths = friendlyDeaths;
        _npcKills = npcKills;
        _npcDeaths = npcDeaths;
        _neutralDeaths = neutralDeaths;
    }

    internal void Deactivate()
    {
        _ping = -1;
        
        _active = false;

        foreach (ControllableInfo? controllableInfo in _controllableInfos)
            if (controllableInfo is not null)
                controllableInfo.Deactivate();
    }
}
