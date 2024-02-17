using Flattiverse.Connector.Network;
using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector
{
    public class Ship : INamedUnit
    {
        public readonly Galaxy Galaxy;

        internal readonly Upgrade?[] upgrades = new Upgrade?[256];
        public readonly UniversalHolder<Upgrade> Upgrades;

        private byte id;
        private ShipConfig config;

        internal Ship(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            config = new ShipConfig(reader);

            Upgrades = new UniversalHolder<Upgrade>(upgrades);
        }

        public int ID => id;
        /// <summary>
        /// The name of the ship.
        /// </summary>
        public string Name => config.Name;
        public ShipConfig Config => config;

        /// <summary>
        /// Sets given values in this ship.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<ShipConfig> config)
        {
            ShipConfig changes = new ShipConfig(this.config);
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
        public async Task<Upgrade> CreateUpgrade(Action<UpgradeConfig> config)
        {
            UpgradeConfig changes = UpgradeConfig.Default;
            config(changes);

            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x4D;
            packet.Header.Param0 = id;

            using (PacketWriter writer = packet.Write())
                changes.Write(writer);

            packet = await session.SendWait(packet);

            if (upgrades[packet.Header.Param0] is not Upgrade upgrade)
                throw GameException.TODO;

            return upgrade;
        }

        internal void ReadUpgrade(byte id, PacketReader reader)
        {
            upgrades[id] = new Upgrade(Galaxy, this, id, reader);
            Console.WriteLine($"Received upgrade {upgrades[id]!.Name} update for ship {Name}");
        }
    }
}
