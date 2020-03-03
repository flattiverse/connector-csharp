using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A player unit. This is a controllable of you or another player.
    /// </summary>
    public class PlayerUnit : Unit
    {
        /// <summary>
        /// The direction the unit is looking at.
        /// </summary>
        public readonly float Direction;

        /// <summary>
        /// The engine output the unit currently has.
        /// </summary>
        public readonly float Engine;

        /// <summary>
        /// The rotation for each internal tick.
        /// </summary>
        public readonly float Rotation;

        /// <summary>
        /// The direction the unit is looking at.
        /// </summary>
        public readonly float Thruster;

        /// <summary>
        /// Informations abot the broad scanner or null if the scanned ship isn't scanning.
        /// </summary>
        public readonly Scanner BroadScanner;

        internal PlayerUnit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Direction = reader.ReadSingle();
            Engine = reader.ReadSingle();
            Rotation = reader.ReadSingle();
            Thruster = reader.ReadSingle();

            if (reader.ReadBoolean())
                BroadScanner = new Scanner(90, reader.ReadSingle());
        }

        public override UnitKind Kind => UnitKind.PlayerUnit;
    }
}
