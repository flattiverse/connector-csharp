using Flattiverse.Connector.Accounts;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System.Diagnostics.CodeAnalysis;

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

        internal Team[] teams = new Team[16];
        internal Universe[] universes = new Universe[64];
        internal Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath> systems = new Dictionary<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath>();

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
        /// The teams in the UniverseGroup.
        /// </summary>
        public IReadOnlyCollection<Team> Teams => teams;

        /// <summary>
        /// The universes of the universegroup.
        /// </summary>
        public IReadOnlyCollection<Universe> Universes => universes;

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