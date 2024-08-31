using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.WebSockets;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
                               // Disabled because fields are guaranteed to be setup after the class instance is
                               // returned via the static constructor. 

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// This represents a flattiverse galaxy that you have connected to.
/// </summary>
public class Galaxy : IDisposable
{
    private const string Version = "0";
    
    private string _name;
    
    private GameMode _gameMode;
    private string _description;

    private byte _maxPlayers;
    private ushort _maxSpectators;
    
    private ushort _galaxyMaxTotalShips;
    private ushort _galaxyMaxClassicShips;
    private ushort _galaxyMaxNewShips;
    private ushort _galaxyMaxBases;
    
    private ushort _teamMaxTotalShips;
    private ushort _teamMaxClassicShips;
    private ushort _teamMaxNewShips;
    private ushort _teamMaxBases;
    
    private byte _playerMaxTotalShips;
    private byte _playerMaxClassicShips;
    private byte _playerMaxNewShips;
    private byte _playerMaxBases;

    private bool _maintenance;
    private bool _active;
    
    private readonly Team?[] _teams;
    private readonly Cluster?[] _clusters;
    
    private readonly Player?[] _players;
    
    private readonly Connection _connection;

    private Player _player;

    private readonly Queue<FlattiverseEvent> _events;
    private TaskCompletionSource? _eventsTcs;
    private readonly object _eventsSync;

    public readonly UniversalHolder<Team> Teams;
    public readonly UniversalHolder<Cluster> Clusters;
    public readonly UniversalHolder<Player> Players;
    
    /// <summary>
    /// Connectes to a galaxy on a specific URI.
    /// </summary>
    /// <param name="uri">The URI to connect to. This must be the full URI (link) to the galaxy end point.</param>
    /// <param name="auth">This should be your auth key or null, if you want to join as spectator.</param>
    /// <param name="team">This should be the name of the team you want to join or null if the galaxy should chose
    /// the team for you (or if only one team exists).</param>
    /// <returns>The galaxy you have connected to.</returns>
    public static async Task<Galaxy> Connect(string debugName, string uri, string? auth = null, string? team = null)
    {
        Uri parsedUri;
        ClientWebSocket? socket = null;

        if (auth is null)
            parsedUri = new Uri($"{uri}?version={Version}&auth=0000000000000000000000000000000000000000000000000000000000000000");
        else if (team is null)
            parsedUri = new Uri($"{uri}?version={Version}&auth={auth}");
        else
            parsedUri = new Uri($"{uri}?version={Version}&auth={auth}&team={team}");
        
        CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            socket = new ClientWebSocket();
            await socket.ConnectAsync(parsedUri, CancellationToken.None).ConfigureAwait(false);
        }
        catch (WebSocketException webSocketException)
        {
            // The .NET framework, or specifically the ClientWebSocket class, is very disappointing at this point:
            // It is not possible to request the HTTP body upon a rejection of the connection upgrade, nor to easily
            // and securely query the HTTP error code.

            socket?.Dispose();
            
            if (webSocketException.Message.Length < 37)
                throw new CantConnectGameException(
                    $"Error while connecting to the specified endpoint: {webSocketException.Message}", webSocketException);

            int httpCode;
            
            if (int.TryParse(webSocketException.Message.Substring(33, 3), NumberStyles.Integer, new CultureInfo("en-US"), out httpCode) && httpCode >= 100 && httpCode <= 999)
                switch (httpCode)
                {
                    case 502: 
                    case 504:
                        throw new CantConnectGameException(
                            "Error while connecting to the specified endpoint: The reverse proxy in front of the galaxy server couldn't reach the galaxy. (You are online, the galaxy is not.)", webSocketException);
                    default:
                        throw new CantConnectGameException(
                            $"The galaxy or some (reverse) proxy in between reported HTTP/{httpCode}.", webSocketException);
                }
                
            throw new CantConnectGameException(
                $"Error while connecting to the specified endpoint: {webSocketException.Message}", webSocketException);
        }
        catch (Exception exception)
        {
            socket?.Dispose();

            throw new CantConnectGameException(
                $"Error while connecting to the specified endpoint: {exception.Message}", exception);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
        
        Connection connection = new Connection(socket, 262144);

        Session session = connection.InitializeInitialSession();
        
        Galaxy galaxy = new Galaxy(connection);

        PacketReaderCopy packet = await session.GetReplyOrThrow();

        byte id;

        if (!packet.Read(out id))
        {
            connection.Close("Couldn't read player id from activation session.");
            throw new InvalidProtocolVersionGameException();
        }
        
        galaxy.SetupSelf(id);
        
        return galaxy;
    }

