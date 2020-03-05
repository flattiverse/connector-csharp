using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A explosion
    /// </summary>
    public class Explosion : SteadyUnit
    {
        public readonly FlattiverseResourceKind Ammunition;

        internal Explosion(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Ammunition = (FlattiverseResourceKind)reader.ReadByte();
        }

        /// <summary>
        /// The unit kind.
        /// </summary>
        public override UnitKind Kind => UnitKind.Explosion;
    }
}
