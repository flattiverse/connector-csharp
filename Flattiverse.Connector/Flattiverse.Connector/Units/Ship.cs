using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    class Ship
    {
        public readonly Galaxy Galaxy;
        public readonly byte ID;

        internal readonly Upgrade?[] upgrades = new Upgrade?[256];
        internal int upgradeMax = 0;

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

        public Ship(byte id, Galaxy galaxy, PacketReader reader)
        {

        }

        internal void ReadUpgrade(byte id, PacketReader reader)
        {
            upgrades[id] = new Upgrade(id, Galaxy, this, reader);

            if (upgradeMax < id + 1)
                upgradeMax = id + 1;
        }
    }
}
