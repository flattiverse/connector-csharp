using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class BlackholeConfiguration : CelestialBodyConfiguration
    {
        private BlackholeConfiguration() : base() { }

        internal BlackholeConfiguration(PacketReader reader) : base(reader)
        {
            //TODO
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            //TODO
        }

        public override UnitKind Kind => UnitKind.BlackHole;

        internal static BlackholeConfiguration Default => new BlackholeConfiguration();
    }
}
