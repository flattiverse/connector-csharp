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
    public class PlayerUnit : Unit
    {
        public readonly int Id;

        private Cluster cluster;

        private ShipDesign shipDesign;

        private Player player;

        private int playerId;
        private int shipDesignId;
        private byte[] upgradeIndex;
        
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
        private double thrusterMaxForward;
        private double thrusterMaxBackward;
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
        private double weaponAmmo;
        private double weaponAmmoMax;
        private double weaponAmmoProduction;

        private Vector position;
        private Vector movement;

        private bool active;


        public ShipDesign ShipDesign => shipDesign;

        public Player Player => player;
        
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
        public double ThrusterMaxForward => thrusterMaxForward;
        public double ThrusterMaxBackward => thrusterMaxBackward;
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
        public double WeaponAmmo => weaponAmmo;
        public double WeaponAmmoMax => weaponAmmoMax;
        public double WeaponAmmoProduction => weaponAmmoProduction;
        public Vector Position => position;
        public Vector Movement => movement;

        public bool Active => active;

        internal PlayerUnit(Cluster cluster, PacketReader reader) : base(reader)
        {
            playerId = reader.ReadInt32();
            shipDesignId = reader.ReadInt32();

            shipDesign = cluster.Galaxy.ShipsDesigns[shipDesignId];
            player = cluster.Galaxy.GetPlayer(playerId);

            Id = reader.ReadByte();

            upgradeIndex = reader.ReadBytes(32);
            hull = reader.ReadDouble();
            hullMax = reader.ReadDouble();
            hullRepair = reader.ReadDouble();
            shields = reader.ReadDouble();
            shieldsMax = reader.ReadDouble();
            shieldsLoad = reader.ReadDouble();
            size = reader.ReadDouble();
            weight = reader.ReadDouble();
            energy = reader.ReadDouble();
            energyMax = reader.ReadDouble();
            energyCells = reader.ReadDouble();
            energyReactor = reader.ReadDouble();
            energyTransfer = reader.ReadDouble();
            ion = reader.ReadDouble();
            ionMax = reader.ReadDouble();
            ionCells = reader.ReadDouble();
            ionReactor = reader.ReadDouble();
            ionTransfer = reader.ReadDouble();
            thruster = reader.ReadDouble();
            thrusterMaxForward = reader.ReadDouble();
            thrusterMaxBackward = reader.ReadDouble();
            nozzle = reader.ReadDouble();
            nozzleMax = reader.ReadDouble();
            speedMax = reader.ReadDouble();
            turnrate = reader.ReadDouble();
            cargoTungsten = reader.ReadDouble();
            cargoIron = reader.ReadDouble();
            cargoSilicon = reader.ReadDouble();
            cargoTritium = reader.ReadDouble();
            cargoMax = reader.ReadDouble();
            extractor = reader.ReadDouble();
            extractorMax = reader.ReadDouble();
            weaponSpeed = reader.ReadDouble();
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.ReadDouble();
            weaponAmmo = reader.ReadDouble();
            weaponAmmoMax = reader.ReadDouble();
            weaponAmmoProduction = reader.ReadDouble();
            position = new Vector(reader);
            movement = new Vector(reader);
            active = true;
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);
            
            
        }

        internal void Deactivate()
        {
            active = false;
        }
    }
}
