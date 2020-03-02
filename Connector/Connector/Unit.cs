using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A unit.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// The name of the unit. This name is unique in the universe.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The galaxy this unit is in.
        /// </summary>
        public readonly Galaxy Galaxy;

        /// <summary>
        /// The radius of this unit.
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// The mobility of the unit.
        /// </summary>
        public readonly Mobility Mobility;

        /// <summary>
        /// The position of the unit.
        /// </summary>
        public readonly Vector Position;

        /// <summary>
        /// The movement of the unit or null, if the unit is Still.
        /// </summary>
        public readonly Vector Movement;

        /// <summary>
        /// Units, which are phased out of the galaxy don't take part in the game. Thus you never can scan such units.
        /// However admin view operations will return phased units. This is, as an example, in the case of a player ship
        /// which died. This player ship will then be reported by the view api at the location it died.
        /// </summary>
        public readonly bool Phased;

        /// <summary>
        /// The team of the unit or null, if the unit doesn't have a team assignment.
        /// </summary>
        public readonly Team Team;

        /// <summary>
        /// The gravity which the unit is producing.
        /// </summary>
        public readonly float Gravity;

        /// <summary>
        /// The radiation the unit is generating.
        /// </summary>
        public readonly float Radiation;

        /// <summary>
        /// The power output of the unit.
        /// </summary>
        public readonly float PowerOutput;

        /// <summary>
        /// True, if the unit can be altered by the XML Unit API.
        /// </summary>
        public readonly bool Alterable;

        internal Unit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader)
        {
            byte datas = reader.ReadByte();

            Mobility = (Mobility)(datas >> 6);

            Name = reader.ReadString();

            Galaxy = galaxy;

            Position = new Vector(ref reader);

            if (Mobility == Mobility.Still)
                Movement = null;
            else
                Movement = new Vector(ref reader);

            Radius = reader.ReadSingle();

            if ((datas & 0b0000_0001) == 0b0000_0001)
                Gravity = reader.ReadSingle();

            if ((datas & 0b0000_0010) == 0b0000_0010)
                Radiation = reader.ReadSingle();

            if ((datas & 0b0000_0100) == 0b0000_0100)
                PowerOutput = reader.ReadSingle();

            if ((datas & 0b0000_1000) == 0b0000_1000)
                Team = universe.teams[reader.ReadByte()];

            Alterable = (datas & 0b0010_0000) == 0b0010_0000;
            Phased = (datas & 0b0001_0000) == 0b0001_0000;
        }

        internal static Unit FromPacket(Universe universe, Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();
            Galaxy galaxy = universe.galaxies[packet.SubAddress];

            switch (packet.Helper)
            {
                case 0x04: // Target
                    return new Target(universe, galaxy, ref reader);
                case 0x08: // Sun
                    return new Sun(universe, galaxy, ref reader);
                case 0x10: // Planet
                    return new Planet(universe, galaxy, ref reader);
                case 0x11: // Moon
                    return new Moon(universe, galaxy, ref reader);
                case 0x12: // Meteoroid
                    return new Meteoroid(universe, galaxy, ref reader);
                case 0x20: // Buoy
                    return new Buoy(universe, galaxy, ref reader);
            }

            // Unknown unit.
            return null;
        }

        /// <summary>
        /// Checks, if the name of a unit is valid. Rules for this are:
        /// 1-64 chars, all latin chars, including umlauts and the chars: space . - _ \ / | and #.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>true, if the name is valid. false otherwise.</returns>
        public static bool CheckName(string name)
        {
            if (name == null || name.Length <= 1 || name.Length > 64)
                return false;

            if (name.StartsWith(" ") || name.EndsWith(" ") || name.StartsWith(".") || name.EndsWith(".") || name.StartsWith("-") || name.EndsWith("-") || name.StartsWith("_") || name.EndsWith("_") ||
                name.StartsWith("\\") || name.EndsWith("\\") || name.StartsWith("/") || name.EndsWith("/") || name.StartsWith("|") || name.EndsWith("|") || name.StartsWith("#") || name.EndsWith("#"))
                return false;

            foreach (char c in name)
            {
                if (c >= 'a' && c <= 'z')
                    continue;

                if (c >= 'A' && c <= 'Z')
                    continue;

                if (c >= '0' && c <= '9')
                    continue;

                if (c >= 192 && c <= 214)
                    continue;

                if (c >= 216 && c <= 246)
                    continue;

                if (c >= 248 && c <= 687)
                    continue;

                if (c == ' ' || c == '.' || c == '_' || c == '-' || c == '\\' || c == '/' || c == '|' || c == '#')
                    continue;

                return false;
            }

            return true;
        }

        /// <summary>
        /// The String representation of the unit.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"[{GetType()}] {Name} {Position} r={Radius}";
        }
    }
}
