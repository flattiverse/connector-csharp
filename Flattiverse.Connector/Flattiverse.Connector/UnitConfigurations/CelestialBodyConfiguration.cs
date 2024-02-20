using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class CelestialBodyConfiguration : Configuration
    {
        private Vector position;

        private double radius;

        private double gravity;

        protected internal CelestialBodyConfiguration() : base() { }

        internal CelestialBodyConfiguration(PacketReader reader) : base(reader)
        {
            position = new Vector(reader);

            radius = reader.Read4U(100);

            // JAM TODO: Die Anzahl der Digits für verschiedene Wertetypen müssen in der PROTOCOL.md dokumentiert werden.

            gravity = reader.Read4S(10000);

            // This is a reserve for orbiting units.
            reader.ReadByte();
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            position.Write(writer);

            writer.Write4U(radius, 100);
            writer.Write4S(gravity, 10000);

            // No orbiting configuration.
            writer.Write((byte)0);
        }

        public Vector Position
        {
            get => new Vector(position);
            set
            {
                if (value is null || value.IsDamaged || value.X < -20000 || value.Y < -20000 || value.X > 20000 || value.Y > 20000)
                    throw new GameException(0x31);

                position = new Vector(value);
            }
        }

        public double Radius
        {
            get => radius;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < 0.001 || value > 2000.0)
                    throw new GameException(0x31);

                radius = value;
            }
        }

        public double Gravity
        {
            get => gravity;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < 30.0 || value > 30.0)//TODO: realisitsche Min und Max-Werte setzen:
                    throw new GameException(0x31);

                radius = value;
            }
        }

        internal new static CelestialBodyConfiguration Default => new CelestialBodyConfiguration();
    }
}
