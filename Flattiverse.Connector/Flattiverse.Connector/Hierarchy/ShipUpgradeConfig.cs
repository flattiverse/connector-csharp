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
        public double CostTime; // TODO MALUK this is a ushort in ShipDesignConfig
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
        public double WeaponTime; // TODO MALUK this is a ushort in ShipDesignConfig
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

            CostEnergy = reader.Read2U(1);
            CostIon = reader.Read2U(100);
            CostIron = reader.Read2U(1);
            CostTungsten = reader.Read2U(100);
            CostSilicon = reader.Read2U(1);
            CostTritium = reader.Read2U(10);
            CostTime = reader.ReadUInt16();
            Hull = reader.Read2U(10);
            HullRepair = reader.Read2U(100);
            Shields = reader.Read2U(10);
            ShieldsLoad = reader.Read2U(100);
            Size = reader.Read3U(1000);
            Weight = reader.Read2S(10000);
            EnergyMax = reader.Read2U(10);
            EnergyCells = reader.Read4U(100);
            EnergyReactor = reader.Read2U(100);
            EnergyTransfer = reader.Read2U(100);
            IonMax = reader.Read2U(100);
            IonCells = reader.Read2U(100);
            IonReactor = reader.Read2U(1000);
            IonTransfer = reader.Read2U(1000);
            Thruster = reader.Read2U(10000);
            Nozzle = reader.Read2U(100);
            Speed = reader.Read2U(100);
            Turnrate = reader.Read2U(100);
            Cargo = reader.Read4U(1000);
            Extractor = reader.Read2U(100);
            WeaponSpeed = reader.Read2U(10);
            WeaponTime = reader.ReadUInt16();
            WeaponLoad = reader.Read3U(1000);
            // TODO JUW: WeaponLoad wird hier mit 3U, 3 gelesen, aber im Server mit 3U, 1 geschrieben. Alle Werte müssten wirklich überall gesynced sein, Sorry.
            // TODO JUW: Bitte auch WeaponAmmo und Production anpassen (siehe Controllable).
            WeaponAmmo = reader.ReadUInt16();
            WeaponAmmoProduction = reader.Read2U(100000);
            FreeSpawn = reader.ReadBoolean();
            NozzleEnergyConsumption = reader.Read4U(1000);
            ThrusterEnergyConsumption = reader.Read4U(1000);
            HullRepairEnergyConsumption = reader.Read4U(1000);
            HullRepairIronConsumption = reader.Read4U(1000);
            ShieldsIonConsumption = reader.Read4U(1000);
            ExtractorEnergyConsumption = reader.Read4U(1000);
            WeaponEnergyConsumption = reader.Read4U(1000);
            ScannerEnergyConsumption = reader.Read4U(1000);
            ScannerRange = reader.Read3U(1000);
            ScannerWidth = reader.Read2U(100);
        }

        internal static ShipUpgradeConfig Default => new ShipUpgradeConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.WriteNullable(PreviousUpgrade is null ? null : (byte)PreviousUpgrade.ID);
            writer.Write2U(CostEnergy, 1);
            writer.Write2U(CostIon, 100);
            writer.Write2U(CostIron, 1);
            writer.Write2U(CostTungsten, 100);
            writer.Write2U(CostSilicon, 1);
            writer.Write2U(CostTritium, 10);
            writer.Write2U(CostTime, 10); // TODO MALUK read as uint16, written as double 2u
            writer.Write2U(Hull, 10);
            writer.Write2U(HullRepair, 100);
            writer.Write2U(Shields, 10);
            writer.Write2U(ShieldsLoad, 100);
            writer.Write3U(Size, 1000);
            writer.Write2S(Weight, 10000);
            writer.Write4U(EnergyMax, 10);
            writer.Write2U(EnergyCells, 100);
            writer.Write2U(EnergyReactor, 100);
            writer.Write2U(EnergyTransfer, 100);
            writer.Write2U(IonMax, 100);
            writer.Write2U(IonCells, 100);
            writer.Write2U(IonReactor, 1000);
            writer.Write2U(IonTransfer, 1000);
            writer.Write2U(Thruster, 10000);
            writer.Write2U(Nozzle, 100);
            writer.Write2U(Speed, 100);
            writer.Write2U(Turnrate, 100);
            writer.Write4U(Cargo, 1000);
            writer.Write2U(Extractor, 100);
            writer.Write2U(WeaponSpeed, 10);
            writer.Write((ushort)WeaponTime);
            writer.Write3U(WeaponLoad, 1000);
            writer.Write(WeaponAmmo);
            writer.Write2U(WeaponAmmoProduction, 100000);
            writer.Write(FreeSpawn);
            writer.Write4U(NozzleEnergyConsumption, 1000);
            writer.Write4U(ThrusterEnergyConsumption, 1000);
            writer.Write4U(HullRepairEnergyConsumption, 1000);
            writer.Write4U(HullRepairIronConsumption, 1000);
            writer.Write4U(ShieldsIonConsumption, 1000);
            writer.Write4U(ExtractorEnergyConsumption, 1000);
            writer.Write4U(WeaponEnergyConsumption, 1000);
            writer.Write4U(ScannerEnergyConsumption, 1000);
            writer.Write3U(ScannerRange, 1000);
            writer.Write2U(ScannerWidth, 100);
        }
    }
}
