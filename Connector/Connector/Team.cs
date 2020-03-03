using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    /// <summary>
    /// Represents a team in an universe.
    /// </summary>
    public class Team : UniversalEnumerable
    {
        /// <summary>
        /// The universe this team belongs to.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The ID of this Team in the Universe.
        /// </summary>
        public readonly byte ID;

        private string name;

        private byte r;
        private byte g;
        private byte b;

        internal Team(Universe universe, Packet packet)
        {
            Universe = universe;
            ID = packet.SubAddress;

            updateFromPacket(packet);
        }

        /// <summary>
        /// Sending a message to all players in this team.
        /// </summary>
        /// <param name="chatMessage">The chat message that should be send to all players of the universe.</param>
        /// <exception cref="ArgumentException">The chat message is larger than 256 characters.</exception>
        /// <exception cref="ArgumentNullException">The chat message is null.</exception>
        public async Task Chat(string chatMessage)
        {
            if (chatMessage == null)
                throw new ArgumentNullException("The chat message can't be null.", chatMessage);

            if (chatMessage.Length > 256)
            {
                throw new ArgumentException("The chat message is not allowed to have more than 256 characters!", chatMessage);
            }

            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x67;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                packet.Write().Write(chatMessage);

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();

            r = reader.ReadByte();
            g = reader.ReadByte();
            b = reader.ReadByte();
        }

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The red color component from 0f to 1f.
        /// </summary>
        public byte R => r;
        
        /// <summary>
        /// The green color component from 0f to 1f.
        /// </summary>
        public byte G => g;

        /// <summary>
        /// The blue color component from 0f to 1f.
        /// </summary>
        public byte B => b;

        /// <summary>
        /// The color as hex representation without leading hash.
        /// </summary>
        public string Hex => $"{r.ToString("X02")}{g.ToString("X02")}{b.ToString("X02")}";
    }
}
