using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A Meteoroid. A cosmetic "wall" unit.
    /// </summary>
    public class Meteoroid : SteadyUnit
    {
        internal Meteoroid(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
        }
    }
}
