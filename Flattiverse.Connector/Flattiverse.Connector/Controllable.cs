using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector
{
    public class Controllable
    {
        private Cluster cluster;

        public readonly string Name;

        private double hull;            // u16,10
        private double hullMax;         // u16,10
        private double hullRepair;      // u16,100
        private double shields;         // u16,10
        private double shieldsMax;      // u16,10
        private double shieldsLoad;     // u16,100
        private double size;            // u16,10
        private double weight;          // s16,10000
        private double energy;          // u32,10
        private double energyMax;       // u32,10
        private double energyCells;     // u16,100
        private double energyReactor;   // u16,100
        private double energyTransfer;  // u16,100
        private double ion;             // u16,100
        private double ionMax;          // u16,100
        private double ionCells;        // u16,100
        private double ionReactor;      // u16,1000
        private double ionTransfer;     // u16,1000
        private double thruster;        // u16,10000
        private double thrusterMax;     // u16,10000
        private double nozzle;          // u16,100
        private double nozzleMax;       // u16,100
        private double speed;           // u16,100
        private double speedMax;         // u16,100
        private double turnrate;        // u16,100
        private double cargo;           // u24,1000
        private double cargoMax;        // u24,1000
        private double extractor;       // u16,100
        private double weaponSpeed;     // u16,10
        private ushort weaponTime;      // u8   [Ticks]
        private double weaponLoad;      // u16,10

        internal Controllable(Cluster cluster, PacketReader reader)
        {
            Name = reader.ReadString();

            hull = reader.Read2U(10);
            hullMax = reader.Read2U(10);
            hullRepair = reader.Read2U(100);
            shields = reader.Read2U(10);
            shieldsMax = reader.Read2U(10);
            shieldsLoad = reader.Read2U(100);
            size = reader.Read2U(10);
            weight = reader.Read2S(10000);
            energy = reader.Read4U(10);
            energyMax = reader.Read4U(10);
            energyCells = reader.Read2U(100);
            energyReactor = reader.Read2U(100);
            energyTransfer = reader.Read2U(100);
            ion = reader.Read2U(100);
            ionMax = reader.Read2U(100);
            ionCells = reader.Read2U(100);
            ionReactor = reader.Read2U(1000);
            ionTransfer = reader.Read2U(1000);
            thruster = reader.Read2U(10000);
            thrusterMax = reader.Read2U(10000);
            nozzle = reader.Read2U(100);
            nozzleMax = reader.Read2U(100);
            speed = reader.Read2U(100);
            speedMax = reader.Read2U(100);
            turnrate = reader.Read2U(100);
            cargo = reader.Read4U(1000);
            cargoMax = reader.Read4U(1000);
            extractor = reader.Read2U(100);
            weaponSpeed = reader.Read2U(10);
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.Read2U(10);
        }
    }
}
