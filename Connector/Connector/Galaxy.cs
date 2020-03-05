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
            if (!Units.Unit.CheckName(name))
                throw new ArgumentException("Invalid name.", nameof(name));

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
        /// Updates or creates an unit according to the specified XML data. Requires ManageUnits privilege.
        /// </summary>
        /// <param name="xml">The unit specification.</param>
        public async Task UpdateUnitXml(string xml)
        {
            if (xml == null || xml.Length < 5 || xml.Length > 8192)
                throw new ArgumentException("Xml data is ambiguous.", nameof(xml));

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
        /// Deletes the unit with the given name. Requires ManageUnits privilege.
        /// </summary>
        /// <param name="name">The name of the unit to delete.</param>
        public async Task DeleteUnit(string name)
        {
            if (!Units.Unit.CheckName(name))
                throw new ArgumentException("Invalid name.", nameof(name));

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

        /// <summary>
        /// Queries all regions of the universe.
        /// </summary>
        /// <returns>A list of regions.</returns>
        public async Task<List<Region>> QueryRegions()
        {
            List<Region> regions = new List<Region>();

            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x68;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                Packet response = await session.Wait().ConfigureAwait(false);

                BinaryMemoryReader reader = response.Read();

                for (int position = 0; position < response.Helper; position++)
                    regions.Add(new Region(Universe, ref reader));
            }

            return regions;
        }

        /// <summary>
        /// Updates the specified region. Regions with the same ID will be overwritten.
        /// </summary>
        /// <param name="region">The region to write.</param>
        public async Task UpdateRegion(Region region)
        {
            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x69;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                region.Write(packet.Write());

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the specified region.
        /// </summary>
        /// <param name="region">The region to delete.</param>
        public async Task DeleteRegion(Region region)
        {
            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x6A;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;
                packet.Helper = region.ID;

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Starts the universe view.
        /// </summary>
        public async Task StartView()
        {
            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x84;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Stopps the universe view.
        /// </summary>
        public async Task StopView()
        {
            using (Session session = Universe.Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x85;
                packet.BaseAddress = Universe.ID;
                packet.SubAddress = ID;

                Universe.Server.connection.Send(packet);
                Universe.Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }
    }
}
