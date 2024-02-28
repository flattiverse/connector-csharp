using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    public class ShipDesignConfig
    {
        private string name;
        public double CostEnergy;
        public double CostIon;
        public double CostIron;
        public double CostTungsten;
        public double CostSilicon;
        public double CostTritium;
        public ushort CostTime;
        public double Hull;
        public double HullRepair;
        public double Shields;
        public double ShieldsLoad;
        public double Radius;
        public double Gravity;
        public double EnergyMax;
        public double EnergyCells;
        public double EnergyReactor;
        public double EnergyTransfer;
        public double IonMax;
        public double IonCells;
        public double IonReactor;
        public double IonTransfer;
        public double ThrusterForward;
        public double ThrusterBackward;
        public double Nozzle;
        public double Speed;
        public double Turnrate;
        public double Cargo;
        public double Extractor;
        public double WeaponSpeed;
        public ushort WeaponTime;
        public double WeaponLoad;
        public double WeaponAmmo;
        public double WeaponAmmoProduction;
        public bool FreeSpawn;
        public double NozzleEnergyConsumption;
        public double ThrusterEnergyConsumption;
        public double HullRepairEnergyConsumption;
        public double HullRepairIronConsumption;
        public double ShieldsIonConsumption;
        public double ExtractorEnergyConsumption;
        public double WeaponEnergyConsumption;
        public double ScannerEnergyConsumption;
        public double ScannerRange;
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
            ThrusterForward = 0;
            ThrusterBackward = 0;
            Nozzle = 0;
            Speed = 0;
            Turnrate = 0;
            Cargo = 0;
            Extractor = 0;
            WeaponSpeed = 0;
            WeaponTime = 0;
            WeaponLoad = 0;
            WeaponAmmo = 0;
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
            ThrusterForward = ship.ThrusterForward;
            ThrusterBackward = ship.ThrusterBackward;
            Nozzle = ship.Nozzle;
            Speed = ship.Speed;
            Turnrate = ship.Turnrate;
            Cargo = ship.Cargo;
            Extractor = ship.Extractor;
            WeaponSpeed = ship.WeaponSpeed;
            WeaponTime = ship.WeaponTime;
            WeaponLoad = ship.WeaponLoad;
            WeaponAmmo = ship.WeaponAmmo;
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
            ThrusterForward = reader.ReadDouble();
            ThrusterBackward = reader.ReadDouble();
            Nozzle = reader.ReadDouble();
            Speed = reader.ReadDouble();
            Turnrate = reader.ReadDouble();
            Cargo = reader.ReadDouble();
            Extractor = reader.ReadDouble();
            WeaponSpeed = reader.ReadDouble();
            WeaponTime = reader.ReadUInt16();
            WeaponLoad = reader.ReadDouble();
            WeaponAmmo = reader.ReadDouble();
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
            writer.Write(ThrusterForward);
            writer.Write(ThrusterBackward);
            writer.Write(Nozzle);
            writer.Write(Speed);
            writer.Write(Turnrate);
            writer.Write(Cargo);
            writer.Write(Extractor);
            writer.Write(WeaponSpeed);
            writer.Write(WeaponTime);
            writer.Write(WeaponLoad);
            writer.Write(WeaponAmmo);
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
