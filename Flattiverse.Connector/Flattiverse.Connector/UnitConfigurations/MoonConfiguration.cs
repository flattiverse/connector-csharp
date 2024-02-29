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
    /// The configuration of a moon.
    /// </summary>
    public class MoonConfiguration : HarvestableConfiguration
    {
        internal MoonConfiguration() : base() { }

        internal MoonConfiguration(PacketReader reader) : base(reader)
        {
            //TODO
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            //TODO
        }

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.Moon;

        internal static MoonConfiguration Default => new MoonConfiguration();
    }
}