    private Galaxy(Connection connection)
    {
        _connection = connection;
        _connection.Disconnected += ConnectionDisconnected;
        
        _teams = new Team?[33];
        _clusters = new Cluster?[64];

        _teams[32] = new Team(32, "Spectators", 128, 128, 128);
        
        _players = new Player?[193];

        _events = new Queue<FlattiverseEvent>();
        _eventsSync = new object();
        
        Teams = new UniversalHolder<Team>(_teams);
        Clusters = new UniversalHolder<Cluster>(_clusters);
        Players = new UniversalHolder<Player>(_players);

        ThreadPool.QueueUserWorkItem(async delegate { await Receiver(); });
    }

    private void ConnectionDisconnected(string reason)
    {
        DeactivateAll();
        
        PushEvent(new ConnectionTerminatedEvent(reason));
    }
    
    internal bool TryGetTeam(byte id, [NotNullWhen(true)] out Team? team)
    {
        Debug.Assert(id < 33, "Invalid id for teams.");
        
        team = _teams[id];

        return team is not null;
    }
    
    /// <summary>
    /// Yourself.
    /// </summary>
    public Player Player => _player;
    
    /// <summary>
    /// The name of the galaxy.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// The description of the galaxy.
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// The game mode in effect of the galaxy.
    /// </summary>
    public GameMode GameMode => _gameMode;
    
    /// <summary>
    /// The maximum amount of players allowed to connect to the galaxy.
    /// </summary>
    public int MaxPlayers => _maxPlayers;
    
    /// <summary>
    /// The maximum amount of spectators allowed to connect to the galaxy. If this value is 0 no spectators are allowed.
    /// </summary>
    public int MaxSpectators => _maxSpectators;
    
    /// <summary>
    /// The maximum amount of toital ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxProbes => _galaxyMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxDrones => _galaxyMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxShips => _galaxyMaxNewShips;

    /// <summary>
    /// The maximum amount of bases allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxBases => _galaxyMaxBases;

    /// <summary>
    /// The maximum amount of total ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxProbes => _teamMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxDrones => _teamMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxShips => _teamMaxNewShips;

    /// <summary>
    /// The maximum amount of bases allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxBases => _teamMaxBases;

    /// <summary>
    /// The maximum amount of total ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxProbes => _playerMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxDrones => _playerMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxShips => _playerMaxNewShips;

    /// <summary>
    /// The maximum amount of bases allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxBases => _playerMaxBases;

    /// <summary>
    /// false, if you have been disconnected.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// true if a galaxy admin has enabled maintenance mode. When maintenance mode is enabled, new players or spectators
    /// cannot connect, and existing players cannot register new ships or continue existing ships. Some things in the
    /// galaxy (such as the game mode) can only be changed when maintenance mode is enabled, in order to maintain a
    /// consistent player state.
    /// </summary>
    public bool Maintenance => _maintenance;

    internal void PushEvent(FlattiverseEvent @event)
    {
        lock (_eventsSync)
        {
            _events.Enqueue(@event);

            if (_eventsTcs is not null)
            {
                _eventsTcs.SetResult();
                _eventsTcs = null;
            }
        }
    }

    /// <summary>
    /// Dequeues or waits for an event.
    /// </summary>
    /// <returns>The event.</returns>
    /// <exception cref="CantCallThisConcurrentGameException">Thrown, if you try to access this mehtod from different threads in parallel.</exception>
    /// <exception cref="ConnectionTerminatedGameException">Thrown, if the network connection has been terminated.</exception>
    public async ValueTask<FlattiverseEvent> NextEvent()
    {
        TaskCompletionSource tcs;

        FlattiverseEvent? @event;

        lock (_eventsSync)
            if (_events.TryDequeue(out @event))
                return @event;
            else if (_connection.Disposed)
                throw new ConnectionTerminatedGameException(_connection.CloseReason);
            else if (_eventsTcs is not null)
                throw new CantCallThisConcurrentGameException();
            else
            {
                tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                _eventsTcs = tcs;
            }

        await tcs.Task.ConfigureAwait(false);

        lock (_eventsSync)
            if (_events.TryDequeue(out @event))
                return @event;
            else if (_connection.Disposed)
                throw new ConnectionTerminatedGameException(_connection.CloseReason);

        throw new CantCallThisConcurrentGameException();
    }

    private async Task Receiver()
    {
        byte[] data = GC.AllocateUninitializedArray<byte>(16777216, false);
        PacketReader reader = new PacketReader(data);

        CommandRouter router = new CommandRouter(this);

        while (await _connection.TryRead(reader).ConfigureAwait(false))
            do
            {
                if (reader.Session > 0)
                    if (_connection.SessionReply(reader))
                        continue;
                    else
                    {
                        _connection.Close("Received reply to a session which doesn't exist.");
                        return;
                    }

                try
                {
                    if (!router.Call(reader))
                    {
                        _connection.Close($"Command not found: 0x{reader.Command:X02}.");
                        return;
                    }
                }
                catch (Exception exception)
                {
                    _connection.Close($"Error while executing command 0x{reader.Command:X02}: {exception.Message}");
                    return;
                }

            } while (reader.Next());
        
        _connection.Close("Read failed.");
    }

