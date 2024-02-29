using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// This is a contains the configuration parameters of a region.
    /// </summary>
    /// <see cref="Region" />
    public class RegionConfig
    {
        private string name;

        /// <summary>
        /// The probability between 0 and 1 that a unit will spawn in this region when continue is called.
        /// </summary>
        public double StartPropability;

        /// <summary>
        /// Unused.
        /// </summary>
        public double RespawnPropability;

        /// <summary>
        /// Unimplemented.
        /// </summary>
        public bool Protected;

        /// <summary>
        /// The left border of the region.
        /// </summary>
        public double Left;

        /// <summary>
        /// The top border of the region.
        /// </summary>
        public double Top;

        /// <summary>
        /// The right border of the region.
        /// </summary>
        public double Right;

        /// <summary>
        /// The bottom border of the region.
        /// </summary>
        public double Bottom;

        /// <summary>
        /// The team the region belongs to.
        /// </summary>
        public uint Team; // TODO MALUK TeamId is usually 1 byte

        /// <summary>
        /// The name of the configured unit.
        /// </summary>
        /// <exception cref="GameException">0x32 may be thrown, if the name violates rules.</exception>
        public string Name
        {
            get => name;
            set
            {
                if (!Utils.CheckName64(value))
                    throw new GameException(0x31);

                name = value;
            }
        }

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
