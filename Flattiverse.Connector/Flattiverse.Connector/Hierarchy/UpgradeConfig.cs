using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public bool FreeSpawn;

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
            FreeSpawn = true;
        }

        public UpgradeConfig(Upgrade upgrade)
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
            FreeSpawn = upgrade.FreeSpawn;
        }

        internal static UpgradeConfig Default => new UpgradeConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.WriteNullable(PreviousUpgrade?.id);
            writer.Write2U(CostEnergy, 1);
            writer.Write2U(CostIon, 100);
            writer.Write2U(CostIron, 1);
            writer.Write2U(CostTungsten, 100);
            writer.Write2U(CostSilicon, 1);
            writer.Write2U(CostTritium, 10);
            writer.Write2U(CostTime, 10);
            writer.Write2U(Hull, 10);
            writer.Write2U(HullRepair, 100);
            writer.Write2U(Shields, 10);
            writer.Write2U(ShieldsLoad, 100);
            writer.Write2U(Size, 10);
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
            writer.Write2U(WeaponLoad, 10);
            writer.Write(FreeSpawn);
        }
    }
}
