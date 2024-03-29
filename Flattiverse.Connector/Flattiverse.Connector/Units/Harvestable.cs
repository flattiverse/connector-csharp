﻿using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// A harvestable celestial body.
    /// Resources can be gathered from this body.
    /// </summary>
    public class Harvestable : CelestialBody
    {
        internal ReadOnlyCollection<HarvestableSection> sections;

        internal Harvestable(Cluster cluster, PacketReader reader) : base(cluster, reader)
        {
            int coronas = reader.ReadByte();

            List<HarvestableSection> tSections = new List<HarvestableSection>();

            for (int position = 0; position < coronas; position++)
                tSections.Add(new HarvestableSection(null, reader));

            sections = new ReadOnlyCollection<HarvestableSection>(tSections);
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);

            int coronas = reader.ReadByte();

            List<HarvestableSection> tSections = new List<HarvestableSection>();

            for (int position = 0; position < coronas; position++)
                tSections.Add(new HarvestableSection(null, reader));

            sections = new ReadOnlyCollection<HarvestableSection> (tSections);
        }

        /// <summary>
        /// Sections of this harvestable.
        /// These are areas with resources.
        /// </summary>
        public ReadOnlyCollection<HarvestableSection> Sections => sections;

    }
}
