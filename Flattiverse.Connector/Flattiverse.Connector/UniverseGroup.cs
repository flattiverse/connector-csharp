using Flattiverse.Connector.Accounts;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector
{
    /// <summary>
    /// Represents the main class. The UniverseGroup you connect to is managed by an instance of this class.
    /// </summary>
    public class UniverseGroup : IDisposable
    {
        private readonly Connection connection;

        // players[0-63] are real players, players[64] is a substitute, if the server treats us as non player, like a spectator or admin.
        private readonly Player[] players = new Player[65];

        /// <summary>
        /// This is the connected player.
        /// </summary>
        public readonly Player Player;

        internal string name;
        internal string description;
        internal GameMode mode;

        internal int maxPlayers;
        internal int maxShipsPerPlayer;
        internal int maxShipsPerTeam;
        internal int maxBasesPerPlayer;
        internal int maxBasesPerTeam;

        internal Team[] teams;

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

                Console.WriteLine($"WHOAMI={query.ReceiveInteger().GetAwaiter().GetResult()}.");

                //Player = players[query.ReceiveInteger().GetAwaiter().GetResult()];
            }
        }

        private UniverseGroup(Connection connection, int playerId)
        {
            this.connection = connection;
            Player = players[playerId];
        }

        /// <summary>
        /// Connects to the specific UniverseGroup asynchonously.
        /// </summary>
        /// <param name="uri">The URI of the Universegroup.</param>
        /// <param name="auth">The auth key for UniverseGroup access.</param>
        /// <returns>The connected UniverseGroup.</returns>
        public async Task<UniverseGroup> NewAsyncUniverseGroup(string uri, string auth)
        {
            Connection connection = await Connection.NewAsyncConnection(this, uri, auth).ConfigureAwait(false);

            using (Query query = connection.Query("whoami"))
            {
                await query.Send().ConfigureAwait(false);

                return new UniverseGroup(connection, await query.ReceiveInteger().ConfigureAwait(false));
            }
        }

        // TOG

        /// <summary>
        /// The name of the UniverseGroup.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The description of the UniverseGroup.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// The amount of players allowed to play simultaneous.
        /// </summary>
        public int MaxPlayers => maxPlayers;

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