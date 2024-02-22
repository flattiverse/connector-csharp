using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ShipDesignConfig
    {
        public string Name;
        public double CostEnergy;
        public double CostIon;
        public double CostIron;
        public double CostTungsten;
        public double CostSilicon;
        public double CostTritium;
        public double CostTime;
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
        public double WeaponTime;
        public double WeaponLoad;
        public ushort WeaponAmmo;
        public double WeaponAmmoProduction;
        public bool FreeSpawn;

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
            Size = ship.Size;
            Weight = ship.Weight;
            EnergyMax = ship.EnergyMax;
            EnergyCells = ship.EnergyCells;
            EnergyReactor = ship.EnergyReactor;
            EnergyTransfer = ship.EnergyTransfer;
            IonMax = ship.IonMax;
            IonCells = ship.IonCells;
            IonReactor = ship.IonReactor;
            IonTransfer = ship.IonTransfer;
            Thruster = ship.Thruster;
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
        }

        internal ShipDesignConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            CostEnergy = reader.Read4U(1000);
            CostIon = reader.Read4U(1000);
            CostIron = reader.Read4U(1000);
            CostTungsten = reader.Read4U(1000);
            CostSilicon = reader.Read4U(1000);
            CostTritium = reader.Read4U(1000);
            CostTime = reader.Read2U(10);
            Hull = reader.Read3U(10000);
            HullRepair = reader.Read3U(10000);
            Shields = reader.Read3U(10000);
            ShieldsLoad = reader.Read3U(10000);
            Size = reader.Read3U(1000);
            Weight = reader.Read2S(10000);
            EnergyMax = reader.Read4U(1000);
            EnergyCells = reader.Read4U(1000);
            EnergyReactor = reader.Read4U(1000);
            EnergyTransfer = reader.Read4U(1000);
            IonMax = reader.Read4U(1000);
            IonCells = reader.Read4U(1000);
            IonReactor = reader.Read4U(1000);
            IonTransfer = reader.Read4U(1000);
            Thruster = reader.Read2U(10000);
            Nozzle = reader.Read2U(100);
            Speed = reader.Read2U(1000);
            Turnrate = reader.Read2U(100);
            Cargo = reader.Read4U(1000);
            Extractor = reader.Read4U(1000);
            WeaponSpeed = reader.Read2U(1000);
            WeaponTime = reader.ReadUInt16();
            WeaponLoad = reader.Read3U(1000);
            WeaponAmmo = reader.ReadUInt16();
            WeaponAmmoProduction = reader.Read4U(1000);
            FreeSpawn = reader.ReadBoolean();
        }

        internal static ShipDesignConfig Default => new ShipDesignConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write4U(CostEnergy, 1000);
            writer.Write4U(CostIon, 1000);
            writer.Write4U(CostIron, 1000);
            writer.Write4U(CostTungsten, 1000);
            writer.Write4U(CostSilicon, 1000);
            writer.Write4U(CostTritium, 1000);
            writer.Write2U(CostTime, 10);
            writer.Write3U(Hull, 10000);
            writer.Write3U(HullRepair, 10000);
            writer.Write3U(Shields, 1000);
            writer.Write3U(ShieldsLoad, 10000);
            writer.Write3U(Size, 1000);
            writer.Write2S(Weight, 10000);
            writer.Write4U(EnergyMax, 1000);
            writer.Write4U(EnergyCells, 1000);
            writer.Write4U(EnergyReactor, 1000);
            writer.Write4U(EnergyTransfer, 1000);
            writer.Write4U(IonMax, 1000);
            writer.Write4U(IonCells, 1000);
            writer.Write4U(IonReactor, 1000);
            writer.Write4U(IonTransfer, 1000);
            writer.Write2U(Thruster, 10000);
            writer.Write2U(Nozzle, 100);
            writer.Write2U(Speed, 1000);
            writer.Write2U(Turnrate, 100);
            writer.Write4U(Cargo, 1000);
            writer.Write4U(Extractor, 1000);
            writer.Write2U(WeaponSpeed, 1000);
            writer.Write((ushort)WeaponTime);
            writer.Write3U(WeaponLoad, 1000);
            writer.Write(WeaponAmmo);//TODO: Is that correct?
            writer.Write4U(WeaponAmmoProduction, 1000);
            writer.Write(FreeSpawn);
        }
    }
}
