using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
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
        public readonly ushort WeaponTime;
        public readonly double WeaponLoad;

        public Ship(byte id, Galaxy galaxy, PacketReader reader)
        {
            ID = id;
            Galaxy = galaxy;

            Name = reader.ReadString();
            CostEnergy = reader.Read2U(1);
            CostIon = reader.Read2U(100);
            CostIron = reader.Read2U(1);
            CostTungsten = reader.Read2U(100);
            CostSilicon = reader.Read2U(1);
            CostTritium = reader.Read2U(10);
            CostTime = reader.Read2U(10);
            Hull = reader.Read2U(10);
            HullRepair = reader.Read2U(100);
            Shields = reader.Read2U(10);
            ShieldsRepair = reader.Read2U(100);
            Size = reader.Read2U(10);
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
            WeaponLoad = reader.Read2U(10);
        }

        internal void ReadUpgrade(byte id, PacketReader reader)
        {
            upgrades[id] = new Upgrade(id, Galaxy, this, reader);

            if (upgradeMax < id + 1)
                upgradeMax = id + 1;
        }
    }
}
