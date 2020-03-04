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
        public readonly FlattiverseResource Resource;

        internal CommodityUnit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Resource = (FlattiverseResource)reader.ReadByte();
        }
    }
}
