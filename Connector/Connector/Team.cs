using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

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
