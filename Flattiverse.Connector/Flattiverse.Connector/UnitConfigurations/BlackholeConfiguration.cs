using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class BlackHoleConfiguration : CelestialBodyConfiguration
    {
        internal readonly List<BlackHoleSection> sections = new List<BlackHoleSection>();

        private BlackHoleConfiguration() : base() { }

        internal BlackHoleConfiguration(PacketReader reader) : base(reader)
        {
            int sections = reader.ReadByte();

            for (int section = 0; section < sections; section++)
                this.sections.Add(new BlackHoleSection(this, reader));
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            writer.Write((byte)sections.Count);

            for (int section = 0; section < sections.Count; section++)
                sections[section].Write(writer);
        }

        public ReadOnlyCollection<BlackHoleSection> Sections =>
            new ReadOnlyCollection<BlackHoleSection>(new List<BlackHoleSection>(sections));

        public BlackHoleSection AddSection()
        {
            if (sections.Count >= 16)
                throw new GameException(0x32);

            BlackHoleSection section = new BlackHoleSection(this);

            sections.Add(section);

            return section;
        }

        public override UnitKind Kind => UnitKind.BlackHole;

        internal static BlackHoleConfiguration Default => new BlackHoleConfiguration();
    }
}
