using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// This is a defined area in a cluster which then can be configured for special game rules
    /// like a starting location or a region with a special name which will be shown to the player via GameMessage.
    /// </summary>
    public class Region : INamedUnit
    {
        /// <summary>
        /// The galaxy this region is in.
        /// </summary>
        public readonly Galaxy Galaxy;

        /// <summary>
        /// The cluster this region is in.
        /// </summary>
        public readonly Cluster Cluster;

        private byte id;
        private RegionConfig config;

        private bool isActive;

        /// <summary>
        /// This flag indicates if this region is still part of the simulation.
        /// </summary>
        public bool IsActive => isActive;

        internal Region(Galaxy galaxy, Cluster cluster, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            Cluster = cluster;
            this.id = id;
            isActive = true;

            config = new RegionConfig(reader);
        }

        /// <summary>
        /// The ID of the region. This is unique in the cluster.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// The name of the region.
        /// </summary>
        /// <remarks>
        /// SAFETY: Make sure this is unique in the cluster, since can be addressed via name.
        /// </remarks>
        public string Name => config.Name;

        /// <summary>
        /// The configuration of this region.
        /// </summary>
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
            packet.Header.Id0 = id;

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
            packet.Header.Id0 = id;

            await session.SendWait(packet);
        }

        internal void Deactivate()
        {
            isActive = false;
        }

        internal void Update(PacketReader reader)
        {
            config = new RegionConfig(reader);
        }
    }
}
