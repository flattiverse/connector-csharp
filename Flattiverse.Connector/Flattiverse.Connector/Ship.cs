using Flattiverse.Connector.Network;
using Flattiverse.Connector.Hierarchy;
using System.Drawing;

namespace Flattiverse.Connector
{
    public class Ship : INamedUnit
    {
        public readonly Galaxy Galaxy;

        internal readonly Upgrade?[] upgrades = new Upgrade?[256];
        public readonly UniversalHolder<Upgrade> Upgrades;

        private byte id;
        private string name;
        private double costEnergy;
        private double costIon;
        private double costIron;
        private double costTungsten;
        private double costSilicon;
        private double costTritium;
        private double costTime;
        private double hull;
        private double hullRepair;
        private double shields;
        private double shieldsLoad;
        private double size;
        private double weight;
        private double energyMax;
        private double energyCells;
        private double energyReactor;
        private double energyTransfer;
        private double ionMax;
        private double ionCells;
        private double ionReactor;
        private double ionTransfer;
        private double thruster;
        private double nozzle;
        private double speed;
        private double turnrate;
        private double cargo;
        private double extractor;
        private double weaponSpeed;
        private double weaponTime;
        private double weaponLoad;
        private bool freeSpawn;

        internal Ship(Galaxy galaxy, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            this.id = id;

            name = reader.ReadString();
            costEnergy = reader.Read2U(1);
            costIon = reader.Read2U(100);
            costIron = reader.Read2U(1);
            costTungsten = reader.Read2U(100);
            costSilicon = reader.Read2U(1);
            costTritium = reader.Read2U(10);
            costTime = reader.Read2U(10);
            hull = reader.Read2U(10);
            hullRepair = reader.Read2U(100);
            shields = reader.Read2U(10);
            shieldsLoad = reader.Read2U(100);
            size = reader.Read2U(10);
            weight = reader.Read2S(10000);
            energyMax = reader.Read2U(10);
            energyCells = reader.Read4U(100);
            energyReactor = reader.Read2U(100);
            energyTransfer = reader.Read2U(100);
            ionMax = reader.Read2U(100);
            ionCells = reader.Read2U(100);
            ionReactor = reader.Read2U(1000);
            ionTransfer = reader.Read2U(1000);
            thruster = reader.Read2U(10000);
            nozzle = reader.Read2U(100);
            speed = reader.Read2U(100);
            turnrate = reader.Read2U(100);
            cargo = reader.Read4U(1000);
            extractor = reader.Read2U(100);
            weaponSpeed = reader.Read2U(10);
            weaponTime = reader.ReadUInt16();// TODO: MALUK hier wolltest du etwas verrechnen
            weaponLoad = reader.Read2U(10);
            freeSpawn = reader.ReadBoolean();

            Upgrades = new UniversalHolder<Upgrade>(upgrades);
        }

        public int ID => id;
        /// <summary>
        /// The name of the ship.
        /// </summary>
        public string Name => name;
        public double CostEnergy => costEnergy;
        public double CostIon => costIon;
        public double CostIron => costIron;
        public double CostTungsten => costTungsten;
        public double CostSilicon => costSilicon;
        public double CostTritium => costTritium;
        public double CostTime => costTime;
        public double Hull => hull;
        public double HullRepair => hullRepair;
        public double Shields => shields;
        public double ShieldsLoad => shieldsLoad;
        public double Size => size;
        public double Weight => weight;
        public double EnergyMax => energyMax;
        public double EnergyCells => energyCells;
        public double EnergyReactor => energyReactor;
        public double EnergyTransfer => energyTransfer;
        public double IonMax => ionMax;
        public double IonCells => ionCells;
        public double IonReactor => ionReactor;
        public double IonTransfer => ionTransfer;
        public double Thruster => thruster;
        public double Nozzle => nozzle;
        public double Speed => speed;
        public double Turnrate => turnrate;
        public double Cargo => cargo;
        public double Extractor => extractor;
        public double WeaponSpeed => weaponSpeed;
        public double WeaponTime => weaponTime;
        public double WeaponLoad => weaponLoad;
        public bool FreeSpawn => freeSpawn;

        /// <summary>
        /// Sets given values in this ship.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<ShipConfig> config)
        {
            ShipConfig changes = new ShipConfig(this);
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
