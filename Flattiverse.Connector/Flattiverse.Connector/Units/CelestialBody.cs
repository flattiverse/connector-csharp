using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units
{
    public class CelestialBody : Unit
    {
        private Vector position;

        private double radius;

        private double gravity;

        internal CelestialBody(CelestialBodyConfiguration configuration) : base(configuration)
        {
            position = configuration.Position;
            radius = configuration.Radius;
            gravity = configuration.Gravity;
        }

        public override double Gravity => gravity;

        public override double Radius => radius;

        public override Vector Position => position;

        public override Mobility Mobility => Mobility.Still;
    }
}
