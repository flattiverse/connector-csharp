using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A Sun. A unit mostly producing much energy and sometimes also much radiation.
    /// </summary>
    public class Sun : SteadyUnit
    {
        /// <summary>
        /// The corona the sun has or null if the sun has no corona.
        /// </summary>
        public readonly SunCorona Corona;

        internal Sun(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            byte plasma = reader.ReadByte();

            if (plasma > 0)
                Corona = new SunCorona(plasma, ref reader);
        }

        public override UnitKind Kind => UnitKind.Sun;
    }
}
