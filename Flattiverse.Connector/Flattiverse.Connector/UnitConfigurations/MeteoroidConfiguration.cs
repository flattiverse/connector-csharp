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
    /// The configuration of a meteoroid.
    /// </summary>
    public class MeteoroidConfiguration : HarvestableConfiguration
    {
        private MeteoroidConfiguration() : base() { }

        internal MeteoroidConfiguration(PacketReader reader) : base(reader)
        {
            //TODO
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            //TODO
        }

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.Meteoroid;

        internal static MeteoroidConfiguration Default => new MeteoroidConfiguration();
    }
}
