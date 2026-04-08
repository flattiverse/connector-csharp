using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Represents one player account inside the connected galaxy session.
/// </summary>
public class Player : INamedUnit
{
    /// <summary>
    /// Protocol id of the player.
    /// </summary>
    public readonly byte Id;

    /// <summary>
    /// Login kind of the player, for example normal player, spectator, or admin.
    /// </summary>
    public readonly PlayerKind Kind;
    
    /// <summary>
    /// The team the player belongs to.
    /// </summary>
    public readonly Team Team;

    private readonly string _name;
    
    private float _ping;
    private bool _admin;
    private bool _disconnected;
    private int _rank;
    private long _playerKills;
    private long _playerDeaths;
    private long _friendlyKills;
    private long _friendlyDeaths;
    private long _npcKills;
    private long _npcDeaths;
    private long _neutralDeaths;
    private readonly bool _hasAvatar;
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
    /// Owner-side roster entries of all controllables registered to this player in the current galaxy session.
    /// </summary>
    public readonly UniversalHolder<ControllableInfo> ControllableInfos;
    
    /// <summary>
    /// The connected galaxy session this player belongs to.
    /// </summary>
    public readonly Galaxy Galaxy;

    internal Player(Galaxy galaxy, byte id, PlayerKind kind, Team team, string name, float ping, bool admin, bool disconnected, int rank,
        long playerKills, long playerDeaths, long friendlyKills, long friendlyDeaths, long npcKills, long npcDeaths,
        long neutralDeaths, bool hasAvatar, RuntimeDisclosure? runtimeDisclosure, BuildDisclosure? buildDisclosure)
    {
        Galaxy = galaxy;
        _score = new Score();
        
        Id = id;

        Kind = kind;
        Team = team;

        _name = name;
        
        _ping = ping;
        _admin = admin;
        _disconnected = disconnected;
        _rank = rank;
        _playerKills = playerKills;
        _playerDeaths = playerDeaths;
        _friendlyKills = friendlyKills;
        _friendlyDeaths = friendlyDeaths;
        _npcKills = npcKills;
        _npcDeaths = npcDeaths;
        _neutralDeaths = neutralDeaths;
        _hasAvatar = hasAvatar;
        RuntimeDisclosure = runtimeDisclosure;
        BuildDisclosure = buildDisclosure;
        _active = true;

        _controllableInfos = new ControllableInfo?[256];
        
        ControllableInfos = new UniversalHolder<ControllableInfo>(_controllableInfos);
    }
    
    /// <summary>
    /// Sends a private chat message to the player.
    /// </summary>
    /// <param name="message">The message to send.</param>
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
    /// Sends one private binary chat message to the player.
    /// The first binary message opens the channel; further binary messages require a binary reply from the target player.
    /// </summary>
    /// <param name="message">Raw binary message payload.</param>
    public async Task Chat(byte[] message)
    {
        ArgumentNullException.ThrowIfNull(message);

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xCC;
            writer.Write(Id);
            writer.Write((ushort)message.Length);
            writer.Write(message);
        });
    }

    /// <summary>
    /// Sends up to 32 private binary chat messages in one protocol packet.
    /// Bulk sending is only allowed after the target player has acknowledged the binary channel with a binary reply.
    /// </summary>
    /// <param name="messages">Raw binary message payloads to send in order.</param>
    public async Task Chat(List<byte[]> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);

        for (int index = 0; index < messages.Count; index++)
            ArgumentNullException.ThrowIfNull(messages[index]);

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xCD;
            writer.Write(Id);
            writer.Write((byte)messages.Count);

            for (int index = 0; index < messages.Count; index++)
            {
                byte[] message = messages[index];
                writer.Write((ushort)message.Length);
                writer.Write(message);
            }
        });
    }

    /// <summary>
    /// Downloads the player's cached small avatar image bytes.
    /// </summary>
    /// <exception cref="AvatarNotAvailableGameException">
    /// Thrown, if the player currently has no cached avatar on the server.
    /// </exception>
    public async Task<byte[]> DownloadSmallAvatar(ProgressState? progressState = null)
    {
        if (!_hasAvatar)
            throw new AvatarNotAvailableGameException();

        return await ChunkedTransfer.DownloadBytes(Galaxy.Connection, delegate (ref PacketWriter writer, int offset, ushort maximumLength)
        {
            writer.Command = 0xF1;
            writer.Write(Id);
            writer.Write(offset);
            writer.Write(maximumLength);
        }, progressState, "small avatar").ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads the player's cached big avatar image bytes.
    /// </summary>
    /// <exception cref="AvatarNotAvailableGameException">
    /// Thrown, if the player currently has no cached avatar on the server.
    /// </exception>
    public async Task<byte[]> DownloadBigAvatar(ProgressState? progressState = null)
    {
        if (!_hasAvatar)
            throw new AvatarNotAvailableGameException();

        return await ChunkedTransfer.DownloadBytes(Galaxy.Connection, delegate (ref PacketWriter writer, int offset, ushort maximumLength)
        {
            writer.Command = 0xF2;
            writer.Write(Id);
            writer.Write(offset);
            writer.Write(maximumLength);
        }, progressState, "big avatar").ConfigureAwait(false);
    }
    
    /// <summary>
    /// Account display name inside this galaxy session.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// True while this player is still represented in the current galaxy session.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// True when the player's connection has already dropped and only session cleanup remains.
    /// </summary>
    public bool Disconnected => _disconnected;
    
    /// <summary>
    /// Latest ping in milliseconds reported by the server.
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
    /// True if this player currently has a cached avatar available on the server.
    /// </summary>
    public bool HasAvatar => _hasAvatar;

    /// <summary>
    /// Current live player score.
    /// </summary>
    public Score Score => _score;

    internal void Update(float ping, bool admin, bool disconnected, int rank, long playerKills, long playerDeaths, long friendlyKills,
        long friendlyDeaths, long npcKills, long npcDeaths, long neutralDeaths)
    {
        _ping = ping;
        _admin = admin;
        _disconnected = disconnected;
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
        _disconnected = true;

        foreach (ControllableInfo? controllableInfo in _controllableInfos)
            if (controllableInfo is not null)
                controllableInfo.Deactivate();
    }
}
