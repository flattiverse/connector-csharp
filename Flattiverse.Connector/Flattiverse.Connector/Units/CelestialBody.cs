using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using System.Diagnostics.Metrics;

namespace Flattiverse.Connector.Units
{
    public class CelestialBody : Unit
    {
        private Vector position;

        private double radius;

        private double gravity;

        private readonly Cluster cluster;

        internal CelestialBody(Cluster cluster, PacketReader reader) : base(reader)
        {
            this.cluster = cluster;

            position = new Vector(reader);
            radius = reader.ReadDouble();
            gravity = reader.ReadDouble();

            // This is a reserve for orbiting units.
            reader.ReadByte();
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);
            
            position = new Vector(reader);
            radius = reader.ReadDouble();
            gravity = reader.ReadDouble();

            // This is a reserve for orbiting units.
            reader.ReadByte();
        }

        public override Cluster Cluster => cluster;

        public override double Gravity => gravity;

        public override double Radius => radius;

        public override Vector Position => position;

        public override Mobility Mobility => Mobility.Still;
    }
}
