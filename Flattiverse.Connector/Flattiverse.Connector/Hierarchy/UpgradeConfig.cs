using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Hierarchy
{
    public class UpgradeConfig
    {
        public string Name;
        public Upgrade? PreviousUpgrade;
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

        private UpgradeConfig()
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

        internal UpgradeConfig(UpgradeConfig upgrade)
        {
            Name = upgrade.Name;
            PreviousUpgrade = upgrade.PreviousUpgrade;
            CostEnergy = upgrade.CostEnergy;
            CostIon = upgrade.CostIon;
            CostIron = upgrade.CostIron;
            CostTungsten = upgrade.CostTungsten;
            CostSilicon = upgrade.CostSilicon;
            CostTritium = upgrade.CostTritium;
            CostTime = upgrade.CostTime;
            Hull = upgrade.Hull;
            HullRepair = upgrade.HullRepair;
            Shields = upgrade.Shields;
            ShieldsLoad = upgrade.ShieldsLoad;
            Size = upgrade.Size;
            Weight = upgrade.Weight;
            EnergyMax = upgrade.EnergyMax;
            EnergyCells = upgrade.EnergyCells;
            EnergyReactor = upgrade.EnergyReactor;
            EnergyTransfer = upgrade.EnergyTransfer;
            IonMax = upgrade.IonMax;
            IonCells = upgrade.IonCells;
            IonReactor = upgrade.IonReactor;
            IonTransfer = upgrade.IonTransfer;
            Thruster = upgrade.Thruster;
            Nozzle = upgrade.Nozzle;
            Speed = upgrade.Speed;
            Turnrate = upgrade.Turnrate;
            Cargo = upgrade.Cargo;
            Extractor = upgrade.Extractor;
            WeaponSpeed = upgrade.WeaponSpeed;
            WeaponTime = upgrade.WeaponTime;
            WeaponLoad = upgrade.WeaponLoad;
            WeaponAmmo = upgrade.WeaponAmmo;
            WeaponAmmoProduction = upgrade.WeaponAmmoProduction;
            FreeSpawn = upgrade.FreeSpawn;
            NozzleEnergyConsumption = upgrade.NozzleEnergyConsumption;
            ThrusterEnergyConsumption = upgrade.ThrusterEnergyConsumption;
            HullRepairEnergyConsumption = upgrade.HullRepairEnergyConsumption;
            HullRepairIronConsumption = upgrade.HullRepairIronConsumption;
            ShieldsIonConsumption = upgrade.ShieldsIonConsumption;
            ExtractorEnergyConsumption = upgrade.ExtractorEnergyConsumption;
            WeaponEnergyConsumption = upgrade.WeaponEnergyConsumption;
            ScannerEnergyConsumption = upgrade.ScannerEnergyConsumption;
            ScannerRange = upgrade.ScannerRange;
            ScannerWidth = upgrade.ScannerWidth;
        }

        internal UpgradeConfig(PacketReader reader, ShipDesign ship)
        {
            Name = reader.ReadString();

            if (reader.ReadNullableByte() is byte previousUpgradeId && ship.upgrades[previousUpgradeId] is Upgrade previousUpgrade)
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
            ScannerWidth = reader.Read3U(1000);
        }

        internal static UpgradeConfig Default => new UpgradeConfig();

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
            writer.Write3U(ScannerWidth, 1000);
        }
    }
}
