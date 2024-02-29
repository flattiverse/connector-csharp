using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using Flattiverse.Connector.Units.SubComponents;
using System.Collections.ObjectModel;

namespace Flattiverse.Connector.UnitConfigurations
{
   /// <summary>
   /// The configuration of a black hole.
   /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<BlackHoleSection> Sections =>
            new ReadOnlyCollection<BlackHoleSection>(new List<BlackHoleSection>(sections));

        /// <summary>
        /// Adds a new circular section to the black hole.
        /// </summary>
        /// <returns cref="BlackHoleSection"> The circular section that was added</returns>
        /// <exception cref="GameException"></exception>
        /// <remarks>
        /// Only available to admins.
        /// </remarks>
        public BlackHoleSection AddSection()
        {
            if (sections.Count >= 16)
                throw new GameException(0x32);

            BlackHoleSection section = new BlackHoleSection(this);

            sections.Add(section);

            return section;
        }

        /// <summary>
        /// Removes a section from the black hole.
        /// </summary>
        /// <remarks>
        /// Only available to admins.
        /// </remarks>
        public void ClearSections()
        {
            sections.Clear();
        }

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.BlackHole;

        internal static BlackHoleConfiguration Default => new BlackHoleConfiguration();
    }
}
