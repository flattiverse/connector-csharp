using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flattiverse.Connector.Units
{
    class Upgrade
    {
        public readonly Galaxy Galaxy;
        public readonly Ship Ship;
        public readonly byte ID;

        public readonly Upgrade? PreviousUpgrade;

        public readonly string Name;
        public readonly double CostEnergy;
        public readonly double CostIon;
        public readonly double CostIron;
        public readonly double CostTungsten;
        public readonly double CostSilicon;
        public readonly double CostTritium;
        public readonly double CostTime;
        public readonly double Hull;
        public readonly double HullRepair;
        public readonly double Shields;
        public readonly double ShieldsRepair;
        public readonly double Size;
        public readonly double Weight;
        public readonly double EnergyMax;
        public readonly double EnergyCells;
        public readonly double EnergyReactor;
        public readonly double EnergyTransfer;
        public readonly double IonMax;
        public readonly double IonCells;
        public readonly double IonReactor;
        public readonly double IonTransfer;
        public readonly double Thruster;
        public readonly double Nozzle;
        public readonly double Speed;
        public readonly double Turnrate;
        public readonly double Cargo;
        public readonly double Extractor;
        public readonly double WeaponSpeed;
        public readonly double WeaponTime;
        public readonly double WeaponLoad;

        public Upgrade(byte id, Galaxy galaxy, Ship ship, PacketReader reader)
        {
            ID = id;
            Galaxy = galaxy;

            PreviousUpgrade = reader.ReadNullableByte();

            writer.WriteNullable(PreviousUpgrade?.ID);
            writer.Write(Name);
            writer.Write4S(CostEnergy, 2);
            writer.Write4S(CostIon, 2);
            writer.Write4S(CostIron, 2);
            writer.Write4S(CostTungsten, 2);
            writer.Write4S(CostSilicon, 2);
            writer.Write4S(CostTritium, 2);
            writer.Write4S(CostTime, 2);
            writer.Write4S(Hull, 2);
            writer.Write4S(HullRepair, 2);
            writer.Write4S(Shields, 2);
            writer.Write4S(ShieldsRepair, 2);
            writer.Write4S(Size, 2);
            writer.Write4S(Weight, 2);
            writer.Write4S(EnergyMax, 2);
            writer.Write4S(EnergyCells, 2);
            writer.Write4S(EnergyReactor, 2);
            writer.Write4S(EnergyTransfer, 2);
            writer.Write4S(IonMax, 2);
            writer.Write4S(IonCells, 2);
            writer.Write4S(IonReactor, 2);
            writer.Write4S(IonTransfer, 2);
            writer.Write4S(Thruster, 2);
            writer.Write4S(Nozzle, 2);
            writer.Write4S(Speed, 2);
            writer.Write4S(Turnrate, 2);
            writer.Write4S(Cargo, 2);
            writer.Write4S(Extractor, 2);
            writer.Write4S(WeaponSpeed, 2);
            writer.Write4S(WeaponTime, 2);
            writer.Write4S(WeaponLoad, 2);
        }
    }
}
