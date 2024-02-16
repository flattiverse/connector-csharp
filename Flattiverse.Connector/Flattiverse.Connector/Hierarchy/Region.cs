using Flattiverse.Connector.Network;
using System.Diagnostics.Metrics;

namespace Flattiverse.Connector.Hierarchy
{
    public class Region : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly Cluster Cluster;

        private byte id;
        private string name;
        private double startPropability;
        private double respawnPropability;
        private bool @protected;

        internal Region(Galaxy galaxy, Cluster cluster, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            Cluster = cluster;
            this.id = id;

            name = reader.ReadString();
            startPropability = reader.Read2U(100);
            respawnPropability = reader.Read2U(100);
            @protected = reader.ReadBoolean();
        }

        public int ID => id;
        /// <summary>
        /// The name of the region.
        /// </summary>
        public string Name => name;
        public double StartPropability => startPropability;
        public double RespawnPropability => respawnPropability;
        public bool Protected => @protected;

        /// <summary>
        /// Sets given values in this region.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<RegionConfig> config)
        {
            RegionConfig changes = new RegionConfig(this);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x45;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            packet = await session.SendWait(packet);

            if (GameException.Check(packet) is GameException ex)
                throw ex;
        }

        /// <summary>
        /// Removes this region.
        /// </summary>
        /// <returns></returns>
        public async Task Remove()
        {
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x46;
            packet.Header.Param0 = id;

            packet = await session.SendWait(packet);

            if (GameException.Check(packet) is GameException ex)
                throw ex;
        }
    }
}
