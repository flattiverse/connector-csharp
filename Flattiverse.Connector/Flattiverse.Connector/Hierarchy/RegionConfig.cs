using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public RegionConfig(Region region)
        {
            Name = region.Name;
            StartPropability = region.StartPropability;
            RespawnPropability = region.RespawnPropability;
            Protected = region.Protected;
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
