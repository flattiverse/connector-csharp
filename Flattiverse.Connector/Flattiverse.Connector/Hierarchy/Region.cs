using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Region : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly Cluster Cluster;

        private byte id;
        private RegionConfig config;

        internal Region(Galaxy galaxy, Cluster cluster, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            Cluster = cluster;
            this.id = id;

            config = new RegionConfig(reader);
        }

        public int ID => id;
        /// <summary>
        /// The name of the region.
        /// </summary>
        public string Name => config.Name;
        public RegionConfig Config => config;

        /// <summary>
        /// Sets given values in this region.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<RegionConfig> config)
        {
            RegionConfig changes = new RegionConfig(this.config);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x45;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            await session.SendWait(packet);
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

            await session.SendWait(packet);
        }
    }
}
