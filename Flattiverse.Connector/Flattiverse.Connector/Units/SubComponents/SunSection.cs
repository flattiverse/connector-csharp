using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;

namespace Flattiverse.Connector.Units.SubComponents
{

    public class SunSection
    {
        // JAM TODO: Das müssen im Connector Properties mit Exceptions mit entsprechenden Fehlermeldungen sein - zumindest im Connector. Hier Beispielhaft implementiert.
        // JAM TODO: Die Properties müssen Configuration honorieren. (Sollten nur geändert werden können, wenn sie sich in einem Konfigurations-Scenario befinden.)

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

            InnerRadius = 100;
            OuterRadius = 130;

            AngelFrom = 45;
            AngelTo = 135;

            Energy = 4;
            Ions = 0;
        }

        internal SunSection(SunConfiguration? configuration, PacketReader reader)
        {
            // JAM TODO: Hierfür (shift bei verschiedenen Werten) einheitliche Doku in PROTOCOL.md und inkonsistenzen finden und fixen.

            Configuration = configuration;

            InnerRadius = reader.Read2U(100);
            OuterRadius = reader.Read2U(100);
            AngelFrom = reader.Read2U(100);
            AngelTo = reader.Read2U(100);

            Energy = reader.Read2S(100);
            Ions = reader.Read2S(100);
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write2U(InnerRadius, 100);
            writer.Write2U(OuterRadius, 100);
            writer.Write2U(AngelFrom, 100);
            writer.Write2U(AngelTo, 100);

            writer.Write2S(Energy, 100);
            writer.Write2S(Ions, 100);
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
                if (double.IsInfinity(value) || double.IsNaN(value) || innerRadius < 0.0 || innerRadius >= outerRadius)
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
                if (double.IsInfinity(value) || double.IsNaN(value) || outerRadius < 0.0 || innerRadius >= outerRadius)
                    throw new GameException(0x31);

                if (Configuration is null)
                    throw new GameException(0x34);

                outerRadius = value;
            }
        }

        /// <summary>
        /// The left angle, when you look from the middle point of the sun to the section.
        /// </summary>
        public double AngelFrom
        {
            get => angelFrom;
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value) || angelFrom < 0.0 || angelFrom >= angelTo)
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
                if (double.IsInfinity(value) || double.IsNaN(value) || angelTo > 360.0 || angelFrom >= angelTo)
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
