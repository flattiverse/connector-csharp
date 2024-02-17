using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class TeamConfig
    {
        public string Name;
        public int Red;
        public int Green;
        public int Blue;

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
