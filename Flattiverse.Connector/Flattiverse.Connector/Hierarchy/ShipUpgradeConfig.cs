using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Hierarchy
{
    public class ShipUpgradeConfig
    {
        public string Name;
        public ShipUpgrade? PreviousUpgrade;
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
        public double Size;
        public double Weight;
        public double EnergyMax;
        public double EnergyCells;
        public double EnergyReactor;
        public double EnergyTransfer;
        public double IonMax;
        public double IonCells;
        public double IonReactor;
        public double IonTransfer;
        public double Thruster;
        public double Nozzle;
        public double Speed;
        public double Turnrate;
        public double Cargo;
        public double Extractor;
        public double WeaponSpeed;
        public ushort WeaponTime;
        public double WeaponLoad;
        public ushort WeaponAmmo;
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

        private ShipUpgradeConfig()
        {
            Name = string.Empty;
            PreviousUpgrade = null;
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
            Size = 0;
            Weight = 0;
            EnergyMax = 0;
            EnergyCells = 0;
            EnergyReactor = 0;
            EnergyTransfer = 0;
            IonMax = 0;
            IonCells = 0;
            IonReactor = 0;
            IonTransfer = 0;
            Thruster = 0;
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

        internal ShipUpgradeConfig(ShipUpgradeConfig shipUpgrade)
        {
            Name = shipUpgrade.Name;
            PreviousUpgrade = shipUpgrade.PreviousUpgrade;
            CostEnergy = shipUpgrade.CostEnergy;
            CostIon = shipUpgrade.CostIon;
            CostIron = shipUpgrade.CostIron;
            CostTungsten = shipUpgrade.CostTungsten;
            CostSilicon = shipUpgrade.CostSilicon;
            CostTritium = shipUpgrade.CostTritium;
            CostTime = shipUpgrade.CostTime;
            Hull = shipUpgrade.Hull;
            HullRepair = shipUpgrade.HullRepair;
            Shields = shipUpgrade.Shields;
            ShieldsLoad = shipUpgrade.ShieldsLoad;
            Size = shipUpgrade.Size;
            Weight = shipUpgrade.Weight;
            EnergyMax = shipUpgrade.EnergyMax;
            EnergyCells = shipUpgrade.EnergyCells;
            EnergyReactor = shipUpgrade.EnergyReactor;
            EnergyTransfer = shipUpgrade.EnergyTransfer;
            IonMax = shipUpgrade.IonMax;
            IonCells = shipUpgrade.IonCells;
            IonReactor = shipUpgrade.IonReactor;
            IonTransfer = shipUpgrade.IonTransfer;
            Thruster = shipUpgrade.Thruster;
            Nozzle = shipUpgrade.Nozzle;
            Speed = shipUpgrade.Speed;
            Turnrate = shipUpgrade.Turnrate;
            Cargo = shipUpgrade.Cargo;
            Extractor = shipUpgrade.Extractor;
            WeaponSpeed = shipUpgrade.WeaponSpeed;
            WeaponTime = shipUpgrade.WeaponTime;
            WeaponLoad = shipUpgrade.WeaponLoad;
            WeaponAmmo = shipUpgrade.WeaponAmmo;
            WeaponAmmoProduction = shipUpgrade.WeaponAmmoProduction;
            FreeSpawn = shipUpgrade.FreeSpawn;
            NozzleEnergyConsumption = shipUpgrade.NozzleEnergyConsumption;
            ThrusterEnergyConsumption = shipUpgrade.ThrusterEnergyConsumption;
            HullRepairEnergyConsumption = shipUpgrade.HullRepairEnergyConsumption;
            HullRepairIronConsumption = shipUpgrade.HullRepairIronConsumption;
            ShieldsIonConsumption = shipUpgrade.ShieldsIonConsumption;
            ExtractorEnergyConsumption = shipUpgrade.ExtractorEnergyConsumption;
            WeaponEnergyConsumption = shipUpgrade.WeaponEnergyConsumption;
            ScannerEnergyConsumption = shipUpgrade.ScannerEnergyConsumption;
            ScannerRange = shipUpgrade.ScannerRange;
            ScannerWidth = shipUpgrade.ScannerWidth;
        }

        internal ShipUpgradeConfig(PacketReader reader, ShipDesign ship)
        {
            Name = reader.ReadString();

            if (reader.ReadNullableByte() is byte previousUpgradeId && ship.upgrades[previousUpgradeId] is ShipUpgrade previousUpgrade)
                PreviousUpgrade = previousUpgrade;

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
            Size = reader.ReadDouble();
            Weight = reader.ReadDouble();
            EnergyMax = reader.ReadDouble();
            EnergyCells = reader.ReadDouble();
            EnergyReactor = reader.ReadDouble();
            EnergyTransfer = reader.ReadDouble();
            IonMax = reader.ReadDouble();
            IonCells = reader.ReadDouble();
            IonReactor = reader.ReadDouble();
            IonTransfer = reader.ReadDouble();
            Thruster = reader.ReadDouble();
            Nozzle = reader.ReadDouble();
            Speed = reader.ReadDouble();
            Turnrate = reader.ReadDouble();
            Cargo = reader.ReadDouble();
            Extractor = reader.ReadDouble();
            WeaponSpeed = reader.ReadDouble();
            WeaponTime = reader.ReadUInt16();
            WeaponLoad = reader.ReadDouble();
            // TODO JUW: WeaponLoad wird hier mit 3U, 3 gelesen, aber im Server mit 3U, 1 geschrieben. Alle Werte müssten wirklich überall gesynced sein, Sorry.
            // TODO JUW: Bitte auch WeaponAmmo und Production anpassen (siehe Controllable).
            WeaponAmmo = reader.ReadUInt16();
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

        internal static ShipUpgradeConfig Default => new ShipUpgradeConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.WriteNullable(PreviousUpgrade is null ? null : (byte)PreviousUpgrade.ID);
            writer.Write(CostEnergy);
            writer.Write(CostIon);
            writer.Write(CostIron);
            writer.Write(CostTungsten);
            writer.Write(CostSilicon);
            writer.Write(CostTritium);
            writer.Write(CostTime); // TODO MALUK read as uint16, written as double 2u
            writer.Write(Hull);
            writer.Write(HullRepair);
            writer.Write(Shields);
            writer.Write(ShieldsLoad);
            writer.Write(Size);
            writer.Write(Weight);
            writer.Write(EnergyMax);
            writer.Write(EnergyCells);
            writer.Write(EnergyReactor);
            writer.Write(EnergyTransfer);
            writer.Write(IonMax);
            writer.Write(IonCells);
            writer.Write(IonReactor);
            writer.Write(IonTransfer);
            writer.Write(Thruster);
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
