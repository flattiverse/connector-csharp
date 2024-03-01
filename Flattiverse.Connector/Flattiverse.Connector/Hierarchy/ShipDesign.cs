using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// ShipDesign contains the base parameters of a ship.
    /// </summary>
    /// <seealso cref="ShipDesignConfig"/>
    public class ShipDesign : INamedUnit
    {
        /// <summary>
        /// The galaxy the ship design is valid for.
        /// </summary>
        public readonly Galaxy Galaxy;

        internal readonly ShipUpgrade?[] upgrades = new ShipUpgrade?[256];

        /// <summary>
        /// Readonly collection of ship upgrades.
        /// </summary>
        public readonly UniversalHolder<ShipUpgrade> Upgrades;

        internal byte id;
        private ShipDesignConfig config;

        internal ShipDesign(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            config = new ShipDesignConfig(reader);

            Upgrades = new UniversalHolder<ShipUpgrade>(upgrades);
        }

        /// <summary>
        /// The index into the galaxy's ship design list.
        /// </summary>
        public int ID => id;
        
        /// <summary>
        /// The name of the ship.
        /// </summary>
        public string Name => config.Name;

        /// <summary>
        /// The actual base parameters of the ship.
        /// </summary>
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
            packet.Header.Id0 = id;

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
            packet.Header.Id0 = id;

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
            packet.Header.Id0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            packet = await session.SendWait(packet);

            if (upgrades[packet.Header.Id0] is not ShipUpgrade upgrade)
                throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

            return upgrade;
        }


        internal void Update(PacketReader reader)
        {
            config = new ShipDesignConfig(reader);
        }

        internal void ReadUpgrade(byte id, PacketReader reader)
        {
            upgrades[id] = new ShipUpgrade(Galaxy, this, id, reader);
        }
    }
}
