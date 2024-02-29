using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;
using Flattiverse.Connector.Hierarchy;
using System.Xml.Linq;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// A sun. Suns have sections which are central to the sun's position.
    /// These sections give energy, ions or both to players.
    /// </summary>
    public class Sun : CelestialBody
    {
        private ReadOnlyCollection<SunSection> sections;

        internal Sun(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            int coronas = reader.ReadByte();

            List<SunSection> tSections = new List<SunSection>();

            for (int position = 0; position < coronas; position++)
                tSections.Add(new SunSection(null, reader));

            sections = new ReadOnlyCollection<SunSection>(tSections);
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);
            
            int coronas = reader.ReadByte();

            List<SunSection> tSections = new List<SunSection>();

            for (int position = 0; position < coronas; position++)
                tSections.Add(new SunSection(null, reader));
            
            sections = new ReadOnlyCollection<SunSection>(tSections);
        }

        /// <summary>
        /// Returns the sections of the sun.
        /// </summary>
        public ReadOnlyCollection<SunSection> Sections => sections;
        
        /// <summary>
        /// Updates the configuration of this unit.
        /// </summary>
        /// <param name="config">The configuration delegate which allows you to setup the configuration.</param>
        /// <returns>The task you should await.</returns>
        public async Task<Sun> Configure(Action<SunConfiguration> config)
        {
            Session session = await Cluster.Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x50;
            packet.Header.Id0 = (byte)Cluster.id;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                writer.Write(Name);

            Packet configurationPacket = await session.SendWait(packet);
            SunConfiguration changes = new SunConfiguration(configurationPacket.Read());
            config(changes);

            session = await Cluster.Galaxy.GetSession();

            packet = new Packet();
            packet.Header.Command = 0x52;
            packet.Header.Id0 = (byte)Cluster.id;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
            {
                writer.Write(Name);
                changes.Write(writer);
            }

            await session.SendWait(packet);

            if (!Cluster.TryGetUnit(changes.Name, out Unit? unit) || unit is not Sun sun)
                throw new GameException(0x35);

            return sun;
        }

        /// <summary>
        /// Removes this unit.
        /// </summary>
        /// <returns></returns>
        public async Task Remove()
        {
            Session session = await Cluster.Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x53;
            packet.Header.Id0 = (byte)Cluster.id;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                writer.Write(Name);

            await session.SendWait(packet);
        }

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.Sun;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Sun {Name}";
        }
    }
}
