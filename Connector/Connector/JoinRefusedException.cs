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
        internal JoinRefusedException(byte reason) : base (message(reason))
        {
        }

        private static string message(byte reason)
        {
            switch (reason)
            {
                case 0x01:
                    return "Universe is full. (Maximum players reached.)";
                case 0x02:
                    return "Selected team is full. (Maximum players for this team reached.)";
                case 0x03:
                    return "Access denied. (You don't have the necessary privileges or are banned from this universe.)";
                case 0x04:
                    return "Access denied. (Your join configuration doesn't match the tournament configuration.)";
                default:
                    return "Denied, but I don't know why. :(";
            }
        }
    }
}
