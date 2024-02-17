using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units.SubComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units
{
    public class Sun : CelestialBody
    {
        public readonly ReadOnlyCollection<SunSection> Sections;

        internal Sun(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            int coronas = reader.ReadByte();

            List<SunSection> sections = new List<SunSection>();

            for (int position = 0; position < coronas; position++)
                sections.Add(new SunSection(null, reader));
        }
    }
}
