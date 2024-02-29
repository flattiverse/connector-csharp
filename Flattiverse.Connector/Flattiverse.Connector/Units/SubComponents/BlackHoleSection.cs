using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units.SubComponents
{
    /// <summary>
    /// A section of a black hole and are central to the black hole's position.
    /// Players in this section will experience additional gravity.
    /// </summary>
    /// <remarks>
    /// This can be used as a slingshot to accelerate your units.
    /// </remarks>
    public class BlackHoleSection
    {
        private double innerRadius;
        private double outerRadius;
        private double angleFrom;
        private double angleTo;

        private double additionalGravity;

        private readonly BlackHoleConfiguration? Configuration;

        internal BlackHoleSection(BlackHoleConfiguration configuration)
        {
            Configuration = configuration;

            outerRadius = 130;
            innerRadius = 100;

            angleFrom = 45;
            angleTo = 135;
            additionalGravity = 0.03;
        }

        internal BlackHoleSection(BlackHoleConfiguration? configuration, PacketReader reader)
        {
            Configuration = configuration;

            innerRadius = reader.ReadDouble();
            outerRadius = reader.ReadDouble();
            angleFrom = reader.ReadDouble();
            angleTo = reader.ReadDouble();
            
            additionalGravity = reader.ReadDouble();
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write(innerRadius);
            writer.Write(outerRadius);
            writer.Write(angleFrom);
            writer.Write(angleTo);

            writer.Write(additionalGravity);
        }

        /// <summary>
        /// Removes this section from the configuration.
        /// </summary>
        /// <exception cref="GameException">Thrown, if you don't have permission to do this.</exception>
        public void Remove()
        {
            Configuration?.sections.Remove(this);
        }

        /// <summary>
        /// The inner radius (=radius which is nearer to the sun) of the section.
        /// </summary>
        public double InnerRadius
        {
            get => innerRadius;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < 0.0 || value >= outerRadius)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                innerRadius = value;
            }
        }

        /// <summary>
        /// The outer radius (=radius which is farer away to the sun) of the section.
        /// </summary>
        public double OuterRadius
        {
            get => outerRadius;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value > 1000.0 || innerRadius >= value)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                outerRadius = value;
            }
        }

        /// <summary>
        /// Sets the radius for the inner and the outer radius at once.
        /// </summary>
        /// <param name="inner">The inner radius.</param>
        /// <param name="outer">The outer radius.</param>
        /// <exception cref="GameException">Thrown, if you don't have permission to do this or the values are invalid.</exception>
        public void SetRadii(double inner, double outer)
        {
            if (double.IsInfinity(inner) || double.IsNaN(inner) || inner < 0.0 || inner >= outer)
                throw new GameException(0x31);

            if (double.IsInfinity(outer) || double.IsNaN(outer) || outer > 2000.0)//TODO: inconsistent: OuterRadius-Setter checks if it is bigger than 1000.0
                throw new GameException(0x31);

            if (Configuration is null)
                throw new GameException(0x34);

            innerRadius = inner;
            outerRadius = outer;
        }

        /// <summary>
        /// Sets the angle for the left (from) and the right (to) side at once.
        /// </summary>
        /// <param name="from">The from (left) radius.</param>
        /// <param name="to">The to (right) radius.</param>
        /// <exception cref="GameException">Thrown, if you don't have permission to do this or the values are invalid.</exception>
        public void SetAngles(double from, double to)
        {
            if (double.IsInfinity(from) || double.IsNaN(from) || from < 0.0 || from > 360.0 || from == to)
                throw new GameException(0x31);

            if (double.IsInfinity(to) || double.IsNaN(to) || to < 0.0 || to > 360.0)
                throw new GameException(0x31);

            if (Configuration is null)
                throw new GameException(0x34);

            angleFrom = from;
            angleTo = to;
        }

        /// <summary>
        /// The left angle, when you look from the middle point of the sun to the section.
        /// </summary>
        public double AngleFrom
        {
            get => angleFrom;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < 0.0 || value > 360.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                angleFrom = value;
            }
        }

        /// <summary>
        /// The right angle, when you look from the middle point of the sun to the section.
        /// </summary>
        public double AngleTo
        {
            get => angleTo;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value > 360.0 || value < 0.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                angleTo = value;
            }
        }

        /// <summary>
        /// The force by which the black hole pulls or pushes in this direction.
        /// </summary>
        public double AdditionalGravity
        {
            get => additionalGravity;
            set
            {
                // TODO: MALUK Werte anpassen
                if (double.IsInfinity(value) || double.IsNaN(value) || additionalGravity > 500.0 || additionalGravity < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                additionalGravity = value;
            }
        }
    }
}
