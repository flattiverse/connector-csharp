using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    /// <summary>
    /// This class represents a player currently in the game.
    /// </summary>
    public class Player : UniversalEnumerable
    {
        /// <summary>
        /// The server over which you have received this player infos.
        /// </summary>
        public readonly Server Server;

        internal int id;
        private string name;
        private bool online;
        private float ping;

        private uint account;

        private Universe universe;
        private Team team;

        /// <summary>
        /// The scores of the player.
        /// </summary>
        public readonly Scores Scores;

        internal Player(Server server, Packet packet)
        {
            Server = server;

            id = packet.BaseAddress;
            account = packet.ID;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadStringNonNull();
            online = reader.ReadBoolean();
            ping = reader.ReadSingle();

            Scores = new Scores();
        }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// true, if the player is still online. false otherwise. If false the player may have lingering ships.
        /// </summary>
        public bool Online => online;

        /// <summary>
        /// The ping in seconds of the players connection.
        /// </summary>
        public float Ping => ping;

        /// <summary>
        /// The universe the player did join. null, if the player isn't in an universe.
        /// </summary>
        public Universe Universe => universe;

        /// <summary>
        /// The team the player is on. null, if the player isn't in an universe.
        /// </summary>
        public Team Team => team;

        /// <summary>
        /// Sending a message to this player.
        /// </summary>
        /// <param name="message">The chat message that should be send to this player.</param>
        /// <exception cref="ArgumentException">The chat message is larger than 256 characters.</exception>
        /// <exception cref="ArgumentNullException">The chat message is null.</exception>
        public async Task Chat(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "The chat message can't be null.");

            if (message.Length > 256)
                throw new ArgumentException("The chat message is not allowed to have more than 256 characters.", nameof(message));

            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x65;
                packet.BaseAddress = (ushort)id;

                packet.Write().Write(message);

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        internal void UpdatePing(Packet packet)
        {
            ping = packet.Read().PeekSingle();
        }

        internal void UpdatePlayerAssignment(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            if (reader.Size == 0)
            {
                universe = null;
                team = null;

                return;
            }

            universe = Server.universes[reader.ReadUInt16()];

            if (universe == null)
                return;

            team = universe.teams[reader.ReadByte()];
        }
    }
}
