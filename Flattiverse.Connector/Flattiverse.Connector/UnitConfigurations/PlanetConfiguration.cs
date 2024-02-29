using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.UnitConfigurations
{
    /// <summary>
    /// The configuration of a planet.
    /// </summary>
    public class PlanetConfiguration : HarvestableConfiguration
    {
        private PlanetConfiguration() : base() { }

        internal PlanetConfiguration(PacketReader reader) : base(reader)
        {
            //TODO
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            //TODO
        }

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.Planet;

        internal static PlanetConfiguration Default => new PlanetConfiguration();
    }
}
