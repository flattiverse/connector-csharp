using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A Sun. A unit mostly producing much energy and sometimes also much radiation.
    /// </summary>
    public class Sun : SteadyUnit
    {
        internal Sun(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
        }
    }
}
