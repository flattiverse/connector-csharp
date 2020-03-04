using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A Moon. A cosmetic "wall" unit.
    /// </summary>
    public class Moon : SteadyUnit
    {
        internal Moon(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
        }

        /// <summary>
        /// The kind of the unit.
        /// </summary>
        public override UnitKind Kind => UnitKind.Moon;
    }
}
