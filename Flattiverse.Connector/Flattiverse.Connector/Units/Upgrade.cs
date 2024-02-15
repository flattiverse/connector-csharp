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
            Ship = ship;

            if (reader.ReadNullableByte() is byte previousUpgradeId && ship.upgrades[previousUpgradeId] is Upgrade previousUpgrade)
                PreviousUpgrade = previousUpgrade;

            Name = reader.ReadString();
            CostEnergy = reader.Read4S(2);
            CostIon = reader.Read4S(2);
            CostIron = reader.Read4S(2);
            CostTungsten = reader.Read4S(2);
            CostSilicon = reader.Read4S(2);
            CostTritium = reader.Read4S(2);
            CostTime = reader.Read4S(2);
            Hull = reader.Read4S(2);
            HullRepair = reader.Read4S(2);
            Shields = reader.Read4S(2);
            ShieldsRepair = reader.Read4S(2);
            Size = reader.Read4S(2);
            Weight = reader.Read4S(2);
            EnergyMax = reader.Read4S(2);
            EnergyCells = reader.Read4S(2);
            EnergyReactor = reader.Read4S(2);
            EnergyTransfer = reader.Read4S(2);
            IonMax = reader.Read4S(2);
            IonCells = reader.Read4S(2);
            IonReactor = reader.Read4S(2);
            IonTransfer = reader.Read4S(2);
            Thruster = reader.Read4S(2);
            Nozzle = reader.Read4S(2);
            Speed = reader.Read4S(2);
            Turnrate = reader.Read4S(2);
            Cargo = reader.Read4S(2);
            Extractor = reader.Read4S(2);
            WeaponSpeed = reader.Read4S(2);
            WeaponTime = reader.Read4S(2);
            WeaponLoad = reader.Read4S(2);
        }
    }
}
