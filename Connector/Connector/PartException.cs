using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown when you couldn't part the universe. Most likely you weren't on the right universe, etc.
    /// </summary>
    public class PartException : InvalidOperationException
    {
        /// <summary>
        /// A number indicating the reason, so you don't have to parse text.
        /// </summary>
        public readonly byte Reason;

        internal PartException(byte reason) : base(message(reason))
        {
            Reason = reason;
        }

        private static string message(byte reason)
        {
            switch (reason)
            {
                case 0x01:
                    return "You are on no universe.";
                case 0x02:
                    return "You are on another universe.";
                default:
                    return "Denied, but I don't know why. :(";
            }
        }
    }
}
