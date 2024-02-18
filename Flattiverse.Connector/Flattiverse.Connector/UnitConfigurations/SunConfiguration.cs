using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class SunConfiguration : CelestialBodyConfiguration
    {
        internal readonly List<SunSection> sections = new List<SunSection>();

        private SunConfiguration() : base() { }

        internal SunConfiguration(PacketReader reader) : base(reader)
        {
            int sections = reader.ReadByte();

            for (int section = 0; section < sections; section++)
                this.sections.Add(new SunSection(this, reader));
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            writer.Write((byte)sections.Count);

            for (int section = 0; section < sections.Count; section++)
                sections[section].Write(writer);
        }

        public ReadOnlyCollection<SunSection> Sections =>
            new ReadOnlyCollection<SunSection>(new List<SunSection>(sections));

        public SunSection AddSection()
        {
            if (sections.Count >= 16)
                throw new GameException(0x32);

            SunSection section = new SunSection(this);

            sections.Add(section);

            return section;
        }

        public void ClearSections()
        {
            sections.Clear();
        }

        public override UnitKind Kind => UnitKind.Sun;

        internal static SunConfiguration Default => new SunConfiguration();
    }
}
