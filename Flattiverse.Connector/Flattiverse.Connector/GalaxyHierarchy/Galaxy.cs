using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.WebSockets;
using System.Reflection;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
                               // Disabled because fields are guaranteed to be setup after the class instance is
                               // returned via the static constructor. 

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Connected galaxy session with a local mirror of the current server state.
/// The instance owns the websocket connection, the event queue, and the owner-side and visible runtime snapshots.
/// </summary>
public partial class Galaxy : IDisposable
{
    private const string Version = "29";
    private const byte SpectatorsTeamId = 12;
    private const int TeamCapacity = 13;
    private const int ClusterCapacity = 24;
    
    private string _name;
    
    private GameMode _gameMode;
    private string _description;

    private byte _maxPlayers;
    private ushort _maxSpectators;
    
    private ushort _galaxyMaxTotalShips;
    private ushort _galaxyMaxClassicShips;
    private ushort _galaxyMaxModernShips;
    
    private ushort _teamMaxTotalShips;
    private ushort _teamMaxClassicShips;
    private ushort _teamMaxModernShips;
    
    private byte _playerMaxTotalShips;
    private byte _playerMaxClassicShips;
    private byte _playerMaxModernShips;

    private bool _maintenance;
    private bool _requiresSelfDisclosure;
    private string? _requiredAchievement;
    private bool _active;
    private bool _receivedCompiledWith;
    private bool _receivedGalaxySettings;
    private byte _compiledWithMaxPlayersSupported;
    private string _compiledWithSymbol;
    
    private readonly Team?[] _teams;
    private readonly Cluster?[] _clusters;
    
    private readonly Player?[] _players;
    private readonly Controllable?[] _controllables;
    
    internal readonly Connection Connection;

    private Player _player;

    private readonly Queue<FlattiverseEvent> _events;
    private TaskCompletionSource? _eventsTcs;
    private readonly object _eventsSync;
    private Crystal[] _crystals;

    /// <summary>
    /// Represents all teams in the galaxy.
    /// </summary>
    public readonly UniversalHolder<Team> Teams;
    
    /// <summary>
    /// Represents all clusters in the galaxy.
    /// </summary>
    public readonly UniversalHolder<Cluster> Clusters;
    
    /// <summary>
    /// Represents all players in the galaxy.
    /// </summary>
    public readonly UniversalHolder<Player> Players;
    
    /// <summary>
    /// Represents all units that you control.
    /// </summary>
    public readonly UniversalHolder<Controllable> Controllables;

    /// <summary>
    /// Represents the account-wide crystals last received from the server.
    /// </summary>
    public IReadOnlyList<Crystal> Crystals
    {
        get { return _crystals; }
    }
    
    /// <summary>
    /// Opens a websocket connection to a galaxy endpoint, completes the login handshake, and returns a ready-to-use
    /// local mirror of the current galaxy state.
    /// </summary>
    /// <param name="uri">
    /// Base websocket endpoint of the galaxy, for example <c>wss://www.flattiverse.com/galaxies/0/api</c>.
    /// The connector appends its protocol version and login query parameters automatically.
    /// </param>
    /// <param name="auth">
    /// API key for a player or admin login.
    /// Pass <see langword="null" /> to join as spectator; the connector then sends the protocol's special all-zero key.
    /// </param>
    /// <param name="team">
    /// Requested team name for a normal player login.
    /// Pass <see langword="null" /> to let the server choose a team automatically. Outside tournaments this usually
    /// means the non-spectator team with the fewest connected normal players. During tournaments the server may instead
    /// force the account's configured tournament team.
    /// </param>
    /// <param name="runtimeDisclosure">
    /// Optional runtime self-disclosure that is sent once during login.
    /// Some galaxies require this for regular player logins.
    /// </param>
    /// <param name="buildDisclosure">
    /// Optional build-assistance self-disclosure that is sent once during login.
    /// Some galaxies require this for regular player logins.
    /// </param>
    /// <returns>
    /// A fully initialized <see cref="Galaxy" /> instance.
    /// When this method returns, the initial galaxy, team, cluster, player, controllable, and visible-unit snapshots
    /// have already been processed, so properties such as <see cref="Player" />, <see cref="Teams" />,
    /// <see cref="Clusters" />, <see cref="Players" />, and <see cref="Controllables" /> can be used immediately.
    /// </returns>
    /// <remarks>
    /// This method does more than opening the socket: it waits until the server has delivered the initial state and the
    /// activation session reply. It therefore returns only after the connector is ready for normal event processing via
    /// <see cref="NextEvent" />.
    /// </remarks>
    /// <exception cref="CantConnectGameException">
    /// Thrown, if the websocket connection cannot be established or an HTTP proxy in front of the galaxy rejects the
    /// upgrade before protocol login completes.
    /// </exception>
    /// <exception cref="AuthFailedGameException">Thrown, if the supplied API key is missing, invalid, or unusable.</exception>
    /// <exception cref="TeamSelectionFailedGameException">
    /// Thrown, if the requested team does not exist or the server cannot choose a valid team automatically.
    /// </exception>
    /// <exception cref="TeamNotPlayableGameException">
    /// Thrown, if the requested team exists but is not playable for regular player logins.
    /// </exception>
    /// <exception cref="SelfDisclosureRequiredGameException">
    /// Thrown, if the galaxy requires self-disclosure for the chosen login kind and one or both disclosure values are
    /// missing.
    /// </exception>
    /// <exception cref="ServerFullOfPlayerKindGameException">
    /// Thrown, if the galaxy has no free slot for the requested player kind, for example normal player or spectator.
    /// </exception>
    /// <exception cref="PlayerAccessRestrictedGameException">
    /// Thrown, if a normal player login is denied by the galaxy player ACL.
    /// </exception>
    /// <exception cref="AdminAccessRestrictedGameException">
    /// Thrown, if an admin login is denied by the galaxy admin ACL.
    /// </exception>
    /// <exception cref="AccountAlreadyLoggedInGameException">
    /// Thrown, if the same account already has another active session on this or another galaxy.
    /// </exception>
    /// <exception cref="PersistenceUnavailableGameException">
    /// Thrown, if the login requires persistent account/session storage and that storage is currently unavailable.
    /// </exception>
    /// <exception cref="MissingAchievementGameException">
    /// Thrown, if a normal player login is denied because the galaxy requires an achievement the account does not have.
    /// </exception>
    /// <exception cref="GameException">
    /// Thrown, if the galaxy rejects the login with another protocol-level game exception.
    /// </exception>
    /// <exception cref="InvalidProtocolVersionGameException">
    /// Thrown, if the socket connection succeeds but the activation reply cannot be parsed by this connector version.
    /// </exception>
    public static async Task<Galaxy> Connect(string uri, string? auth = null, string? team = null,
        RuntimeDisclosure? runtimeDisclosure = null, BuildDisclosure? buildDisclosure = null)
    {
        Uri parsedUri;
        ClientWebSocket? socket = null;
        string authQuery;
        System.Text.StringBuilder queryBuilder = new System.Text.StringBuilder();

        if (auth is null)
            authQuery = "0000000000000000000000000000000000000000000000000000000000000000";
        else
            authQuery = auth;

        queryBuilder.Append(uri);
        queryBuilder.Append("?version=");
        queryBuilder.Append(Version);
        queryBuilder.Append("&auth=");
        queryBuilder.Append(Uri.EscapeDataString(authQuery));

        if (team is not null)
        {
            queryBuilder.Append("&team=");
            queryBuilder.Append(Uri.EscapeDataString(team));
        }

        if (runtimeDisclosure is not null)
        {
            queryBuilder.Append("&runtimeDisclosure=");
            queryBuilder.Append(runtimeDisclosure.ToString());
        }

        if (buildDisclosure is not null)
        {
            queryBuilder.Append("&buildDisclosure=");
            queryBuilder.Append(buildDisclosure.ToString());
        }

        parsedUri = new Uri(queryBuilder.ToString());
        
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
        Connection = connection;
        Connection.Disconnected += ConnectionDisconnected;
        _active = true;
        
        _teams = new Team?[TeamCapacity];
        _clusters = new Cluster?[ClusterCapacity];
        _controllables = new Controllable?[192];

        Team spectators = new Team(this, SpectatorsTeamId, "Spectators", 128, 128, 128, false);
        _teams[SpectatorsTeamId] = spectators;
        
        _players = new Player?[193];
        _compiledWithSymbol = string.Empty;

        _events = new Queue<FlattiverseEvent>();
        _eventsSync = new object();
        _crystals = Array.Empty<Crystal>();
        
        Teams = new UniversalHolder<Team>(_teams);
        Clusters = new UniversalHolder<Cluster>(_clusters);
        Players = new UniversalHolder<Player>(_players);
        Controllables = new UniversalHolder<Controllable>(_controllables);

        ThreadPool.QueueUserWorkItem(async delegate { await Receiver(); });
    }

