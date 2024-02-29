using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// A moon. A harvestable unit that can be mined for resources.
    /// Smaller than planets but bigger than meteoroids.
    /// </summary>
    public class Moon : Harvestable
    {
        internal Moon(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
        }

        /// <summary>
        /// Sets given values in this unit.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<Moon> Configure(Action<MoonConfiguration> config)
        {
            Session session = await Cluster.Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x50;
            packet.Header.Id0 = (byte)Cluster.id;
            packet.Header.Param0 = (byte)Kind;

            using (PacketWriter writer = packet.Write())
                writer.Write(Name);

            Packet configurationPacket = await session.SendWait(packet);
            MoonConfiguration changes = new MoonConfiguration(configurationPacket.Read());
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

            if (!Cluster.TryGetUnit(changes.Name, out Unit? unit) || unit is not Moon moon)
                throw new GameException(0x35);

            return moon;
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);
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
        public override UnitKind Kind => UnitKind.Moon;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Moon {Name}";
        }
    }
}
