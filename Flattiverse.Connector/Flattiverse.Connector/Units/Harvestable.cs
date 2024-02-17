using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.Units
{
    public class Harvestable : CelestialBody
    {
        public readonly ReadOnlyCollection<HarvestableSection> Sections;

        internal Harvestable(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            int coronas = reader.ReadByte();

            List<HarvestableSection> sections = new List<HarvestableSection>();

            for (int position = 0; position < coronas; position++)
                sections.Add(new HarvestableSection(null, reader));
        }
    }
}
