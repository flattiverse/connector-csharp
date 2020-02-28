using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A plasma containing corona of a sun.
    /// </summary>
    public class Corona
    {
        /// <summary>
        /// The kind of plasma the corona contains.
        /// </summary>
        public readonly Plasma Plasma;

        /// <summary>
        /// Howmany plasma is produced per universe heartbeat.
        /// </summary>
        public readonly float Amount;

        /// <summary>
        /// The absolute radius of the corona.
        /// </summary>
        public readonly float Radius;

        internal Corona(byte plasma, ref BinaryMemoryReader reader)
        {
            Plasma = (Plasma)plasma;
            Amount = reader.ReadSingle();
            Radius = reader.ReadSingle();
        }
    }
}
