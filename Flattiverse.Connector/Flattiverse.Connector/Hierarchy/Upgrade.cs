using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Upgrade : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly ShipDesign Ship;
        private byte id;

        private UpgradeConfig config;

        internal Upgrade(Galaxy galaxy, ShipDesign ship, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            Ship = ship;
            this.id = id;
            config = new UpgradeConfig(reader, ship);
        }

        public int ID => id;
        /// <summary>
        /// The name of the upgrade.
        /// </summary>
        public string Name => config.Name;
        public UpgradeConfig Config => config;

        /// <summary>
        /// Sets given values in this upgrade.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<UpgradeConfig> config)
        {
            UpgradeConfig changes = new UpgradeConfig(this.config);
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
