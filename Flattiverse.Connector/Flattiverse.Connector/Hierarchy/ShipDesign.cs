using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ShipDesign : INamedUnit
    {
        public readonly Galaxy Galaxy;

        internal readonly ShipUpgrade?[] upgrades = new ShipUpgrade?[256];
        public readonly UniversalHolder<ShipUpgrade> Upgrades;

        private byte id;
        private ShipDesignConfig config;

        internal ShipDesign(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            config = new ShipDesignConfig(reader);

            Upgrades = new UniversalHolder<ShipUpgrade>(upgrades);
        }

        public int ID => id;
        /// <summary>
        /// The name of the ship.
        /// </summary>
        public string Name => config.Name;
        public ShipDesignConfig Config => config;

        /// <summary>
        /// Sets given values in this ship.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<ShipDesignConfig> config)
        {
            ShipDesignConfig changes = new ShipDesignConfig(this.config);
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4B;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            await session.SendWait(packet);
        }

        /// <summary>
        /// Removes this ship.
        /// </summary>
        /// <returns></returns>
        public async Task Remove()
        {
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4C;
            packet.Header.Param0 = id;

            await session.SendWait(packet);
        }

        /// <summary>
        /// Creates an upgrade with given values in this ship.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<ShipUpgrade> CreateUpgrade(Action<ShipUpgradeConfig> config)
        {
            ShipUpgradeConfig changes = ShipUpgradeConfig.Default;
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4D;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            packet = await session.SendWait(packet);

            if (upgrades[packet.Header.Param0] is not ShipUpgrade upgrade)
                throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

            return upgrade;
        }

        internal void ReadUpgrade(byte id, PacketReader reader)
        {
            upgrades[id] = new ShipUpgrade(Galaxy, this, id, reader);
        }
    }
}