    private void ConnectionDisconnected(string reason)
    {
        DeactivateAll();
        
        PushEvent(new ConnectionTerminatedEvent(reason));
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
    /// The expected amount of simulation ticks the galaxy advances per second.
    /// </summary>
    public int ExpectedTicksPerSecond => 10;
    
    /// <summary>
    /// The maximum amount of total ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxTotalShips => _galaxyMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxClassicShips => _galaxyMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed in the galaxy.
    /// </summary>
    public int GalaxyMaxModernShips => _galaxyMaxModernShips;

    /// <summary>
    /// The maximum amount of total ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxTotalShips => _teamMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxClassicShips => _teamMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed per team in the galaxy.
    /// </summary>
    public int TeamMaxModernShips => _teamMaxModernShips;

    /// <summary>
    /// The maximum amount of total ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxTotalShips => _playerMaxTotalShips;

    /// <summary>
    /// The maximum amount of classic style ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxClassicShips => _playerMaxClassicShips;

    /// <summary>
    /// The maximum amount of new style ships allowed per player in the galaxy.
    /// </summary>
    public int PlayerMaxModernShips => _playerMaxModernShips;

    /// <summary>
    /// True while the underlying session and connection are still active.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// True if a galaxy admin has enabled maintenance mode. When maintenance mode is enabled, new players or spectators
    /// cannot connect, and existing players cannot register new ships or continue existing ships. Some things in the
    /// galaxy (such as the game mode) can only be changed when maintenance mode is enabled, in order to maintain a
    /// consistent player state.
    /// </summary>
    public bool Maintenance => _maintenance;

    /// <summary>
    /// True if this galaxy requires self-disclosure for regular player logins.
    /// </summary>
    public bool RequiresSelfDisclosure => _requiresSelfDisclosure;

    /// <summary>
    /// Optional achievement key required for regular player logins.
    /// </summary>
    public string? RequiredAchievement => _requiredAchievement;

    /// <summary>
    /// The maximum amount of players this server binary has been compiled to support.
    /// </summary>
    public int CompiledWithMaxPlayersSupported => _compiledWithMaxPlayersSupported;

    /// <summary>
    /// The compile symbol that selected the server binary's player-capacity profile.
    /// </summary>
    public string CompiledWithSymbol => _compiledWithSymbol;

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
    /// <exception cref="CantCallThisConcurrentGameException">Thrown, if you try to access this method from different threads in parallel.</exception>
    /// <exception cref="ConnectionTerminatedGameException">Thrown, if the network connection has been terminated.</exception>
    public async ValueTask<FlattiverseEvent> NextEvent()
    {
        TaskCompletionSource tcs;

        FlattiverseEvent? @event;

        lock (_eventsSync)
            if (_events.TryDequeue(out @event))
                return @event;
            else if (Connection.Disposed)
                throw new ConnectionTerminatedGameException(Connection.CloseReason);
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
            else if (Connection.Disposed)
                throw new ConnectionTerminatedGameException(Connection.CloseReason);

        throw new CantCallThisConcurrentGameException();
    }

    private async Task Receiver()
    {
        byte[] data = GC.AllocateUninitializedArray<byte>(16777216, false);
        PacketReader reader = new PacketReader(data);
        string previousPacket = string.Empty;

        CommandRouter router = new CommandRouter(this);

        while (await Connection.TryRead(reader).ConfigureAwait(false))
            do
            {
                string currentPacket = reader.ToString();

                if (reader.Session > 0)
                    if (Connection.SessionReply(reader))
                    {
                        previousPacket = currentPacket;
                        continue;
                    }
                    else
                    {
                        Connection.Close(
                            $"Received reply to a session which doesn't exist: cmd=0x{reader.Command:X02}, sess=0x{reader.Session:X02}, size={reader.Size} | PreviousPacket={previousPacket} | CurrentPacket={currentPacket}");
                        return;
                    }

                try
                {
                    if (!router.Call(reader))
                    {
                        Connection.Close($"Command not found: 0x{reader.Command:X02} | PreviousPacket={previousPacket} | CurrentPacket={currentPacket}");
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Exception actualException = exception;

                    if (exception is TargetInvocationException targetInvocationException && targetInvocationException.InnerException is not null)
                        actualException = targetInvocationException.InnerException;

                    Connection.Close(
                        $"Error while executing command 0x{reader.Command:X02}: {actualException.GetType().Name}: {actualException.Message} | PreviousPacket={previousPacket} | CurrentPacket={currentPacket}");
                    return;
                }

                previousPacket = currentPacket;

            } while (reader.Next());
        
        Connection.Close("Read failed.");
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

    /// <summary>
    /// Configures galaxy metadata, teams and clusters from an XML document.
    /// Missing attributes keep old values for the referenced element.
    /// Team/Cluster elements define the final set: missing ids are removed.
    /// Unknown attributes and unknown child nodes are rejected by the server.
    /// </summary>
    /// <param name="xml">
    /// Configuration XML. Example:
    /// <code>
    /// &lt;Galaxy Name="New Name"&gt;
    ///   &lt;Team Id="0" /&gt;
    ///   &lt;Team Id="1" Name="Green" ColorR="64" ColorG="255" ColorB="64" Playable="true" /&gt;
    ///   &lt;Cluster Id="0" Name="Playground" Start="true" Respawn="false" /&gt;
    /// &lt;/Galaxy&gt;
    /// </code>
    /// Team id 12 (Spectators) must not be included.
    /// Team names must be unique.
    /// Removing a team fails if any remaining cluster still has regions referencing that team.
    /// Removing a cluster fails if any remaining unit still references that cluster, for example via a worm hole target.
    /// Galaxy/Team/Cluster names must be non-empty and at most 32 characters.
    /// Description must be at most 4096 characters.
    /// At least one cluster must end up with Start="true".
    /// </param>
    /// <exception cref="InvalidArgumentGameException">
    /// Thrown, if <paramref name="xml" /> is empty, unreadable on protocol level, or malformed XML.
    /// </exception>
    /// <exception cref="InvalidXmlNodeValueGameException">
    /// Thrown, if a specific XML node or attribute contains an invalid value.
    /// </exception>
    public async Task Configure(string xml)
    {
        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x04;
            writer.Write(xml);
        });
    }

    /// <summary>
    /// Send a chat message to the galaxy.
    /// </summary>
    /// <param name="message">The chat message to send.</param>
    public async Task Chat(string message)
    {
        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xC4;
            writer.Write(message);
        });
    }

    /// <summary>
    /// Creates a classic style ship.
    /// </summary>
    /// <returns>The controllable of the ship.</returns>
    public async Task<ClassicShipControllable> CreateClassicShip(string name)
    {
        return await CreateClassicShip(name, string.Empty, string.Empty, string.Empty).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a classic style ship with up to three equipped crystals.
    /// </summary>
    /// <returns>The controllable of the ship.</returns>
    public async Task<ClassicShipControllable> CreateClassicShip(string name, string crystal0Name, string crystal1Name,
        string crystal2Name)
    {
        if (!Utils.CheckName(name))
            throw new InvalidArgumentGameException(InvalidArgumentKind.NameConstraint, "name");

        PacketReaderCopy result = await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x80;
            writer.Write(name);
            writer.Write(crystal0Name);
            writer.Write(crystal1Name);
            writer.Write(crystal2Name);
        });
        
        if (result.Read(out byte id) && _controllables[id] is ClassicShipControllable controllable)
            return controllable;
        
        throw new InvalidDataException("This shouldn't have happened: Couldn't read controllable id or there was no proper controllable.");
    }

    /// <summary>
    /// Creates a modern style ship.
    /// </summary>
    /// <returns>The controllable of the ship.</returns>
    public async Task<ModernShipControllable> CreateModernShip(string name)
    {
        return await CreateModernShip(name, string.Empty, string.Empty, string.Empty).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a modern style ship with up to three equipped crystals.
    /// </summary>
    /// <returns>The controllable of the ship.</returns>
    public async Task<ModernShipControllable> CreateModernShip(string name, string crystal0Name, string crystal1Name,
        string crystal2Name)
    {
        if (!Utils.CheckName(name))
            throw new InvalidArgumentGameException(InvalidArgumentKind.NameConstraint, "name");

        PacketReaderCopy result = await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x81;
            writer.Write(name);
            writer.Write(crystal0Name);
            writer.Write(crystal1Name);
            writer.Write(crystal2Name);
        });

        if (result.Read(out byte id) && _controllables[id] is ModernShipControllable controllable)
            return controllable;

        throw new InvalidDataException("This shouldn't have happened: Couldn't read controllable id or there was no proper controllable.");
    }

    /// <summary>
    /// Requests the current account-wide crystal snapshot.
    /// </summary>
    public async Task<IReadOnlyList<Crystal>> RequestCrystals()
    {
        PacketReaderLarge result = await Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA0;
        }).ConfigureAwait(false);

        _crystals = ReadCrystalsSnapshot(ref result);
        return _crystals;
    }

    /// <summary>
    /// Produces a crystal from nebula cargo.
    /// </summary>
    /// <returns><c>true</c> if a crystal was created; <c>false</c> if the nebula faded.</returns>
    public async Task<bool> ProduceCrystal(ClassicShipControllable controllable, string name)
    {
        PacketReaderLarge result = await Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x9D;
            writer.Write(controllable.Id);
            writer.Write(name);
        }).ConfigureAwait(false);

        if (!result.Read(out byte produced))
            throw new InvalidDataException("Couldn't read crystal creation result.");

        _crystals = ReadCrystalsSnapshot(ref result);
        return produced != 0x00;
    }

    /// <summary>
    /// Renames an account-wide crystal.
    /// </summary>
    public async Task<IReadOnlyList<Crystal>> RenameCrystal(string oldName, string newName)
    {
        PacketReaderLarge result = await Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x9E;
            writer.Write(oldName);
            writer.Write(newName);
        }).ConfigureAwait(false);

        _crystals = ReadCrystalsSnapshot(ref result);
        return _crystals;
    }

    /// <summary>
    /// Destroys an account-wide crystal.
    /// </summary>
    public async Task<IReadOnlyList<Crystal>> DestroyCrystal(string name)
    {
        PacketReaderLarge result = await Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x9F;
            writer.Write(name);
        }).ConfigureAwait(false);

        _crystals = ReadCrystalsSnapshot(ref result);
        return _crystals;
    }

    private static Crystal[] ReadCrystalsSnapshot(ref PacketReaderLarge reader)
    {
        if (!reader.Read(out byte count))
            throw new InvalidDataException("Couldn't read crystal count.");

        Crystal[] crystals = new Crystal[count];

        for (int index = 0; index < crystals.Length; index++)
        {
            if (!reader.Read(out string? name) ||
                !reader.Read(out float hue) ||
                !reader.Read(out byte gradeValue) ||
                !reader.Read(out float energyBatteryMultiplier) ||
                !reader.Read(out float ionsBatteryMultiplier) ||
                !reader.Read(out float neutrinosBatteryMultiplier) ||
                !reader.Read(out float hullMultiplier) ||
                !reader.Read(out float shieldMultiplier) ||
                !reader.Read(out float armorMultiplier) ||
                !reader.Read(out float energyCellMultiplier) ||
                !reader.Read(out float ionsCellMultiplier) ||
                !reader.Read(out float neutrinosCellMultiplier) ||
                !reader.Read(out float shotWeaponProductionMultiplier) ||
                !reader.Read(out float interceptorWeaponProductionMultiplier) ||
                !reader.Read(out float crystalCargoLimitMultiplier) ||
                !reader.Read(out byte locked))
                throw new InvalidDataException("Couldn't read crystal snapshot entry.");

            if (name is null)
                throw new InvalidDataException("Server did send a null crystal name.");

            CrystalGrade grade = gradeValue switch
            {
                0x00 => CrystalGrade.LowGrade,
                0x01 => CrystalGrade.Regular,
                0x02 => CrystalGrade.Pure,
                0x03 => CrystalGrade.Mastery,
                0x04 => CrystalGrade.Divine,
                _ => throw new InvalidDataException("Server did send an unknown crystal grade.")
            };

            crystals[index] = new Crystal(name, hue, grade, energyBatteryMultiplier, ionsBatteryMultiplier,
                neutrinosBatteryMultiplier, hullMultiplier, shieldMultiplier, armorMultiplier, energyCellMultiplier,
                ionsCellMultiplier, neutrinosCellMultiplier, shotWeaponProductionMultiplier,
                interceptorWeaponProductionMultiplier, crystalCargoLimitMultiplier, locked != 0x00);
        }

        return crystals;
    }
    
    [Command(0x00)]
    private void PingReply(ushort challenge)
    {
        if (!_active || Connection.Disposed)
            return;

        Connection.Send(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x00;
            writer.Write(challenge);
        });
        Connection.Flush();
    }
    
    [Command(0x01)]
    private void UpdateGalaxy(GameMode gameMode, string name, string description, byte maxPlayers, ushort maxSpectators,
        ushort galaxyMaxTotalShips, ushort galaxyMaxClassicShips, ushort galaxyMaxModernShips,
        ushort teamMaxTotalShips, ushort teamMaxClassicShips, ushort teamMaxModernShips,
        byte playerMaxTotalShips, byte playerMaxClassicShips, byte playerMaxModernShips, byte maintenance,
        byte requiresSelfDisclosure, string requiredAchievement)
    {
        GalaxySettingsSnapshot? oldSettings;

        if (_receivedGalaxySettings)
            oldSettings = new GalaxySettingsSnapshot(_gameMode, _name, _description, _maxPlayers, _maxSpectators,
                _galaxyMaxTotalShips, _galaxyMaxClassicShips, _galaxyMaxModernShips,
                _teamMaxTotalShips, _teamMaxClassicShips, _teamMaxModernShips,
                _playerMaxTotalShips, _playerMaxClassicShips, _playerMaxModernShips, _maintenance,
                _requiresSelfDisclosure, _requiredAchievement);
        else
            oldSettings = null;

        _gameMode = gameMode;
        _name = name;
        _description = description;
        _maxPlayers = maxPlayers;
        _maxSpectators = maxSpectators;
        _galaxyMaxTotalShips = galaxyMaxTotalShips;
        _galaxyMaxClassicShips = galaxyMaxClassicShips;
        _galaxyMaxModernShips = galaxyMaxModernShips;
        _teamMaxTotalShips = teamMaxTotalShips;
        _teamMaxClassicShips = teamMaxClassicShips;
        _teamMaxModernShips = teamMaxModernShips;
        _playerMaxTotalShips = playerMaxTotalShips;
        _playerMaxClassicShips = playerMaxClassicShips;
        _playerMaxModernShips = playerMaxModernShips;
        _maintenance = maintenance != 0;
        _requiresSelfDisclosure = requiresSelfDisclosure != 0;
        _requiredAchievement = string.IsNullOrEmpty(requiredAchievement) ? null : requiredAchievement;

        _receivedGalaxySettings = true;

        GalaxySettingsSnapshot newSettings = new GalaxySettingsSnapshot(_gameMode, _name, _description, _maxPlayers, _maxSpectators,
            _galaxyMaxTotalShips, _galaxyMaxClassicShips, _galaxyMaxModernShips,
            _teamMaxTotalShips, _teamMaxClassicShips, _teamMaxModernShips,
            _playerMaxTotalShips, _playerMaxClassicShips, _playerMaxModernShips, _maintenance,
            _requiresSelfDisclosure, _requiredAchievement);

        PushEvent(new UpdatedGalaxySettingsEvent(oldSettings, newSettings));
    }

    [Command(0x02)]
    private void UpdateTeam(byte id, byte red, byte green, byte blue, byte playable, string name)
    {
        Debug.Assert(id < SpectatorsTeamId, "Invalid team ID.");
        
        Team? team = _teams[id];
        
        if (team is null)
        {
            Team newTeam = new Team(this, id, name, red, green, blue, playable != 0x00);
            _teams[id] = newTeam;
            PushEvent(new CreatedTeamEvent(CreateTeamSnapshot(newTeam)));
        }
        else
        {
            TeamSnapshot oldTeam = CreateTeamSnapshot(team);
            team.Update(name, red, green, blue, playable != 0x00);
            TeamSnapshot newTeam = CreateTeamSnapshot(team);
            PushEvent(new UpdatedTeamEvent(oldTeam, newTeam));
        }
    }

    [Command(0x04)]
    private void UpdateTeamScore(Team team, uint playerKills, uint playerDeaths, uint friendlyKills, uint friendlyDeaths, uint npcKills,
        uint npcDeaths, uint neutralDeaths, int mission)
    {
        uint oldPlayerKills = team.Score.PlayerKills;
        uint oldPlayerDeaths = team.Score.PlayerDeaths;
        uint oldFriendlyKills = team.Score.FriendlyKills;
        uint oldFriendlyDeaths = team.Score.FriendlyDeaths;
        uint oldNpcKills = team.Score.NpcKills;
        uint oldNpcDeaths = team.Score.NpcDeaths;
        uint oldNeutralDeaths = team.Score.NeutralDeaths;
        int oldMission = team.Score.Mission;

        team.Score.Update(playerKills, playerDeaths, friendlyKills, friendlyDeaths, npcKills, npcDeaths, neutralDeaths, mission);

        PushEvent(new UpdatedTeamScoreEvent(team, oldPlayerKills, oldPlayerDeaths, oldFriendlyKills, oldFriendlyDeaths, oldNpcKills,
            oldNpcDeaths, oldNeutralDeaths, oldMission, team.Score.PlayerKills, team.Score.PlayerDeaths, team.Score.FriendlyKills,
            team.Score.FriendlyDeaths, team.Score.NpcKills, team.Score.NpcDeaths, team.Score.NeutralDeaths, team.Score.Mission));
    }
    
    [Command(0x03)]
    private void DeactivateTeam(byte id)
    {
        Debug.Assert(id < SpectatorsTeamId, "Invalid team ID.");
        
        Debug.Assert(_teams[id] is not null, "Team was already deactivated or did never exist.");

        Team team = _teams[id]!;
        TeamSnapshot removedTeam = CreateTeamSnapshot(team);

        team.Deactivate();
        _teams[id] = null;

        PushEvent(new RemovedTeamEvent(removedTeam));
    }
    
    [Command(0x06)]
    private void UpdateCluster(byte id, string name, byte flags)
    {
        Debug.Assert(id < ClusterCapacity, "Invalid cluster ID.");

        bool start = (flags & 0x01) != 0;
        bool respawn = (flags & 0x02) != 0;
        
        Cluster? cluster = _clusters[id];
        
        if (cluster is null)
        {
            Cluster newCluster = new Cluster(this, id, name, start, respawn);
            _clusters[id] = newCluster;
            PushEvent(new CreatedClusterEvent(CreateClusterSnapshot(newCluster)));
        }
        else
        {
            ClusterSnapshot oldCluster = CreateClusterSnapshot(cluster);
            cluster.Update(name, start, respawn);
            ClusterSnapshot newCluster = CreateClusterSnapshot(cluster);
            PushEvent(new UpdatedClusterEvent(oldCluster, newCluster));
        }
    }

    private static TeamSnapshot CreateTeamSnapshot(Team team)
    {
        return new TeamSnapshot(team.Id, team.Name, team.Red, team.Green, team.Blue, team.Playable);
    }
    
    [Command(0x07)]
    private void DeactivateCluster(byte id)
    {
        Debug.Assert(id < ClusterCapacity, "Invalid cluster ID.");
        Debug.Assert(_clusters[id] is not null, "Cluster was already deactivated or did never exist.");

        Cluster cluster = _clusters[id]!;
        cluster.Deactivate();

        ClusterSnapshot removedCluster = CreateClusterSnapshot(cluster);

        _clusters[id] = null;

        PushEvent(new RemovedClusterEvent(removedCluster));
    }

    private static ClusterSnapshot CreateClusterSnapshot(Cluster cluster)
    {
        return new ClusterSnapshot(cluster.Id, cluster.Name, cluster.Active, cluster.Start, cluster.Respawn);
    }
    
    [Command(0x10)]
    private void CreatePlayer(byte id, PlayerKind kind, Team team, string name, float ping, byte admin, byte stateFlags, int rank,
        long playerKills, long playerDeaths, long friendlyKills, long friendlyDeaths, long npcKills, long npcDeaths,
        long neutralDeaths, byte hasAvatar, PacketReader reader)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is null, "Player does already exist.");

        RuntimeDisclosure? runtimeDisclosure = null;
        BuildDisclosure? buildDisclosure = null;

        if (!reader.Read(out byte disclosureFlags))
            throw new InvalidDataException("Player create packet is missing disclosure flags.");

        if ((disclosureFlags & 0x01) != 0 && !RuntimeDisclosure.TryRead(reader, out runtimeDisclosure))
            throw new InvalidDataException("Player create packet contains malformed runtime disclosure.");

        if ((disclosureFlags & 0x02) != 0 && !BuildDisclosure.TryRead(reader, out buildDisclosure))
            throw new InvalidDataException("Player create packet contains malformed build disclosure.");

        bool disconnected = (stateFlags & 0x01) != 0;

        _players[id] = new Player(this, id, kind, team, name, ping, admin != 0x00, disconnected, rank, playerKills, playerDeaths,
            friendlyKills, friendlyDeaths, npcKills, npcDeaths, neutralDeaths, hasAvatar != 0x00, runtimeDisclosure, buildDisclosure);
        
        PushEvent(new JoinedPlayerEvent(_players[id]!));

        if (_players[id]!.Disconnected)
            PushEvent(new DisconnectedPlayerEvent(_players[id]!));
    }
    
    [Command(0x11)]
    private void UpdatePlayer(byte id, float ping, byte admin, byte stateFlags, int rank, long playerKills, long playerDeaths,
        long friendlyKills, long friendlyDeaths, long npcKills, long npcDeaths, long neutralDeaths)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is not null, "Player was already deactivated or did never exist.");

        Player player = _players[id]!;
        bool wasDisconnected = player.Disconnected;

        player.Update(ping, admin != 0x00, (stateFlags & 0x01) != 0, rank, playerKills, playerDeaths, friendlyKills, friendlyDeaths,
            npcKills, npcDeaths, neutralDeaths);

        if (!wasDisconnected && player.Disconnected)
            PushEvent(new DisconnectedPlayerEvent(player));
    }

    [Command(0x12)]
    private void UpdatePlayerScore(byte id, uint playerKills, uint playerDeaths, uint friendlyKills, uint friendlyDeaths, uint npcKills,
        uint npcDeaths, uint neutralDeaths, int mission)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is not null, "Player was already deactivated or did never exist.");

        Player player = _players[id]!;
        uint oldPlayerKills = player.Score.PlayerKills;
        uint oldPlayerDeaths = player.Score.PlayerDeaths;
        uint oldFriendlyKills = player.Score.FriendlyKills;
        uint oldFriendlyDeaths = player.Score.FriendlyDeaths;
        uint oldNpcKills = player.Score.NpcKills;
        uint oldNpcDeaths = player.Score.NpcDeaths;
        uint oldNeutralDeaths = player.Score.NeutralDeaths;
        int oldMission = player.Score.Mission;

        player.Score.Update(playerKills, playerDeaths, friendlyKills, friendlyDeaths, npcKills, npcDeaths, neutralDeaths, mission);

        PushEvent(new UpdatedPlayerScoreEvent(player, oldPlayerKills, oldPlayerDeaths, oldFriendlyKills, oldFriendlyDeaths, oldNpcKills,
            oldNpcDeaths, oldNeutralDeaths, oldMission, player.Score.PlayerKills, player.Score.PlayerDeaths, player.Score.FriendlyKills,
            player.Score.FriendlyDeaths, player.Score.NpcKills, player.Score.NpcDeaths, player.Score.NeutralDeaths, player.Score.Mission));
    }
    
    [Command(0x1F)]
    private void DeactivatePlayer(byte id)
    {
        Debug.Assert(id < 193, "Invalid player ID.");
        Debug.Assert(_players[id] is not null, "Player was already deactivated or did never exist.");

        _players[id]!.Deactivate();
        
        PushEvent(new PartedPlayerEvent(_players[id]!));
        
        _players[id] = null;
    }

    [Command(0x20)]
    private void ControllableInfoNew(Player player, UnitKind kind, byte id, string name, byte alive)
    {
        if (ControllableInfo.New(kind, player, id, name, alive == 0x01, out ControllableInfo? info))
        {
            player._controllableInfos[id] = info;
            
            PushEvent(new RegisteredControllableInfoEvent(player, info));
            
            return;
        }

        throw new InvalidDataException("Server did send invalid data.");
    }
    
    [Command(0x21)]
    private void ControllableInfoAlive(Player player, byte id)
    {
        ControllableInfo? controllable = player._controllableInfos[id];

        if (controllable is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo.");

        controllable.SetAlive();
    }
    
    [Command(0x22)]
    private void ControllableInfoDeadByReason(Player player, byte id, PlayerUnitDestroyedReason reason)
    {
        ControllableInfo? controllable = player._controllableInfos[id];

        if (controllable is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo.");

        controllable.SetDead(reason);
    }

    [Command(0x23)]
    private void ControllableInfoDeadByNeutralCollision(Player player, byte id, UnitKind unitKind, string name)
    {
        ControllableInfo? controllable = player._controllableInfos[id];

        if (controllable is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo.");

        controllable.SetDeadByNeutralCollision(unitKind, name);
    }

    [Command(0x24)]
    private void ControllableInfoDeadByPlayerUnit(Player player, byte id, PlayerUnitDestroyedReason reason, Player causer, byte causerControllableId)
    {
        ControllableInfo? controllable = player._controllableInfos[id];
        ControllableInfo? causerControllable = causer._controllableInfos[causerControllableId];

        if (controllable is null || causerControllable is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo.");

        controllable.SetDeadByPlayerShip(reason, causerControllable);
    }

    [Command(0x25)]
    private void ControllableInfoScoreUpdated(Player player, byte id, uint playerKills, uint playerDeaths, uint friendlyKills,
        uint friendlyDeaths, uint npcKills, uint npcDeaths, uint neutralDeaths, int mission)
    {
        ControllableInfo? controllable = player._controllableInfos[id];

        if (controllable is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo.");

        uint oldPlayerKills = controllable.Score.PlayerKills;
        uint oldPlayerDeaths = controllable.Score.PlayerDeaths;
        uint oldFriendlyKills = controllable.Score.FriendlyKills;
        uint oldFriendlyDeaths = controllable.Score.FriendlyDeaths;
        uint oldNpcKills = controllable.Score.NpcKills;
        uint oldNpcDeaths = controllable.Score.NpcDeaths;
        uint oldNeutralDeaths = controllable.Score.NeutralDeaths;
        int oldMission = controllable.Score.Mission;

        controllable.Score.Update(playerKills, playerDeaths, friendlyKills, friendlyDeaths, npcKills, npcDeaths, neutralDeaths, mission);

        PushEvent(new UpdatedControllableInfoScoreEvent(player, controllable, oldPlayerKills, oldPlayerDeaths, oldFriendlyKills,
            oldFriendlyDeaths, oldNpcKills, oldNpcDeaths, oldNeutralDeaths, oldMission, controllable.Score.PlayerKills,
            controllable.Score.PlayerDeaths, controllable.Score.FriendlyKills, controllable.Score.FriendlyDeaths,
            controllable.Score.NpcKills, controllable.Score.NpcDeaths, controllable.Score.NeutralDeaths, controllable.Score.Mission));
    }

    [Command(0x2F)]
    private void ControllableInfoRemoved(Player player, byte id)
    {
        ControllableInfo? controllableInfo = player._controllableInfos[id];

        if (controllableInfo is not null)
        {
            controllableInfo.Deactivate();
            
            PushEvent(new ClosedControllableInfoEvent(player, controllableInfo));
            player._controllableInfos[id] = null;
        }
        else
            Debug.Fail("Removed non existent controllable info.");
    }
    
    [Command(0x80)]
    private void ControllableNew(UnitKind kind, byte id, Cluster cluster, string name, PacketReader reader)
    {
        if (_controllables[id] is Controllable oldControllable)
            oldControllable.Deactivate();

        if (Controllable.New(kind, cluster, id, name, reader, out Controllable? info))
        {
            _controllables[id] = info;
            return;
        }

        throw new InvalidDataException("Server did send invalid data.");
    }
    
    [Command(0x81)]
    private void ControllableDeceased(Controllable controllable)
    {
        controllable.Deceased();
    }    
    
    [Command(0x82)]
    private void ControllableUpdated(Controllable controllable, Cluster cluster, PacketReader reader)
    {
        controllable.Updated(cluster, reader);
    }
    
    [Command(0x8F)]
    private void ControllableRemoved(Controllable controllable)
    {
        controllable.Deactivate();
        _controllables[controllable.Id] = null;
    }

    [Command(0x8E)]
    private void PowerUpCollected(Controllable controllable, UnitKind powerUpKind, string powerUpName, float amount, float appliedAmount)
    {
        PushEvent(new CollectedPowerUpEvent(controllable, powerUpKind, powerUpName, amount, appliedAmount));
    }
    
    [Command(0x30)]
    private void UnitNew(Cluster cluster, string name, UnitKind kind, PacketReader reader)
    {
        try
        {
            Unit? unit;

            if (!Unit.TryReadUnit(kind, cluster, name, reader, out unit))
                throw new InvalidDataException("Couldn't parse unit.");
        
            cluster.AddUnit(unit);
        
            PushEvent(new AppearedUnitEvent(unit));
        }
        catch (Exception exception)
        {
            throw new InvalidDataException(
                $"Couldn't process new unit \"{name}\" of kind {kind} in cluster \"{cluster.Name}\": {exception.Message}", exception);
        }
    }
       
    [Command(0x31)]
    private void UnitUpdatedMovement(Cluster cluster, string name, PacketReader reader)
    {
        if (cluster.UpdateUnit(name, reader, out Unit? unit))
            PushEvent(new UpdatedUnitEvent(unit));
    }

    [Command(0x32)]
    private void UnitUpdatedState(Cluster cluster, string name, PacketReader reader)
    {
        if (cluster.UpdateUnitState(name, reader, out Unit? unit))
            PushEvent(new UpdatedUnitEvent(unit));
    }

    [Command(0x3E)]
    private void UnitAlteredByAdmin(byte clusterId, string name)
    {
        PushEvent(new AlteredUnitByAdminEvent(clusterId, name));
    }
    
    [Command(0x3F)]
    private void UnitRemoved(Cluster cluster, string name)
    {
        if (cluster.RemoveUnit(name, out Unit? unit))
            PushEvent(new RemovedUnitEvent(unit));
    }

    [Command(0x0B)]
    private void CompiledWith(byte maxPlayersSupported, string symbol)
    {
        Debug.Assert(!_receivedCompiledWith || (_compiledWithMaxPlayersSupported == maxPlayersSupported && _compiledWithSymbol == symbol),
            "Received conflicting compile-configuration packets.");

        if (_receivedCompiledWith)
            return;

        _compiledWithMaxPlayersSupported = maxPlayersSupported;
        _compiledWithSymbol = symbol;
        _receivedCompiledWith = true;

        PushEvent(new CompiledWithMessageEvent(maxPlayersSupported, symbol));
    }
    
    [Command(0xC0)]
    private void UniverseTick(uint number, float scanMs, float steadyMs, float gravityMs, float enginesMs, float limitMs,
        float movementMs, float collisionsMs, float actionsMs, float visibilityMs, float totalMs, int remainingStaticSegments)
    {
        PushEvent(new GalaxyTickEvent(number, scanMs, steadyMs, gravityMs, enginesMs, limitMs, movementMs, collisionsMs,
            actionsMs, visibilityMs, totalMs, remainingStaticSegments));
    }

    [Command(0xC1)]
    private void FlagScoredChat(Player player, byte controllableId, Team flagTeam, string flagName)
    {
        ControllableInfo? controllableInfo = player._controllableInfos[controllableId];

        if (controllableInfo is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo in flag score chat.");

        PushEvent(new FlagScoredChatEvent(player, controllableInfo, flagTeam, flagName));
    }

    [Command(0xC2)]
    private void DominationPointScoredChat(Team team, string dominationPointName)
    {
        PushEvent(new DominationPointScoredChatEvent(team, dominationPointName));
    }

    [Command(0xC3)]
    private void OwnFlagHitChat(Player player, byte controllableId, Team flagTeam, string flagName)
    {
        ControllableInfo? controllableInfo = player._controllableInfos[controllableId];

        if (controllableInfo is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo in own-flag-hit chat.");

        PushEvent(new OwnFlagHitChatEvent(player, controllableInfo, flagTeam, flagName));
    }
    
    [Command(0xC4)]
    private void GalaxyChat(Player player, string message)
    {
        PushEvent(new GalaxyChatEvent(player, message, this));
    }
    
    [Command(0xC5)]
    private void TeamChat(Player player, string message)
    {
        PushEvent(new TeamChatEvent(player, message, this.Player.Team));
    }
    
    [Command(0xC6)]
    private void PlayerChat(Player player, string message)
    {
        PushEvent(new PlayerChatEvent(player, message, this.Player));
    }

    [Command(0xC7)]
    private void MissionTargetHitChat(Player player, byte controllableId, ushort missionTargetSequence)
    {
        ControllableInfo? controllableInfo = player._controllableInfos[controllableId];

        if (controllableInfo is null)
            throw new InvalidDataException("Server did send a non existent ControllableInfo in mission-target-hit chat.");

        PushEvent(new MissionTargetHitChatEvent(player, controllableInfo, missionTargetSequence));
    }

    [Command(0xC8)]
    private void SystemMessage(string message)
    {
        PushEvent(new SystemMessageEvent(message));
    }

    [Command(0xC9)]
    private void FlagReactivatedChat(Team flagTeam, string flagName)
    {
        PushEvent(new FlagReactivatedChatEvent(flagTeam, flagName));
    }

    [Command(0xCA)]
    private void GateSwitched(Cluster cluster, PacketReader reader)
    {
        if (!reader.Read(out byte hasInvoker))
            throw new InvalidDataException("Couldn't read gate switch event header.");

        Player? invokerPlayer = null;
        ControllableInfo? invokerControllableInfo = null;

        if (hasInvoker != 0x00)
        {
            if (!reader.Read(out byte playerId))
                throw new InvalidDataException("Server did send an incomplete player id in gate switch event.");

            invokerPlayer = _players[playerId];

            if (invokerPlayer is null)
                throw new InvalidDataException("Server did send an unknown player in gate switch event.");

            if (!reader.Read(out byte controllableId))
                throw new InvalidDataException("Server did send an incomplete gate switch controllable id.");

            invokerControllableInfo = invokerPlayer._controllableInfos[controllableId];

            if (invokerControllableInfo is null)
                throw new InvalidDataException("Server did send a non existent ControllableInfo in gate switch event.");
        }

        if (!reader.Read(out string switchName) || !reader.Read(out ushort gateCount))
            throw new InvalidDataException("Couldn't read gate switch event payload.");

        GateStateChange[] gates = new GateStateChange[gateCount];

        for (int index = 0; index < gateCount; index++)
        {
            if (!reader.Read(out string gateName) || !reader.Read(out byte closed))
                throw new InvalidDataException("Couldn't read gate switch state entry.");

            gates[index] = new GateStateChange(gateName, closed != 0x00);
        }

        PushEvent(new SwitchedGateEvent(cluster, switchName, invokerPlayer, invokerControllableInfo, gates));
    }

    [Command(0xCB)]
    private void GateRestored(Cluster cluster, string gateName, byte closed)
    {
        PushEvent(new RestoredGateEvent(cluster, gateName, closed != 0x00));
    }

    [Command(0xCE)]
    private void MotdMessage(string message)
    {
        PushEvent(new MotdMessageEvent(message));
    }

    [Command(0xCC)]
    private void PlayerBinaryChat(Player player, PacketReader reader)
    {
        if (!reader.Read(out ushort messageLength))
            throw new InvalidDataException("Server did send a binary player chat without a message length.");

        if (messageLength == 0 || messageLength > 1024)
            throw new InvalidDataException($"Server did send a binary player chat with invalid length {messageLength}.");

        byte[] message = GC.AllocateUninitializedArray<byte>(messageLength, false);

        if (!reader.Read(message))
            throw new InvalidDataException("Server did send a truncated binary player chat payload.");

        PushEvent(new PlayerBinaryChatEvent(player, message, this.Player));
    }

    /// <summary>
    /// Closes the galaxy connection and gives the background receive loop a short moment to settle.
    /// </summary>
    public void Dispose()
    {
        Connection.Close("You disposed the connection.");
        
        // Give all background processes enough time to update all states.
        Thread.Sleep(10);
    }
}
