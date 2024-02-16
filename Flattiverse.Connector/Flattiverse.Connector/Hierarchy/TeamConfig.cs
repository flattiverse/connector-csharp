using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public TeamConfig(Team team)
        {
            Name = team.Name;
            Red = team.Red;
            Green = team.Green;
            Blue = team.Blue;
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
