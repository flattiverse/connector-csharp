using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class RegionConfig
    {
        public string Name;
        public double StartPropability;
        public double RespawnPropability;
        public bool Protected;

        private RegionConfig()
        {
            Name = string.Empty;
            StartPropability = 0;
            RespawnPropability = 0;
            Protected = false;
        }

        internal RegionConfig(RegionConfig region)
        {
            Name = region.Name;
            StartPropability = region.StartPropability;
            RespawnPropability = region.RespawnPropability;
            Protected = region.Protected;
        }

        internal RegionConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            StartPropability = reader.Read2U(100);
            RespawnPropability = reader.Read2U(100);
            Protected = reader.ReadBoolean();
        }

        internal static RegionConfig Default => new RegionConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write2U(StartPropability, 100);
            writer.Write2U(RespawnPropability, 100);
            writer.Write(Protected);
        }
    }
}
