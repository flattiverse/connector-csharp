using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// A Buoy. This unit transports a message.
    /// </summary>
    public class Buoy : SteadyUnit
    {
        /// <summary>
        /// The message of the buoy.
        /// </summary>
        public readonly string Message;

        internal Buoy(Universe universe, Galaxy galaxy, ref BinaryMemoryReader reader) : base(universe, galaxy, ref reader)
        {
            Message = reader.ReadString();
        }

        /// <summary>
        /// Checks the syntax of the message.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns>true, if the message obeys the rules, false otherwise.</returns>
        public static bool CheckMessage(string message)
        {
            if (message == null || message.Length <= 1 || message.Length > 128)
                return false;

            if (message.StartsWith(" ") || message.EndsWith(" ") || message.StartsWith(".") || message.StartsWith("!") || message.StartsWith("?") || message.StartsWith("-") || message.EndsWith("-") || message.StartsWith("_") || message.EndsWith("_") ||
                message.StartsWith("\\") || message.EndsWith("\\") || message.StartsWith("/") || message.EndsWith("/") || message.StartsWith("|") || message.EndsWith("|") || message.StartsWith("#") || message.EndsWith("#"))
                return false;

            foreach (char c in message)
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

                if (c == ' ' || c == '.' || c == '_' || c == '-' || c == '\\' || c == '/' || c == '|' || c == '#' || c == '?' || c == '!')
                    continue;

                return false;
            }

            return true;
        }

        public override UnitKind Kind => UnitKind.Buoy;
    }
}
