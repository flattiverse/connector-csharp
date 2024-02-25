using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    internal class Utils
    {
        /// <summary>
        /// Checks the name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>truw if the name is proper, false if the name doesn't pass the check.</returns>
        public static bool CheckName32(string? name)
        {
            if (name is null || name.Length < 2 || name.Length > 32)
                return false;

            if (name.StartsWith(' ') || name.EndsWith(' '))
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

                if (c == ' ' || c == '.' || c == '_' || c == '-')
                    continue;

                return false;
            }

            return true;
        }


        /// <summary>
        /// Checks the name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>truw if the name is proper, false if the name doesn't pass the check.</returns>
        public static bool CheckName64(string? name)
        {
            if (name is null || name.Length < 2 || name.Length > 64)
                return false;

            if (name.StartsWith(' ') || name.EndsWith(' '))
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

                if (c == ' ' || c == '.' || c == '_' || c == '-')
                    continue;

                return false;
            }

            return true;
        }
    }
}
