using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class RegionConfig
    {
        public string Name;
        public double StartPropability;
        public double RespawnPropability;
        public bool Protected;
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        private RegionConfig()
        {
            Name = string.Empty;
            StartPropability = 0;
            RespawnPropability = 0;
            Protected = false;
            Left = 0;
            Top = 0;
            Right = 0;
            Bottom = 0;
        }

        internal RegionConfig(RegionConfig region)
        {
            Name = region.Name;
            StartPropability = region.StartPropability;
            RespawnPropability = region.RespawnPropability;
            Protected = region.Protected;
            Left = region.Left;
            Top = region.Top;
            Right = region.Right;
            Bottom = region.Bottom;
        }

        internal RegionConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            StartPropability = reader.Read2U(100);
            RespawnPropability = reader.Read2U(100);
            Protected = reader.ReadBoolean();
            Left = reader.Read2U(100);
            Top = reader.Read2U(100);
            Right = reader.Read2U(100);
            Bottom = reader.Read2U(100);
        }

        internal static RegionConfig Default => new RegionConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write2U(StartPropability, 100);
            writer.Write2U(RespawnPropability, 100);
            writer.Write(Protected);
            writer.Write2U(Left, 100);
            writer.Write2U(Top, 100);
            writer.Write2U(Right, 100);
            writer.Write2U(Bottom, 100);
        }
    }
}
