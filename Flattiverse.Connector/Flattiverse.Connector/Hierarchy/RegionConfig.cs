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
        public uint Team; // TODO MALUK TeamId is usually 1 byte

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
            Bottom = uint.MaxValue; // TODO MALUK why is Bottom assign twice?
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
            Bottom = region.Bottom; // TODO MALUK why is Bottom assign twice?
        }

        internal RegionConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            StartPropability = reader.ReadDouble();
            RespawnPropability = reader.ReadDouble();
            Protected = reader.ReadBoolean();
            // TODO JUW: Weil es sich hier um Koordinaten handelt müsste das natürlich auch mittels derer Genauigkeit übertragen werden. Bitte im Connector und Server anpassen.
            Left = reader.ReadDouble();
            Top = reader.ReadDouble();
            Right = reader.ReadDouble();
            Bottom = reader.ReadDouble();
            Team = reader.ReadUInt32();
        }

        internal static RegionConfig Default => new RegionConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write(StartPropability);
            writer.Write(RespawnPropability);
            writer.Write(Protected);
            writer.Write(Left);
            writer.Write(Top);
            writer.Write(Right);
            writer.Write(Bottom);
            writer.Write(Team);
        }
    }
}
