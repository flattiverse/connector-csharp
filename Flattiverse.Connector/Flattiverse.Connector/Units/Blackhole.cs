﻿using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.Units
{
    public class BlackHole : CelestialBody
    {
        internal ReadOnlyCollection<BlackHoleSection> sections;

        internal BlackHole(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            int coronas = reader.ReadByte();

            List<BlackHoleSection> sections = new List<BlackHoleSection>();

            for (int position = 0; position < coronas; position++)
                sections.Add(new BlackHoleSection(null, reader));
        }

        public ReadOnlyCollection<BlackHoleSection> Sections => sections;

        /// <summary>
        /// Sets given values in this unit.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<BlackHoleConfiguration> config)
        {
            Session session = await Cluster.Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x50;
            packet.Header.Id0 = (byte)Cluster.ID;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                writer.Write(Name);

            Packet configurationPacket = await session.SendWait(packet);
            BlackHoleConfiguration changes = new BlackHoleConfiguration(configurationPacket.Read());
            config(changes);

            session = await Cluster.Galaxy.GetSession();

            packet = new Packet();
            packet.Header.Command = 0x52;
            packet.Header.Id0 = (byte)Cluster.ID;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            await session.SendWait(packet);
        }


        internal override void Update(PacketReader reader)
        {
            base.Update(reader);

            int coronas = reader.ReadByte();

            List<BlackHoleSection> tSections = new List<BlackHoleSection>();

            for (int position = 0; position < coronas; position++)
                tSections.Add(new BlackHoleSection(null, reader));

            sections = new ReadOnlyCollection<BlackHoleSection>(tSections);
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
            packet.Header.Id0 = (byte)Cluster.ID;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                writer.Write(Name);

            await session.SendWait(packet);
        }

        public override string ToString()
        {
            return $"Blackhole {Name}";
        }
    }
}