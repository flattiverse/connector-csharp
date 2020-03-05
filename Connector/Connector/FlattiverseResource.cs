using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A resource on a ship.
    /// </summary>
    public class FlattiverseResource
    {
        /// <summary>
        /// The kind of the resource.
        /// </summary>
        public readonly FlattiverseResourceKind Kind;

        private ushort current;
        private ushort max;

        internal FlattiverseResource(FlattiverseResourceKind kind)
        {
            Kind = kind;
        }

        internal void Update(ushort value, byte cargoSystemLevel)
        {
            current = value;

            if (Kind <= FlattiverseResourceKind.AmmunitionMagenta)
                max = (ushort)(cargoSystemLevel * 20 + 20);
            else if (Kind <= FlattiverseResourceKind.Silicon)
                max = 50000;
            else if (Kind == FlattiverseResourceKind.SpaceCrystal)
                max = (ushort)(cargoSystemLevel * 3);
            else
                max = (ushort)(cargoSystemLevel * 5 + 5);
        }

        /// <summary>
        /// The current amount on this ship.
        /// </summary>
        public int Current => current;

        /// <summary>
        /// The maximum amount on this ship.
        /// </summary>
        public int Max => max;
    }
}
