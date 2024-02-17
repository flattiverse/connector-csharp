using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units
{
    public class CelestialBody : Unit
    {
        private Vector position;

        private double radius;

        private double gravity;

        internal CelestialBody(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            position = new Vector(reader);
            
            radius = reader.Read4U(100);
            gravity = reader.Read4U(10000);
        }

        public override double Gravity => gravity;

        public override double Radius => radius;

        public override Vector Position => position;

        public override Mobility Mobility => Mobility.Still;
    }
}
