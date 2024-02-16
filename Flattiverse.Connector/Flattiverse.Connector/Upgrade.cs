using Flattiverse.Connector.Network;
using Flattiverse.Connector.Hierarchy;
using System.Drawing;

namespace Flattiverse.Connector
{
    public class Upgrade : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly Ship Ship;

        public readonly Upgrade? PreviousUpgrade;

        internal byte id;
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

        internal Upgrade(Galaxy galaxy, Ship ship, byte id, PacketReader reader)
        {
            Galaxy = galaxy;
            Ship = ship;
            this.id = id;

            name = reader.ReadString();

            if (reader.ReadNullableByte() is byte previousUpgradeId && ship.upgrades[previousUpgradeId] is Upgrade previousUpgrade)
                PreviousUpgrade = previousUpgrade;

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
            weaponTime = reader.ReadUInt16() / 20.0;
            weaponLoad = reader.Read2U(10);
            freeSpawn = reader.ReadBoolean();
        }

        public int ID => id;
        /// <summary>
        /// The name of the upgrade.
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
        /// Sets given values in this upgrade.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task Configure(Action<UpgradeConfig> config)
        {
            UpgradeConfig changes = new UpgradeConfig(this);
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
