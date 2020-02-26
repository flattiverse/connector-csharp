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
    }
}
