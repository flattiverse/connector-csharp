using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// ShipDesignConfig contains the base parameters of a ship.
    /// </summary>
    public class ShipDesignConfig
    {
        private string name;

        /// <summary>
        /// The Energy cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostEnergy;

        /// <summary>
        /// The ion cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostIon;

        /// <summary>
        /// The iron cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostIron;

        /// <summary>
        /// The tungsten cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostTungsten;

        /// <summary>
        /// The silicon cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostSilicon;

        /// <summary>
        /// The tritium cost of the controllable if FreeSpawn is false.
        /// </summary>
        public double CostTritium;

        /// <summary>
        /// The time it takes to build the controllable if FreeSpawn is false.
        /// </summary>
        public ushort CostTime;

        /// <summary>
        /// The hull level of the controllable.
        /// If it reaches 0, the controllable is destroyed.
        /// The player can call continue to respawn the controllable.
        /// </summary>
        public double Hull;

        /// <summary>
        /// The rate at which the hull is repaired. Consumes Iron and Energy.
        /// </summary>
        /// <remarks>
        /// Repairing is only possible while the ship is not using any other energy consuming functions.
        /// (e.g. Thruster, Weapons, Shields, etc.)
        /// </remarks>
        /// <seealso cref="Hull" />
        public double HullRepair;

        /// <summary>
        /// The current shield value of the ship.
        /// It will be depleted before the hull is damaged.
        /// </summary>
        /// <remarks>
        /// Depending on a value in ShipDesign you may need Ion to recharge the shields.
        /// </remarks>
        public double Shields;

        /// <summary>
        /// The rate at which the shields are recharged.
        /// </summary>
        /// <seealso cref="Shields" />
        public double ShieldsLoad;

        /// <summary>
        /// The radius of the ship. Also affects the hitbox of the ship.
        /// </summary>
        public double Radius;

        /// <summary>.Controllable
        /// The gravity of the ship. Affects the acceleration and deceleration of the ship.
        /// </summary>
        public double Gravity;

        /// <summary>
        /// The current energy value of the ship.
        /// It can be gained via sections/coronas around suns or via the energy reactor of stations.
        /// </summary>
        /// <remarks>
        /// Energy is used for the thruster and the weapons.
        /// It can also be used to repair the hull or recharge the Shields.
        /// </remarks>
        public double EnergyMax;

        /// <summary>
        /// This is a multiplier for the rate of energy gain from suns and stations.
        /// </summary>
        public double EnergyCells;

        /// <summary>
        /// Used by stations to produce energy.
        /// </summary>
        public double EnergyReactor;

        /// <summary>
        /// The rate at which energy can be transferred to other ships.
        /// </summary>
        /// <remarks>
        /// All ships can transfer energy to other ships.
        /// When doing this, a corona will be created around the ship,
        /// which can be used by other ships to gain energy, even enemies.
        /// </remarks>
        public double EnergyTransfer;

        /// <summary>
        /// The maximum ion value of the ship.
        /// </summary>
        public double IonMax;

        /// <summary>
        /// This is a multiplier for the rate of ion gain from corona sections.
        /// </summary>
        public double IonCells;

        /// <summary>
        /// Used by stations to produce ion.
        /// </summary>
        public double IonReactor;

        /// <summary>
        /// The rate at which ion can be transferred to other ships per tick.
        /// </summary>
        /// <remarks>
        /// All ships can transfer ion to other ships.
        /// When doing this, a corona will be created around the ship,
        /// which can be used by other ships to gain ion, even enemies.
        /// </remarks>
        public double IonTransfer;

        /// <summary>
        /// The maximum forward thruster value of the ship.
        /// </summary>
        public double ThrusterForwardMax;

        /// <summary>
        /// The maximum backward thruster value of the ship.
        /// </summary>
        /// <remarks>
        /// The negative value is usually limited to smaller value than the positive one.
        /// </remarks>
        public double ThrusterBackwardMax;

        /// <summary>
        /// The maximum nozzle value of the ship.
        /// As opposed to the thruster this is symmetrical
        /// </summary>
        public double NozzleMax;

        /// <summary>
        /// The maximum speed of the ship.
        /// </summary>
        /// <remarks>
        /// Past this speed the velocity will be dampened by 10% per tick.
        /// You can still accelerate to go slightly faster, but at the cost of more energy.
        /// </remarks>
        public double SpeedMax;

        /// <summary>
        /// The maximum turnrate of the ship in degrees/tick.
        /// Past this value the turnrate will be dampened by 30% per tick.
        /// You can still accelerate to turn slightly faster, but at the cost of more energy.
        /// </summary>
        public double TurnrateMax;

        /// <summary>
        /// The maximum cummulative cargo of the ship.
        /// </summary>
        /// <remarks>
        /// The materials factored in are Tungsten, Iron, Silicon and Tritium.
        /// </remarks>
        public double CargoMax;

        /// <summary>
        /// The maximum amount of resources that can be extracted from resource sections per tick.
        /// </summary>
        /// <remarks>
        /// Resource sections are defined in the map.
        /// They are found around planets and moons.
        /// </remarks>
        public double ExtractorMax;

        /// <summary>
        /// The speed of the weapon projectile.
        /// </summary>
        public double WeaponSpeedMax;

        /// <summary>
        /// The maximum fuse time of the weapon projectile.
        /// </summary>
        public ushort WeaponTimeMax;

        /// <summary>
        /// The damage radius of the weapon projectile.
        /// </summary>
        public double WeaponLoad;

        /// <summary>
        /// The maximum ammount of ammunition the ship can carry.
        /// </summary>
        public double WeaponAmmoMax;

        /// <summary>
        /// The rate at which the weapon ammunition is produced.
        /// </summary>
        public double WeaponAmmoProduction;

        /// <summary>
        /// If true, the ship can be spawned without any cost.
        /// </summary>
        public bool FreeSpawn;

        /// <summary>
        /// The energy consumption of the nozzle per tick.
        /// </summary>
        public double NozzleEnergyConsumption;

        /// <summary>
        /// The energy consumption of the thruster per tick.
        /// </summary>
        public double ThrusterEnergyConsumption;

        /// <summary>
        /// The energy consumption of the hull repair per tick.
        /// </summary>
        public double HullRepairEnergyConsumption;

        /// <summary>
        /// The iron consumption of the hull repair per tick.
        /// </summary>
        /// 
        public double HullRepairIronConsumption;
        
        /// <summary>
        /// The ion consumption of the shields per tick.
        /// </summary>
        public double ShieldsIonConsumption;

        /// <summary>
        /// The energy consumption of the extractor per tick.
        /// </summary>
        public double ExtractorEnergyConsumption;
        
        /// <summary>
        /// The energy consumption of the weapon per shot.
        /// </summary>
        public double WeaponEnergyConsumption;

        /// <summary>
        /// The energy consumption of the scanner per tick.
        /// </summary>
        public double ScannerEnergyConsumption;
        
        /// <summary>
        /// The range of the scanner. Affects energy consumption.
        /// </summary>
        public double ScannerRange;

        /// <summary>
        /// The width of the scanner. Affects energy consumption.
        /// </summary>
        public double ScannerWidth;

        /// <summary>
        /// The name of the configured unit.
        /// </summary>
        /// <exception cref="GameException">0x32 may be thrown, if the name violates rules.</exception>
        public string Name
        {
            get => name;
            set
            {
                if (!Utils.CheckName32(value))
                    throw new GameException(0x31);

                name = value;
            }
        }

        private ShipDesignConfig()
        {
            Name = string.Empty;
            CostEnergy = 0;
            CostIon = 0;
            CostIron = 0;
            CostTungsten = 0;
            CostSilicon = 0;
            CostTritium = 0;
            CostTime = 0;
            Hull = 0;
            HullRepair = 0;
            Shields = 0;
            ShieldsLoad = 0;
            Radius = 0;
            Gravity = 0;
            EnergyMax = 0;
            EnergyCells = 0;
            EnergyReactor = 0;
            EnergyTransfer = 0;
            IonMax = 0;
            IonCells = 0;
            IonReactor = 0;
            IonTransfer = 0;
            ThrusterForwardMax = 0;
            ThrusterBackwardMax = 0;
            NozzleMax = 0;
            SpeedMax = 0;
            TurnrateMax = 0;
            CargoMax = 0;
            ExtractorMax = 0;
            WeaponSpeedMax = 0;
            WeaponTimeMax = 0;
            WeaponLoad = 0;
            WeaponAmmoMax = 0;
            WeaponAmmoProduction = 0;
            FreeSpawn = true;
            NozzleEnergyConsumption = 0;
            ThrusterEnergyConsumption = 0;
            HullRepairEnergyConsumption = 0;
            HullRepairIronConsumption = 0;
            ShieldsIonConsumption = 0;
            ExtractorEnergyConsumption = 0;
            WeaponEnergyConsumption = 0;
            ScannerEnergyConsumption = 0;
            ScannerRange = 0;
            ScannerWidth = 0;
        }

        internal ShipDesignConfig(ShipDesignConfig ship)
        {
            Name = ship.Name;
            CostEnergy = ship.CostEnergy;
            CostIon = ship.CostIon;
            CostIron = ship.CostIron;
            CostTungsten = ship.CostTungsten;
            CostSilicon = ship.CostSilicon;
            CostTritium = ship.CostTritium;
            CostTime = ship.CostTime;
            Hull = ship.Hull;
            HullRepair = ship.HullRepair;
            Shields = ship.Shields;
            ShieldsLoad = ship.ShieldsLoad;
            Radius = ship.Radius;
            Gravity = ship.Gravity;
            EnergyMax = ship.EnergyMax;
            EnergyCells = ship.EnergyCells;
            EnergyReactor = ship.EnergyReactor;
            EnergyTransfer = ship.EnergyTransfer;
            IonMax = ship.IonMax;
            IonCells = ship.IonCells;
            IonReactor = ship.IonReactor;
            IonTransfer = ship.IonTransfer;
            ThrusterForwardMax = ship.ThrusterForwardMax;
            ThrusterBackwardMax = ship.ThrusterBackwardMax;
            NozzleMax = ship.NozzleMax;
            SpeedMax = ship.SpeedMax;
            TurnrateMax = ship.TurnrateMax;
            CargoMax = ship.CargoMax;
            ExtractorMax = ship.ExtractorMax;
            WeaponSpeedMax = ship.WeaponSpeedMax;
            WeaponTimeMax = ship.WeaponTimeMax;
            WeaponLoad = ship.WeaponLoad;
            WeaponAmmoMax = ship.WeaponAmmoMax;
            WeaponAmmoProduction = ship.WeaponAmmoProduction;
            FreeSpawn = ship.FreeSpawn;
            NozzleEnergyConsumption = ship.NozzleEnergyConsumption;
            ThrusterEnergyConsumption = ship.ThrusterEnergyConsumption;
            HullRepairEnergyConsumption = ship.HullRepairEnergyConsumption;
            HullRepairIronConsumption = ship.HullRepairIronConsumption;
            ShieldsIonConsumption = ship.ShieldsIonConsumption;
            ExtractorEnergyConsumption = ship.ExtractorEnergyConsumption;
            WeaponEnergyConsumption = ship.WeaponEnergyConsumption;
            ScannerEnergyConsumption = ship.ScannerEnergyConsumption;
            ScannerRange = ship.ScannerRange;
            ScannerWidth = ship.ScannerWidth;
        }

        internal ShipDesignConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            CostEnergy = reader.ReadDouble();
            CostIon = reader.ReadDouble();
            CostIron = reader.ReadDouble();
            CostTungsten = reader.ReadDouble();
            CostSilicon = reader.ReadDouble();
            CostTritium = reader.ReadDouble();
            CostTime = reader.ReadUInt16();
            Hull = reader.ReadDouble();
            HullRepair = reader.ReadDouble();
            Shields = reader.ReadDouble();
            ShieldsLoad = reader.ReadDouble();
            Radius = reader.ReadDouble();
            Gravity = reader.ReadDouble();
            EnergyMax = reader.ReadDouble();
            EnergyCells = reader.ReadDouble();
            EnergyReactor = reader.ReadDouble();
            EnergyTransfer = reader.ReadDouble();
            IonMax = reader.ReadDouble();
            IonCells = reader.ReadDouble();
            IonReactor = reader.ReadDouble();
            IonTransfer = reader.ReadDouble();
            ThrusterForwardMax = reader.ReadDouble();
            ThrusterBackwardMax = reader.ReadDouble();
            NozzleMax = reader.ReadDouble();
            SpeedMax = reader.ReadDouble();
            TurnrateMax = reader.ReadDouble();
            CargoMax = reader.ReadDouble();
            ExtractorMax = reader.ReadDouble();
            WeaponSpeedMax = reader.ReadDouble();
            WeaponTimeMax = reader.ReadUInt16();
            WeaponLoad = reader.ReadDouble();
            WeaponAmmoMax = reader.ReadDouble();
            WeaponAmmoProduction = reader.ReadDouble();
            FreeSpawn = reader.ReadBoolean();
            
            NozzleEnergyConsumption = reader.ReadDouble();
            ThrusterEnergyConsumption = reader.ReadDouble();
            HullRepairEnergyConsumption = reader.ReadDouble();
            HullRepairIronConsumption = reader.ReadDouble();
            ShieldsIonConsumption = reader.ReadDouble();
            ExtractorEnergyConsumption = reader.ReadDouble();
            WeaponEnergyConsumption = reader.ReadDouble();
            ScannerEnergyConsumption = reader.ReadDouble();
            ScannerRange = reader.ReadDouble();
            ScannerWidth = reader.ReadDouble();
        }

        internal static ShipDesignConfig Default => new ShipDesignConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write(CostEnergy);
            writer.Write(CostIon);
            writer.Write(CostIron);
            writer.Write(CostTungsten);
            writer.Write(CostSilicon);
            writer.Write(CostTritium);
            writer.Write(CostTime);
            writer.Write(Hull);
            writer.Write(HullRepair);
            writer.Write(Shields);
            writer.Write(ShieldsLoad);
            writer.Write(Radius);
            writer.Write(Gravity);
            writer.Write(EnergyMax);
            writer.Write(EnergyCells);
            writer.Write(EnergyReactor);
            writer.Write(EnergyTransfer);
            writer.Write(IonMax);
            writer.Write(IonCells);
            writer.Write(IonReactor);
            writer.Write(IonTransfer);
            writer.Write(ThrusterForwardMax);
            writer.Write(ThrusterBackwardMax);
            writer.Write(NozzleMax);
            writer.Write(SpeedMax);
            writer.Write(TurnrateMax);
            writer.Write(CargoMax);
            writer.Write(ExtractorMax);
            writer.Write(WeaponSpeedMax);
            writer.Write(WeaponTimeMax);
            writer.Write(WeaponLoad);
            writer.Write(WeaponAmmoMax);
            writer.Write(WeaponAmmoProduction);
            writer.Write(FreeSpawn);
            writer.Write(NozzleEnergyConsumption);
            writer.Write(ThrusterEnergyConsumption);
            writer.Write(HullRepairEnergyConsumption);
            writer.Write(HullRepairIronConsumption);
            writer.Write(ShieldsIonConsumption);
            writer.Write(ExtractorEnergyConsumption);
            writer.Write(WeaponEnergyConsumption);
            writer.Write(ScannerEnergyConsumption);
            writer.Write(ScannerRange);
            writer.Write(ScannerWidth);
        }
    }
}
