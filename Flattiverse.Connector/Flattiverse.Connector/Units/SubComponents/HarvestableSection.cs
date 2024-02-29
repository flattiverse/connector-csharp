using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units.SubComponents
{
    /// <summary>
    /// Players within these sections can collect resources based on its Configuration.
    /// It may contain all 4 types of collactaqble resources being Iron, Silicon, Tungsten and Tritium.
    /// </summary>
    /// <remarks>
    /// The collection speed of these resources is based on your ShipConfiguration and your upgrades.
    /// Doing so will consume energy.
    /// </remarks>
    public class HarvestableSection
    {
        private double innerRadius;
        private double outerRadius;
        private double angleFrom;
        private double angleTo;

        private double iron;
        private double silicon;
        private double tungsten;
        private double tritium;

        private readonly HarvestableConfiguration? Configuration;

        internal HarvestableSection(HarvestableConfiguration configuration)
        {
            Configuration = configuration;

            outerRadius = 130;
            innerRadius = 100;

            angleFrom = 45;
            angleTo = 135;

            iron = 20;
            silicon = 6;
            tungsten = 0;
            tritium = 0;
        }

        internal HarvestableSection(HarvestableConfiguration? configuration, PacketReader reader)
        {
            Configuration = configuration;

            InnerRadius = reader.ReadDouble();
            OuterRadius = reader.ReadDouble();
            AngleFrom = reader.ReadDouble();
            AngleTo = reader.ReadDouble();

            // TODO: MALUK Werte anpassen
            Iron = reader.ReadDouble();
            Silicon = reader.ReadDouble();
            Tungsten = reader.ReadDouble();
            Tritium = reader.ReadDouble();
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write(InnerRadius);
            writer.Write(OuterRadius);
            writer.Write(AngleFrom);
            writer.Write(AngleTo);

            // TODO: MALUK Werte anpassen
            writer.Write(Iron);
            writer.Write(Silicon);
            writer.Write(Tungsten);
            writer.Write(Tritium);
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

            if (double.IsInfinity(outer) || double.IsNaN(outer) || outer > 2000.0)//TODO: inconsistent: OuterRadius-Setter checks if outerRadius is greater than 1000
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

            if (double.IsInfinity(to) || double.IsNaN(to) || to > 360.0 || to < 0.0)
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
        /// The iron output in this area. This value multiplied with extractor results in the iron loaded per Second. 
        /// </summary>
        public double Iron
        {
            get => iron;
            set
            {
                // TODO: MALUK Werte anpassen
                if (double.IsInfinity(value) || double.IsNaN(value) || iron > 500.0 || iron < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                iron = value;
            }
        }

        /// <summary>
        /// The silicon output in this area. This value multiplied with extractor results in the silicon loaded per Second. 
        /// </summary>
        public double Silicon
        {
            get => silicon;
            set
            {
                // TODO: MALUK Werte anpassen
                if (double.IsInfinity(value) || double.IsNaN(value) || silicon > 500.0 || silicon < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                silicon = value;
            }
        }

        /// <summary>
        /// The tungsten output in this area. This value multiplied with extractor results in the tungsten loaded per Second. 
        /// </summary>
        public double Tungsten
        {
            get => tungsten;
            set
            {
                // TODO: MALUK Werte anpassen
                if (double.IsInfinity(value) || double.IsNaN(value) || tungsten > 500.0 || tungsten < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                tungsten = value;
            }
        }

        /// <summary>
        /// The tritium output in this area. This value multiplied with extractor results in the tritium loaded per Second. 
        /// </summary>
        public double Tritium
        {
            get => tritium;
            set
            {
                // TODO: MALUK Werte anpassen
                if (double.IsInfinity(value) || double.IsNaN(value) || tritium > 500.0 || tritium < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                tritium = value;
            }
        }
    }
}
