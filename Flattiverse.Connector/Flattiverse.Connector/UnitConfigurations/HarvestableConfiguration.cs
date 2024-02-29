using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units.SubComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.UnitConfigurations
{
    /// <summary>
    /// The configuration of a harvestable unit.
    /// </summary>
    public class HarvestableConfiguration : CelestialBodyConfiguration
    {
        internal readonly List<HarvestableSection> sections = new List<HarvestableSection>();

        internal HarvestableConfiguration() : base() { }

        internal HarvestableConfiguration(PacketReader reader) : base(reader)
        {
            int sections = reader.ReadByte();

            for (int section = 0; section < sections; section++)
                this.sections.Add(new HarvestableSection(this, reader));
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            writer.Write((byte)sections.Count);

            for (int section = 0; section < sections.Count; section++)
                sections[section].Write(writer);
        }

        /// <summary>
        /// all sections of the harvestable unit.
        /// </summary>
        public ReadOnlyCollection<HarvestableSection> Sections =>
            new ReadOnlyCollection<HarvestableSection>(new List<HarvestableSection>(sections));


        /// <summary>
        /// Adds a new circular section.
        /// </summary>
        /// <returns cref="HarvestableSection"> The circular section that was added</returns>
        /// <exception cref="GameException"></exception>
        /// <remarks>
        /// Only available to admins.
        /// </remarks>
        public HarvestableSection AddSection()
        {
            if (sections.Count >= 16)
                throw new GameException(0x32);

            HarvestableSection section = new HarvestableSection(this);

            sections.Add(section);

            return section;
        }

        /// <summary>
        /// Removes the section.
        /// </summary>
        /// <remarks>
        /// Only available to admins.
        /// </remarks>
        public void ClearSections()
        {
            sections.Clear();
        }
    }
}