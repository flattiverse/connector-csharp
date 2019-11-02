using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the operation range of systems of ships in a universe.
    /// </summary>
    public class UniverseSystem
    {
        /// <summary>
        /// The UniverseSystemKind.
        /// </summary>
        public readonly UniverseSystemKind Kind;

        /// <summary>
        /// The initial level of the corresponding system how ships will be spawned.
        /// </summary>
        public readonly int StartLevel;

        /// <summary>
        /// The maximum level of the corresponding. system (Upgradable limits.)
        /// </summary>
        public readonly int EndLevel;

        internal bool InUse => StartLevel != 0 || EndLevel != 0;

        internal UniverseSystem(ref BinaryMemoryReader reader)
        {
            Kind = (UniverseSystemKind)reader.ReadByte();

            StartLevel = reader.ReadByte();
            EndLevel = reader.ReadByte();
        }

        /// <summary>
        /// A string representation of this UniverseSystem.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{Kind} [{StartLevel}-{EndLevel}]";
        }
    }
}
