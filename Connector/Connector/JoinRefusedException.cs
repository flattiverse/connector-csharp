using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when your try to join the universe has been denied.
    /// </summary>
    public class JoinRefusedException : InvalidOperationException
    {
        /// <summary>
        /// A number indicating the reason, so you don't have to parse text.
        /// </summary>
        public readonly byte Reason;

        internal JoinRefusedException(byte reason) : base (message(reason))
        {
            Reason = reason;
        }

        private static string message(byte reason)
        {
            switch (reason)
            {
                case 0x01:
                    return "You are already assigned to an universe. Part first, please.";
                case 0x02:
                    return "You specified an invalid team.";
                case 0x03:
                    return "Universe is full. (Maximum players reached.)";
                case 0x04:
                    return "Selected team is full. (Maximum players for this team reached.)";
                case 0x05:
                    return "Access denied. (You don't have the necessary privileges or are banned from this universe.)";
                case 0x06:
                    return "Access denied. (Your join configuration doesn't match the tournament configuration.)";
                case 0x07:
                    return "Universe not ready. (E.g. offline.)";
                default:
                    return "Denied, but I don't know why. :(";
            }
        }
    }
}
