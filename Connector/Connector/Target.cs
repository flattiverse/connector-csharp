using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A Target, former MissionTarget. It has a sequence number (check the score!) 
    /// </summary>
    public class Target : SteadyUnit
    {
        /// <summary>
        /// The sequencenumber of the mission target.
        /// </summary>
        public readonly int Sequence;

        /// <summary>
        /// The hint, where the next mission target is hidden. This may also be null.
        /// </summary>
        public readonly float? Hint;

        internal Target(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Sequence = reader.ReadByte();
            Hint = reader.ReadSingle();

            if (Hint == -1)
                Hint = null;
        }
    }
}
