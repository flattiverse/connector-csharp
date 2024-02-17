using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class MeteoroidConfiguration : CelestialBodyConfiguration
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

        public override UnitKind Kind => UnitKind.Meteoroid;

        internal static MeteoroidConfiguration Default => new MeteoroidConfiguration();
    }
}
