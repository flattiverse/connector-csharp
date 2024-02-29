using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// Ship upgrades improve the capabilities of a ship.
    /// </summary>
    /// <remarks>
    /// These represent deltas to the <see cref="ShipDesign" />.
    /// </remarks>
    /// <see cref="ShipDesign" />
    public class ShipUpgrade : INamedUnit
    {

        /// <summary>
        /// The galaxy this upgrade can be used in.
        /// </summary>
        public readonly Galaxy Galaxy;

        /// <summary>
        /// The ship design this upgrade is for.
        /// </summary>
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

        /// <summary>
        /// The ID of the upgrade. This is unique per ship design.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// The name of the upgrade.
        /// </summary>
        public string Name => config.Name;

        /// <summary>
        /// The configuration of the upgrade.
        /// </summary>
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
            packet.Header.Id0 = id;
            packet.Header.Id1 = ShipDesign.id;

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
            packet.Header.Id0 = id;
            packet.Header.Id1 = ShipDesign.id;

            await session.SendWait(packet);
        }
    }
}
