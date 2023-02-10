using Flattiverse.Connector.Accounts;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Flattiverse.Connector
{
    /// <summary>
    /// Represents the main class. The UniverseGroup you connect to is managed by an instance of this class.
    /// </summary>
    public class UniverseGroup : IDisposable
    {
        internal Connection connection;

        // players[0-63] are real players, players[64] is a substitute, if the server treats us as non player, like a spectator or admin.
        internal readonly Player[] players = new Player[65];

        private Player player;

        internal string name = "Unknown";
        internal string description = "Unknown";
        internal GameMode mode;

        internal int maxPlayers;
        internal int maxShipsPerPlayer;
        internal int maxShipsPerTeam;
        internal int maxBasesPerPlayer;
        internal int maxBasesPerTeam;
        internal bool spectators;
        internal int registerShipLimit;

        internal Team[] teams = new Team[16];
        internal Universe[] universes = new Universe[64];
        internal Controllable[] controllables = new Controllable[32];
        internal Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath> systems = new Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath>();

        private readonly object syncControllables = new object();

        /// <summary>
        /// Connects to the specific UniverseGroup.
        /// </summary>
        /// <param name="uri">The URI of the Universegroup.</param>
        /// <param name="auth">The auth key for UniverseGroup access.</param>
        public UniverseGroup(string uri, string auth)
        {
            connection = new Connection(this, uri, auth);

            using (Query query = connection.Query("whoami"))
            {
                query.Send().GetAwaiter().GetResult();

                player = players[query.ReceiveInteger().GetAwaiter().GetResult()];
            }
        }

        public UniverseGroup(string uri, string auth, string team)
        {
            connection = new Connection(this, uri, auth, team);

            using (Query query = connection.Query("whoami"))
            {
                query.Send().GetAwaiter().GetResult();

                player = players[query.ReceiveInteger().GetAwaiter().GetResult()];
            }
        }

#pragma warning disable CS8618 // Wird versprochenerweise direkt initialisiert, bevor ein Benutzer etwas damit machen kann. :D Da es aber eine API ist wollen wir dem Endnutzer die beste Erfahrung geben. :)
        private UniverseGroup()
        {
        }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        internal void setupPlayer(Connection connection, int playerId)
        {
            this.connection = connection;
            player = players[playerId];
        }

        /// <summary>
        /// Connects to the specific UniverseGroup asynchonously.
        /// </summary>
        /// <param name="uri">The URI of the Universegroup.</param>
        /// <param name="auth">The auth key for UniverseGroup access.</param>
        /// <returns>The connected UniverseGroup.</returns>
        public static async Task<UniverseGroup> NewAsyncUniverseGroup(string uri, string auth)
        {
            UniverseGroup universeGroup = new UniverseGroup();
            Connection connection = await Connection.NewAsyncConnection(universeGroup, uri, auth).ConfigureAwait(false);

            using (Query query = connection.Query("whoami"))
            {
                await query.Send().ConfigureAwait(false);

                universeGroup.setupPlayer(connection, await query.ReceiveInteger().ConfigureAwait(false));
            }

            return universeGroup;
        }

        public static async Task<UniverseGroup> NewAsyncUniverseGroup(string uri, string auth, string team)
        {
            UniverseGroup universeGroup = new UniverseGroup();
            Connection connection = await Connection.NewAsyncConnection(universeGroup, uri, auth, team).ConfigureAwait(false);

            using (Query query = connection.Query("whoami"))
            {
                await query.Send().ConfigureAwait(false);

                universeGroup.setupPlayer(connection, await query.ReceiveInteger().ConfigureAwait(false));
            }

            return universeGroup;
        }

        /// <summary>
        /// This is the connected player.
        /// </summary>
        public Player Player => player;

        /// <summary>
        /// The name of the UniverseGroup.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The description of the UniverseGroup.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// The GameMode of the UniverseGroup.
        /// </summary>
        public GameMode Mode => mode;

        /// <summary>
        /// The amount of players allowed to play simultaneously.
        /// </summary>
        public int MaxPlayers => maxPlayers;

        /// <summary>
        /// The amount of ships a player can have in the UniverseGroup.
        /// </summary>
        public int MaxShipsPerPlayer => maxShipsPerPlayer;

        /// <summary>
        /// The amount of ships a team can have in the UniverseGroup.
        /// </summary>
        public int MaxShipsPerTeam => maxShipsPerTeam;

        /// <summary>
        /// The amount of bases a player can have in the UniverseGroup.
        /// </summary>
        public int MaxBasesPerPlayer => maxBasesPerPlayer;

        /// <summary>
        /// The amount of bBases a team can have in the UniverseGroup.
        /// </summary>
        public int MaxBasesPerTeam => maxBasesPerTeam;

        /// <summary>
        /// True, if joining this universe as a spectator is allowed.
        /// </summary>
        public bool Spectators => spectators;

        /// <summary>
        /// The amount of ships that you can register in the UniverseGroup.
        /// </summary>
        public int RegisterShipLimit => registerShipLimit;

        /// <summary>
        /// The teams in the UniverseGroup.
        /// </summary>
        public IReadOnlyCollection<Team> Teams => teams;

        /// <summary>
        /// The universes of the universegroup.
        /// </summary>
        public IReadOnlyCollection<Universe> Universes => universes;

        /// <summary>
        /// The system upgrade paths of the universegroup.
        /// </summary>
        public IReadOnlyDictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath> Systems => systems;

        /// <summary>
        /// Tries to get the corresponding universe.
        /// </summary>
        /// <param name="name">The name of the universe.</param>
        /// <param name="universe">The universe or null, if not found.</param>
        /// <returns>true, if the universe has been found, false otherwise.</returns>
        public bool TryGetUniverse(string name, [NotNullWhen(returnValue: true)] out Universe? universe)
        {
            name = name.ToLower();

            foreach (Universe u in universes)
            {
                if (u is null)
                {
                    universe = null;
                    return false;
                }

                if (u.Name.ToLower() == name)
                {
                    universe = u;
                    return true;
                }
            }

            universe = null;
            return false;
        }

        /// <summary>
        /// Tries to get the corresponding universe.
        /// </summary>
        /// <param name="id">The id of the universe.</param>
        /// <param name="universe">The universe or null, if not found.</param>
        /// <returns>true, if the universe has been found, false otherwise.</returns>
        public bool TryGetUniverse(int id, [NotNullWhen(returnValue: true)] out Universe? universe)
        {
            if (id < 0 || id >= 64)
            {
                universe = null;
                return false;
            }

            universe = universes[id];
            return universe != null;
        }

        /// <summary>
        /// Tries to get the corresponding universe.
        /// </summary>
        /// <param name="name">The name of the universe.</param>
        /// <returns>The universe if found, null otherwise.</returns>
        public Universe? GetUniverse(string name)
        {
            name = name.ToLower();

            foreach (Universe universe in universes)
            {
                if (universe is null)
                    return null;

                if (universe.Name.ToLower() == name)
                    return universe;
            }

            return null;
        }

        /// <summary>
        /// Tries to get the corresponding team.
        /// </summary>
        /// <param name="name">The name of the team.</param>
        /// <param name="team">The team or null, if not found.</param>
        /// <returns>true, if the team has been found, false otherwise.</returns>
        public bool TryGetTeam(string name, [NotNullWhen(returnValue: true)] out Team? team)
        {
            name = name.ToLower();

            foreach (Team t in teams)
            {
                if (t is null)
                {
                    team = null;
                    return false;
                }

                if (t.Name.ToLower() == name)
                {
                    team = t;
                    return true;
                }
            }

            team = null;
            return false;
        }

        /// <summary>
        /// Tries to get the corresponding team.
        /// </summary>
        /// <param name="id">The id of the team.</param>
        /// <param name="team">The universe or null, if not found.</param>
        /// <returns>true, if the team has been found, false otherwise.</returns>
        public bool TryGetTeam(int id, [NotNullWhen(returnValue: true)] out Team? team)
        {
            if (id < 0 || id >= 64)
            {
                team = null;
                return false;
            }

            team = teams[id];
            return team != null;
        }

        /// <summary>
        /// Tries to get the corresponding team.
        /// </summary>
        /// <param name="name">The name of the team.</param>
        /// <returns>The team if found, null otherwise.</returns>
        public Team? GetTeam(string name)
        {
            name = name.ToLower();

            foreach (Team team in teams)
            {
                if (team is null)
                    return null;

                if (team.Name.ToLower() == name)
                    return team;
            }

            return null;
        }

        // TOG: Wir brauchen eine weitere Einstellung für die UniversenGruppe: "RegisterShipLimit". In der Datenbank sollte das ein TinyInt oder so sein, in C# dann ein int. Der Wert
        // muss auch an den Connector übertragen werden und gibt an, ob man NewShip aufrufen kann. (Hat ein Spieler mehr Schiffe + 1 als RegisterShipLimit wird der Aufruf verweigert,
        // also ist RegisterShipLimit schlägt der Aufruf für das 3. Schiff fehl.) Sinn ist es dass mehr Schiffe als diese Grenze In-Game gebaut werden müssen anstatt so erzeugt zu werden.

        /// <summary>
        /// Creates a new ship instantly. There is no building process or resource gathering involved. However, the number of ships that can be registered in this manner may be limited
        /// by the rules of the UniverseGroup. (See RegisterShipLimit.)
        /// </summary>
        /// <param name="name">The name of the ship to be created.</param>
        /// <returns>A controllable object that gives control over the ship.</returns>
        /// <exception cref="GameException">Thrown if the name is already in use or if the ship or RegisterShip limits are exceeded.</exception>
        /// <remarks>This will create a DEAD ship. To bring it to life, you need to call the Continue() method on the ship. Typically, you would call NewShip() followed by Continue()
        /// on the controllable.</remarks>
        public async Task<Controllable> NewShip(string name)
        {
            if (!Utils.CheckName(name))
                throw new GameException(0xB2);

            Controllable? controllable = null;

            int controllableCount = 0;
            int firstAvailableSlot = -1;

            lock (syncControllables)
            {
                for (int position = 0; position < controllables.Length; position++)
                    if (controllables[position] is not null)
                        controllableCount++;
                    else if (firstAvailableSlot == -1)
                        firstAvailableSlot = position;

                if (controllableCount >= registerShipLimit)
                    throw new GameException(0x11);

                if (controllableCount >= maxShipsPerPlayer)
                    throw new GameException(0x10);

                if (firstAvailableSlot == -1)
                    throw new GameException(0xB2);

                controllable = new Controllable(this, name, firstAvailableSlot);
                controllables[firstAvailableSlot] = controllable;
            }

            using (Query query = connection.Query("controllableNew"))
            {
                query.Write("controllable", firstAvailableSlot);
                query.Write("name", name);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }

            return controllable;
        }

        /// <summary>
        /// Tries to get the corresponding system.
        /// </summary>
        /// <param name="kind">The system kind of the system.</param>
        /// <param name="level">The system level of the system.</param>
        /// <param name="system">The system or null, if not found.</param>
        /// <returns>true, if the system has been found, false otherwise.</returns>
        public bool TryGetSystem(PlayerUnitSystemKind kind, int level, [NotNullWhen(returnValue: true)] out PlayerUnitSystemUpgradepath? system)
        {
            return systems.TryGetValue(new PlayerUnitSystemIdentifier(kind, level), out system);
        }

        /// <summary>
        /// Tries to get the corresponding system.
        /// </summary>
        /// <param name="identifier">The systemidentifier associated with the upgrade path.</param>
        /// <param name="system">The system or null, if not found.</param>
        /// <returns>true, if the system has been found, false otherwise.</returns>
        public bool TryGetSystem(PlayerUnitSystemIdentifier identifier, [NotNullWhen(returnValue: true)] out PlayerUnitSystemUpgradepath? system)
        {
            return systems.TryGetValue(identifier, out system);
        }

        /// <summary>
        /// Tries to get the corresponding system.
        /// </summary>
        /// <param name="kind">The system kind of the system.</param>
        /// <param name="level">The system level of the system.</param>
        /// <returns>The system if found, null otherwise.</returns>
        public PlayerUnitSystemUpgradepath? GetSystem(PlayerUnitSystemKind kind, int level)
        {
            if (systems.TryGetValue(new PlayerUnitSystemIdentifier(kind, level), out PlayerUnitSystemUpgradepath system))
                return system;

            return null;
        }

        /// <summary>
        /// Tries to get the corresponding system.
        /// </summary>
        /// <param name="identifier">The systemidentifier associated with the upgrade path.</param>
        /// <returns>The system if found, null otherwise.</returns>
        public PlayerUnitSystemUpgradepath? GetSystem(PlayerUnitSystemIdentifier identifier)
        {
            if (systems.TryGetValue(identifier, out PlayerUnitSystemUpgradepath system))
                return system;

            return null;
        }


        /// <summary>
        /// Retrieves the json definition of all systems in the universegroup.
        /// </summary>
        /// <returns>All systems.</returns>
        public async Task<Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath>> GetSystems()
        {
            using (Query query = connection.Query("systemList"))
            {
                await query.Send().ConfigureAwait(false);

                systems = new Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath>();

                JsonElement element = await query.ReceiveJson().ConfigureAwait(false);
                if (element.ValueKind == JsonValueKind.Array)
                    foreach (JsonElement systemObject in element.EnumerateArray())
                    {
                        systems.Add(new PlayerUnitSystemIdentifier(systemObject), new PlayerUnitSystemUpgradepath(systemObject));
                    }

                return systems;
            }
        }

        /// <summary>
        /// Removes the system from the universegroup.
        /// </summary>
        /// <param name="identifier">The system identifier.</param>
        public async Task RemoveSystem(PlayerUnitSystemIdentifier identifier)
        {
            using (Query query = connection.Query("systemRemove"))
            {
                query.Write("system", identifier.Kind.ToString());
                query.Write("level", identifier.Level);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets or adds the system. Systems with level 0 will be default systems a ship starts with. Systems which start at level 1 will have to be built by you.
        /// </summary>
        /// <param name="kind">The kind of the system.</param>
        /// <param name="level">The level of the system that this entry describes.</param>
        /// <param name="energy">The energy cost of building this system in a ship.</param>
        /// <param name="particles">The energy cost of building this system in a ship.</param>
        /// <param name="iron">The iron cost of building this system in a ship.</param>
        /// <param name="carbon">The cabron cost of building this system in a ship.</param>
        /// <param name="silicon">The silicon cost of building this system in a ship.</param>
        /// <param name="platinum">The platinum cost of building this system in a ship.</param>
        /// <param name="gold">The gold cost of building this system in a ship.</param>
        /// <param name="time">The time it takes to build this system.</param>
        /// <param name="value0">The first effecting value this system has.</param>
        /// <param name="value1">The second effecting value this system has.</param>
        /// <param name="value2">The third effecting value this system has.</param>
        /// <param name="areaIncrease">The increase in area of the ship using this system.</param>
        /// <param name="weightIncrease">The increase in weight of the ship using this system.</param>
        /// <returns>Nothing, or throws an error.</returns>
        public async Task SetSystem(PlayerUnitSystemKind kind, int level, double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2, double areaIncrease, double weightIncrease)
        {
            using (Query query = connection.Query("systemSet"))
            {
                query.Write("system", kind.ToString());
                query.Write("level", level);
                query.Write("energy", energy);
                query.Write("particles", particles);
                query.Write("iron", iron);
                query.Write("carbon", carbon);
                query.Write("silicon", silicon);
                query.Write("platinum", platinum);
                query.Write("gold", gold);
                query.Write("time", time);
                query.Write("value0", value0);
                query.Write("value1", value1);
                query.Write("value2", value2);
                query.Write("areaIncrease", areaIncrease);
                query.Write("weightIncrease", weightIncrease);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets or adds the system. Systems with level 0 will be default systems a ship starts with. Systems which start at level 1 will have to be built by you.
        /// </summary>
        /// <param name="kind">The kind of the system.</param>
        /// <param name="level">The level of the system that this entry describes.</param>
        /// <param name="energy">The energy cost of building this system in a ship.</param>
        /// <param name="particles">The energy cost of building this system in a ship.</param>
        /// <param name="iron">The iron cost of building this system in a ship.</param>
        /// <param name="carbon">The cabron cost of building this system in a ship.</param>
        /// <param name="silicon">The silicon cost of building this system in a ship.</param>
        /// <param name="platinum">The platinum cost of building this system in a ship.</param>
        /// <param name="gold">The gold cost of building this system in a ship.</param>
        /// <param name="time">The time it takes to build this system.</param>
        /// <param name="value0">The first effecting value this system has.</param>
        /// <param name="value1">The second effecting value this system has.</param>
        /// <param name="value2">The third effecting value this system has.</param>
        /// <param name="areaIncrease">The increase in area of the ship using this system.</param>
        /// <param name="weightIncrease">The increase in weight of the ship using this system.</param>
        /// <param name="requiredKind">The kind of the system which needs to be present in order to build this system.</param>
        /// <param name="requiredLevel">The level of the system which needs to be present in order to build this system.</param>
        /// <returns>Nothing, or throws an error.</returns>
        public async Task SetSystem(PlayerUnitSystemKind kind, int level, double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2, double areaIncrease, double weightIncrease, PlayerUnitSystemKind requiredKind, int requiredLevel)
        {
            using (Query query = connection.Query("systemSetRequired"))
            {
                query.Write("system", kind.ToString());
                query.Write("level", level);
                query.Write("energy", energy);
                query.Write("particles", particles);
                query.Write("iron", iron);
                query.Write("carbon", carbon);
                query.Write("silicon", silicon);
                query.Write("platinum", platinum);
                query.Write("gold", gold);
                query.Write("time", time);
                query.Write("value0", value0);
                query.Write("value1", value1);
                query.Write("value2", value2);
                query.Write("areaIncrease", areaIncrease);
                query.Write("weightIncrease", weightIncrease);
                query.Write("requiredSystem", requiredKind.ToString());
                query.Write("requiredLevel", requiredLevel);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        // TOG: Später für controllables die selben Methoden wie jetzt für Universes und Teams einbauen. (Siehe darüber.)

        /// <summary>
        /// Will return the next received event from queue or wait until the event has been received.
        /// </summary>
        /// <returns>The corresponding FlattiverseEvenet.</returns>
        public async Task<FlattiverseEvent> NextEvent() => await connection.NextEvent();

        /// <summary>
        /// Empties all resources.
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}