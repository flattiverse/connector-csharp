using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A unit which can stay Still or has a Steady (Orbiting) Movement.
    /// </summary>
    public class SteadyUnit : Unit
    {
        internal SteadyUnit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
        }
    }
}
