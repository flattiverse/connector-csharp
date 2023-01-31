using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    /// <summary>
    /// An error in the game happened. Try harder!
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
                case 0xA0:
                    return "[0xA0] Your JSON defintion is missing some mandatory base value like name or radius.";
                case 0xA1:
                    return "[0xA1] Your JSON defintion is missing some mandatory non-base value.";
                case 0xA2:
                    return "[0xA2] At least one required JSON property doesn't exist.";
                case 0xA3:
                    return "[0xA3] At least one required JSON property doesn't have the required kind.";
                case 0xA4:
                    return "[0xA4] At least one required JSON property has an invalid value.";
                case 0xF0:
                    return "[0xF0] The web socket got terminated while waiting for the completion of the command. This usually indicates that you have a network connectivity issue somewhere between you and the server or that the server has been rebooted to reload some level settings.";
                case 0xFF:
                    return $"[0xFF] Some fatal error occourred and the server closed the connection. You may give this information to a flattiverse admin because this shouldn't happen:\n   -> \"{info}\"";
                default:
                    return $"[0x{code:X02}] Unknown GameException code 0x{code:X02} received.";
            }
        }
    }
}
