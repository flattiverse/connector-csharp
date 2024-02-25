using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ShipUpgrade : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly ShipDesign ShipDesign;
        private byte id;

        private ShipUpgradeConfig config;

        internal ShipUpgrade(Galaxy galaxy, ShipDesign shipDesign, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            ShipDesign = shipDesign;
            this.id = id;
            config = new ShipUpgradeConfig(reader, shipDesign);
        }

        public int ID => id;
        /// <summary>
        /// The name of the upgrade.
        /// </summary>
        public string Name => config.Name;
        public ShipUpgradeConfig Config => config;

        /// <summary>
        /// Sets given values in this upgrade.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<ShipUpgradeConfig> config)
        {
            ShipUpgradeConfig changes = new ShipUpgradeConfig(this.config);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4E;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            await session.SendWait(packet);
        }

        /// <summary>
        /// Removes this upgrade.
        /// </summary>
        /// <returns></returns>
        public async Task Remove()
        {
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4F;
            packet.Header.Param0 = id;

            await session.SendWait(packet);
        }
    }
}
