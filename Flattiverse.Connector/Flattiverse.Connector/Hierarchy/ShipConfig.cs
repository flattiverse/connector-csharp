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
    public class ShipConfig
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
        public bool FreeSpawn;

        private ShipConfig()
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
            FreeSpawn = true;
        }

        public ShipConfig(Ship ship)
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
            FreeSpawn = ship.FreeSpawn;
        }

        internal static ShipConfig Default => new ShipConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
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
