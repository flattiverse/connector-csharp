using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
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

        /// <summary>
        /// true, if a colission with this element may harm you.
        /// </summary>
        public readonly bool Solid;

        /// <summary>
        /// true, if you can't scan behind this object or if it will keep radiation back.
        /// </summary>
        public readonly bool Masking;

        internal Unit(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader)
        {
            ushort datas = reader.ReadUInt16();

            Mobility = (Mobility)((datas >> 6) & 0b11);

            Name = reader.ReadString();

            Galaxy = galaxy;

            Position = new Vector(ref reader);

            if (Mobility == Mobility.Still)
                Movement = null;
            else
                Movement = new Vector(ref reader);

            Radius = reader.ReadSingle();

            if ((datas & 0b00000000_00000001) == 0b00000000_00000001)
                Gravity = reader.ReadSingle();

            if ((datas & 0b00000000_00000010) == 0b00000000_00000010)
                Radiation = reader.ReadSingle();

            if ((datas & 0b00000000_00000100) == 0b00000000_00000100)
                PowerOutput = reader.ReadSingle();

            if ((datas & 0b00000000_00001000) == 0b00000000_00001000)
                Team = universe.teams[reader.ReadByte()];

            Alterable = (datas & 0b00000000_00100000) == 0b00000000_00100000;
            Phased = (datas & 0b00000000_00010000) == 0b00000000_00010000;

            Solid = (datas & 0b00000001_00000000) == 0b00000001_00000000;
            Masking = (datas & 0b00000010_00000000) == 0b00000010_00000000;
        }

        internal static Unit FromPacket(Universe universe, Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();
            Galaxy galaxy = universe.galaxies[packet.SubAddress];

            switch (packet.Helper)
            {
                case 0x01: // PlayerUnit
                    return new Units.PlayerUnit(universe, galaxy, ref reader);
                case 0x04: // Target
                    return new Units.Target(universe, galaxy, ref reader);
                case 0x08: // Sun
                    return new Units.Sun(universe, galaxy, ref reader);
                case 0x10: // Planet
                    return new Units.Planet(universe, galaxy, ref reader);
                case 0x11: // Moon
                    return new Units.Moon(universe, galaxy, ref reader);
                case 0x12: // Meteoroid
                    return new Units.Meteoroid(universe, galaxy, ref reader);
                case 0x20: // Buoy
                    return new Units.Buoy(universe, galaxy, ref reader);
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
        /// The kind of the unit.
        /// </summary>
        public virtual UnitKind Kind => throw new NotImplementedException("Sorry. Please contact info@flattiverse.com.");

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
