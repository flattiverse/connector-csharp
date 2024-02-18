using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class BuoyConfiguration : CelestialBodyConfiguration
    {
        private BuoyConfiguration() : base() { }

        internal BuoyConfiguration(PacketReader reader) : base(reader)
        {
            //TODO
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            //TODO
        }

        public override UnitKind Kind => UnitKind.Buoy;

        internal static BuoyConfiguration Default => new BuoyConfiguration();
    }
}
