using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A Planet. A cosmetic "wall" unit.
    /// </summary>
    public class Planet : SteadyUnit
    {
        internal Planet(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
        }
        public override UnitKind Kind => UnitKind.Planet;
    }
}
