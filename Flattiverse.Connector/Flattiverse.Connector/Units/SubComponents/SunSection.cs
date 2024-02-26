﻿using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units.SubComponents
{
    public class SunSection
    {
        private double innerRadius;
        private double outerRadius;
        private double angelFrom;
        private double angelTo;

        private double energy;
        private double ions;

        private readonly SunConfiguration? Configuration;

        internal SunSection(SunConfiguration configuration)
        {
            Configuration = configuration;

            OuterRadius = 130;
            InnerRadius = 100;

            AngelFrom = 45;
            AngelTo = 135;

            Energy = 4;
            Ions = 0;
        }

        internal SunSection(SunConfiguration? configuration, PacketReader reader)
        {
            Configuration = configuration;

            outerRadius = reader.ReadDouble();
            innerRadius = reader.ReadDouble();
            angelFrom = reader.ReadDouble();
            angelTo = reader.ReadDouble();

            energy = reader.ReadDouble();
            ions = reader.ReadDouble();
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write(innerRadius);
            writer.Write(outerRadius);
            writer.Write(angelFrom);
            writer.Write(angelTo);

            writer.Write(energy);
            writer.Write(ions);
        }

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

            if (double.IsInfinity(outer) || double.IsNaN(outer) || outer > 2000.0)//TODO: inconsistent OuterRadius-Setter checks if outerRadius is greater than 1000
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
        public void SetAngels(double from, double to)
        {
            // TODO MALUK BlackHoleSection and HarvestableSection had here an additional '|| from == to' condition and are structured differently 
            if (double.IsInfinity(from) || double.IsNaN(from) || from < 0.0 || to > 360.0)
                throw new GameException(0x31);

            // TODO MALUK second half is the same condition as above
            if (double.IsInfinity(to) || double.IsNaN(to) || from < 0.0 || to > 360.0)
                throw new GameException(0x31);

            if (Configuration is null)
                throw new GameException(0x34);

            angelFrom = from;
            angelTo = to;
        }

        /// <summary>
        /// The left angle, when you look from the middle point of the sun to the section.
        /// </summary>
        public double AngelFrom
        {
            get => angelFrom;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value < 0.0 || value > 360.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                angelFrom = value;
            }
        }

        /// <summary>
        /// The right angle, when you look from the middle point of the sun to the section.
        /// </summary>
        public double AngelTo
        {
            get => angelTo;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || value > 360.0 || value < 0.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                angelTo = value;
            }
        }

        /// <summary>
        /// The energy output in this corona. This value multiplied with EnergyCells results in the energy loaded per Second. 
        /// </summary>
        public double Energy
        {
            get => energy;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || energy > 500.0 || energy < -500.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                energy = value;
            }
        }

        /// <summary>
        /// The ions output in this corona. This value multiplied with IonCells results in the ions loaded per Second. 
        /// </summary>
        public double Ions
        {
            get => ions;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || ions > 50.0 || ions < -50.0)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                ions = value;
            }
        }
    }
}
