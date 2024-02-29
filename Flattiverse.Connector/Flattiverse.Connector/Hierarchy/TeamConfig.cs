using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// The configuration used by a team.
    /// </summary>
    public class TeamConfig
    {
        private string name;

        /// <summary>
        /// The red color channel of the team.
        /// </summary>
        /// <remarks>
        /// The range is limited to 0-255
        /// </remarks>
        public int Red;

        /// <summary>
        /// The green color channel of the team.
        /// </summary>
        /// <remarks>
        /// The range is limited to 0-255
        /// </remarks>
        public int Green;

        /// <summary>
        /// The blue color channel of the team.
        /// </summary>
        /// <remarks>
        /// The range is limited to 0-255
        /// </remarks>
        public int Blue;

        /// <summary>
        /// The name of the configured unit.
        /// </summary>
        /// <exception cref="GameException">0x32 may be thrown, if the name violates rules.</exception>
        public string Name
        {
            get => name;
            set
            {
                if (!Utils.CheckName32(value))
                    throw new GameException(0x31);

                name = value;
            }
        }

        private TeamConfig()
        {
            Name = string.Empty;
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        internal TeamConfig(TeamConfig team)
        {
            Name = team.Name;
            Red = team.Red;
            Green = team.Green;
            Blue = team.Blue;
        }

        internal TeamConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            Red = reader.ReadByte();
            Green = reader.ReadByte();
            Blue = reader.ReadByte();
        }

        internal static TeamConfig Default => new TeamConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write((byte)Red);
            writer.Write((byte)Green);
            writer.Write((byte)Blue);
        }
    }
}
