using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Represents a galaxy (a map) in a Universe.
    /// </summary>
    public class Galaxy : UniversalEnumerable
    {
        /// <summary>
        /// The universe this galaxy belongs to.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The ID of this galaxy among all other galaxies in this universe.
        /// </summary>
        public readonly byte ID;

        private string name;
        private bool spawn;

        internal Galaxy(Universe universe, Packet packet)
        {
            Universe = universe;

            ID = packet.SubAddress;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();
            spawn = reader.ReadBoolean();
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();
            spawn = reader.ReadBoolean();
        }

        /// <summary>
        /// The name of this galaxy.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// true, if you can spawn in this galaxy. false otherwise.
        /// </summary>
        public bool Spawn => spawn;
    }
}
