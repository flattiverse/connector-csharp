using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A unit which has minable resources.
    /// </summary>
    public class CommodityUnit : SteadyUnit
    {
        /// <summary>
        /// The resource you will gather when you fly near this units core. (unit.Position - your.Position &lt; unit.Radius + your.Radius + 25f)
        /// </summary>
        public readonly FlattiverseResourceKind Resource;

        internal CommodityUnit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Resource = (FlattiverseResourceKind)reader.ReadByte();
        }
    }
}
