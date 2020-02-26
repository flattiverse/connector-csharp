using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

            name = reader.ReadStringNonNull();
            spawn = reader.ReadBoolean();
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadStringNonNull();
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

        /// <summary>
        /// Queries an unit from the galaxy as Xml representation. Requires ManageUnits privilege.
        /// </summary>
        /// <param name="name">The name of the unit to query.</param>
        /// <returns>The unit represented as xml.</returns>
        public async Task<string> QueryUnitXml(string name)
        {
            if (!Unit.CheckName(name))
                throw new IllegalNameException();

            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x60;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                packet.Write().Write(name);

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                return (await session.Wait().ConfigureAwait(false)).Read().ReadStringNonNull();
            }
        }

        /// <summary>
        /// Updates or creates an unit according to the specified XML data.
        /// </summary>
        /// <param name="xml">The unit specification.</param>
        public async Task UpdateUnitXml(string xml)
        {
            if (xml == null || xml.Length < 5 || xml.Length > 8192)
                throw new AmbiguousXmlDataException();

            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x61;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                packet.Write().Write(xml);

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the unit with the given name.
        /// </summary>
        /// <param name="name">The name of the unit to delete.</param>
        public async Task DeleteUnit(string name)
        {
            if (!Unit.CheckName(name))
                throw new IllegalNameException();

            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x62;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                packet.Write().Write(name);

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }
    }
}
