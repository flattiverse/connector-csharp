using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A shot which most likely will kill somebody.
    /// </summary>
    public class Shot : Unit
    {
        /// <summary>
        /// The kind of ammunition.
        /// </summary>
        public readonly FlattiverseResourceKind Ammunition;

        internal Shot(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Ammunition = (FlattiverseResourceKind)reader.ReadByte();
        }

        /// <summary>
        /// The unit kind.
        /// </summary>
        public override UnitKind Kind => UnitKind.Shot;
    }
}
