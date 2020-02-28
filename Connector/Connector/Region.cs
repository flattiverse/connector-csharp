using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A game region. Used to specify starting regions or named regions.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The galaxy-wide ID of the region. The same ID will overwrite eachother.
        /// </summary>
        public byte ID;

        /// <summary>
        /// The name of a region or null.
        /// </summary>
        public string Name;

        /// <summary>
        /// The team of a reagion or null.
        /// </summary>
        public Team Team;

        /// <summary>
        /// The left border of the region.
        /// </summary>
        public float Left;

        /// <summary>
        /// The top border of the region.
        /// </summary>
        public float Top;

        /// <summary>
        /// The right border of the region.
        /// </summary>
        public float Right;

        /// <summary>
        /// The bottom border of the region.
        /// </summary>
        public float Bottom;

        /// <summary>
        /// True, if ships can spawn there.
        /// </summary>
        public bool Spawn;

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="id">This ID needs to be unique in the galaxy. Your Update/Create request will update a region with the same ID.</param>
        public Region(byte id)
        {
            ID = id;
        }

        internal Region(Universe universe, ref BinaryMemoryReader reader)
        {
            ID = reader.ReadByte();
            Name = reader.ReadString();

            byte team = reader.ReadByte();

            if (team != 0xFF)
                Team = universe.teams[team];

            Left = reader.ReadSingle();
            Top = reader.ReadSingle();
            Right = reader.ReadSingle();
            Bottom = reader.ReadSingle();
            Spawn = reader.ReadBoolean();
        }

        internal void Write(ManagedBinaryMemoryWriter writer)
        {
            writer.Write(ID);
            writer.Write(Name);

            if (Team == null)
                writer.Write((byte)0xFF);
            else
                writer.Write(Team.ID);

            writer.Write(Left);
            writer.Write(Top);
            writer.Write(Right);
            writer.Write(Bottom);
            writer.Write(Spawn);
        }

        /// <summary>
        /// Checks if the name is compatible to a reagion name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>true, if the name complied the region naming ruleset, false otherwise.</returns>
        public static bool CheckName(string name)
        {
            if (name == null)
                return true;

            if (name.Length <= 1 || name.Length > 64)
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
    }
}