    private void DeactivateAll()
    {
        _active = false;

        foreach (Player? player in _players!)
            if (player is not null)
                player.Deactivate();

        foreach (Team? team in _teams!)
            if (team is not null)
                team.Deactivate();
        
        foreach (Cluster? cluster in _clusters!)
            if (cluster is not null)
                cluster.Deactivate();
    }
    
    internal void SetupSelf(byte id)
    {
        Debug.Assert(id < 193, "Id out of bounds.");
        Debug.Assert(_players[id] is not null, "Player at Id not setup.");
        
        _player = _players[id]!;
    }
    
    [Command(0x00)]
    private void PingPong(ushort challenge)
    {
        PacketWriter writer = new PacketWriter(new byte[2]);
        
        writer.Write(challenge);
        
        _connection.Send(writer);
        _connection.Flush();
    }
    
    [Command(0x01)]
    private void UpdateGalaxy(GameMode gameMode, string name, string description, byte maxPlayers, ushort maxSpectators,
        ushort galaxyMaxTotalShips, ushort galaxyMaxClassicShips, ushort galaxyMaxNewShips, ushort galaxyMaxBases,
        ushort teamMaxTotalShips, ushort teamMaxClassicShips, ushort teamMaxNewShips, ushort teamMaxBases,
        byte playerMaxTotalShips, byte playerMaxClassicShips, byte playerMaxNewShips, byte playerMaxBases)
    {
        _gameMode = gameMode;
        _name = name;
        _description = description;
        _maxPlayers = maxPlayers;
        _maxSpectators = maxSpectators;
        _galaxyMaxTotalShips = galaxyMaxTotalShips;
        _galaxyMaxClassicShips = galaxyMaxClassicShips;
        _galaxyMaxNewShips = galaxyMaxNewShips;
        _galaxyMaxBases = galaxyMaxBases;
        _teamMaxTotalShips = teamMaxTotalShips;
        _teamMaxClassicShips = teamMaxClassicShips;
        _teamMaxNewShips = teamMaxNewShips;
        _teamMaxBases = teamMaxBases;
        _playerMaxTotalShips = playerMaxTotalShips;
        _playerMaxClassicShips = playerMaxClassicShips;
        _playerMaxNewShips = playerMaxNewShips;
        _playerMaxBases = playerMaxBases;
    }

    [Command(0x02)]
    private void UpdateTeam(byte id, byte red, byte green, byte blue, string name)
    {
        Debug.Assert(id < 32, "Invalid team ID.");
        
        Team? team = _teams[id];
        
        if (team is null)
            _teams[id] = new Team(id, name, red, green, blue);
        else
            team.Update(name, red, green, blue);
    }
    
    [Command(0x03)]
    private void DeactivateTeam(byte id)
    {
        Debug.Assert(id < 32, "Invalid team ID.");
        
        Debug.Assert(_teams[id] is not null, "Team was already deactivated or did never exist.");
        
        _teams[id]!.Deactivate();
        _teams[id] = null;
    }
    
    [Command(0x06)]
    private void UpdateCluster(byte id, string name)
    {
        Debug.Assert(id < 64, "Invalid cluster ID.");
        
        Cluster? cluster = _clusters[id];
        
        if (cluster is null)
            _clusters[id] = new Cluster(id, name);
        else
            cluster.Update(name);
    }
    
    [Command(0x07)]
    private void DeactivateCluster(byte id)
    {
        Debug.Assert(id < 64, "Invalid cluster ID.");
        Debug.Assert(_clusters[id] is not null, "Cluster was already deactivated or did never exist.");
        
        _clusters[id]!.Deactivate();
        _clusters[id] = null;
    }
    
    [Command(0x10)]
    private void CreatePlayer(byte id, PlayerKind kind, Team team, string name, float ping)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is null, "Player does already exist.");

        _players[id] = new Player(id, kind, team, name, ping);
        
        PushEvent(new JoinedPlayerEvent(_players[id]!));
    }
    
    [Command(0x11)]
    private void UpdatePlayer(byte id, float ping)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is not null, "Player was already deactivated or did never exist.");

        _players[id]!.Update(ping);
    }
    
    [Command(0x13)]
    private void DeactivatePlayer(byte id)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is not null, "Player was already deactivated or did never exist.");

        _players[id]!.Deactivate();
        
        PushEvent(new PartedPlayerEvent(_players[id]!));
        
        _players[id] = null;
    }
    
    [Command(0xC0)]
    private void UniverseTick(int number)
    {
        PushEvent(new GalaxyTickEvent(number));
    }

    public void Dispose()
    {
        _connection.Close("You disposed the connection.");
        
        // Give all background processes enough time to update all states.
        Thread.Sleep(10);
    }
}