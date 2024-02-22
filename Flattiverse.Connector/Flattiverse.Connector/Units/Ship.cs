using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    // TODO Fehlende overrides von Unit für Ship
    public class Ship : Unit
    {
        public readonly int Id;

        private Cluster cluster;

        private ShipDesign shipDesign;

        private Player player;

        private int playerId;
        private int shipDesignId;
        private int upgradeIndex;
        private double hull;
        private double hullMax;
        private double hullRepair;
        private double shields;
        private double shieldsMax;
        private double shieldsLoad;
        private double size;
        private double weight;
        private double energy;
        private double energyMax;
        private double energyCells;
        private double energyReactor;
        private double energyTransfer;
        private double ion;
        private double ionMax;
        private double ionCells;
        private double ionReactor;
        private double ionTransfer;
        private double thruster;
        private double thrusterMax;
        private double nozzle;
        private double nozzleMax;
        private double speedMax;
        private double turnrate;
        private double cargoTungsten;
        private double cargoIron;
        private double cargoSilicon;
        private double cargoTritium;
        private double cargoMax;
        private double extractor;
        private double extractorMax;
        private double weaponSpeed;
        private ushort weaponTime;
        private double weaponLoad;
        private ushort weaponAmmo;
        private ushort weaponAmmoMax;
        private double weaponAmmoProduction;


        public ShipDesign ShipDesign => shipDesign;

        public Player Player => player;

        public int PlayerId => playerId;
        public int ShipDesignId => shipDesignId;
        public int UpgradeIndex => upgradeIndex;
        public double Hull => hull;
        public double HullMax => hullMax;
        public double HullRepair => hullRepair;
        public double Shields => shields;
        public double ShieldsMax => shieldsMax;
        public double ShieldsLoad => shieldsLoad;
        public double Size => size;
        public double Weight => weight;
        public double Energy => energy;
        public double EnergyMax => energyMax;
        public double EnergyCells => energyCells;
        public double EnergyReactor => energyReactor;
        public double EnergyTransfer => energyTransfer;
        public double Ion => ion;
        public double IonMax => ionMax;
        public double IonCells => ionCells;
        public double IonReactor => ionReactor;
        public double IonTransfer => ionTransfer;
        public double Thruster => thruster;
        public double ThrusterMax => thrusterMax;
        public double Nozzle => nozzle;
        public double NozzleMax => nozzleMax;
        public double SpeedMax => speedMax;
        public double Turnrate => turnrate;
        public double CargoTungsten => cargoTungsten;
        public double CargoIron => cargoIron;
        public double CargoSilicon => cargoSilicon;
        public double CargoTritium => cargoTritium;
        public double CargoMax => cargoMax;
        public double Extractor => extractor;
        public double ExtractorMax => extractorMax;
        public double WeaponSpeed => weaponSpeed;
        public ushort WeaponTime => weaponTime;
        public double WeaponLoad => weaponLoad;
        public ushort WeaponAmmo => weaponAmmo;
        public ushort WeaponAmmoMax => weaponAmmoMax;
        public double WeaponAmmoProduction => weaponAmmoProduction;

        internal Ship(Cluster cluster, PacketReader reader) : base(reader)
        {
            playerId = reader.ReadInt32();
            shipDesignId = reader.ReadInt32();

            shipDesign = cluster.Galaxy.ShipsDesigns[shipDesignId];
            player = cluster.Galaxy.GetPlayer(playerId);

            Id = reader.ReadInt32();

            upgradeIndex = reader.ReadInt32();
            hull = reader.Read2U(10);
            hullMax = reader.Read2U(10);
            hullRepair = reader.Read2U(100);
            shields = reader.Read2U(10);
            shieldsMax = reader.Read2U(10);
            shieldsLoad = reader.Read2U(100);
            size = reader.Read2U(10);
            weight = reader.Read2S(10000);
            energy = reader.Read4U(10);
            energyMax = reader.Read4U(10);
            energyCells = reader.Read2U(100);
            energyReactor = reader.Read2U(100);
            energyTransfer = reader.Read2U(100);
            ion = reader.Read2U(100);
            ionMax = reader.Read2U(100);
            ionCells = reader.Read2U(100);
            ionReactor = reader.Read2U(1000);
            ionTransfer = reader.Read2U(1000);
            thruster = reader.Read2U(10000);
            thrusterMax = reader.Read2U(10000);
            nozzle = reader.Read2U(100);
            nozzleMax = reader.Read2U(100);
            speedMax = reader.Read2U(100);
            turnrate = reader.Read2U(100);
            cargoTungsten = reader.Read4U(1000);
            cargoIron = reader.Read4U(1000);
            cargoSilicon = reader.Read4U(1000);
            cargoTritium = reader.Read4U(1000);
            cargoMax = reader.Read4U(1000);
            extractor = reader.Read2U(100);
            extractorMax = reader.Read2U(100);
            weaponSpeed = reader.Read2U(10);
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.Read2U(10);
            weaponAmmo = reader.ReadUInt16();
            weaponAmmoMax = reader.ReadUInt16();
            weaponAmmoProduction = reader.Read2U(100000);
        }
    }
}
