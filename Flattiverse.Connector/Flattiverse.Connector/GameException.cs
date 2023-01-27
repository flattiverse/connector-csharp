using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    /// <summary>
    /// An error int the game happened. Try harder!
    /// </summary>
    public class GameException : Exception
    {
        /// <summary>
        /// The error code number of the exception.
        /// </summary>
        public readonly int Code;

        internal GameException(int error) : base(message(error, null))
        {
            Code = error;
        }

        internal GameException(string info) : base(message(0xFF, info))
        {
            Code = 0xFF;
        }

        private static string message(int code, string? info)
        {
            switch (code)
            {
                case 0xFF:
                    return $"[0xFF] Some fatal error occourred and the server closed the connection. You may give this information to a flattiverse admin because this shouldn't happen:\n   -> \"{info}\"";
                default:
                    return $"[0x{code:X02}] Unknown GameException code 0x{code:X02} received.";
            }
        }
    }
}
