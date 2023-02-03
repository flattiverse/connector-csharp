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

        internal GameException(int error, Exception innerException) : base(message(error, null), innerException)
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
                case 0x05:
                    return "[0x05] The command you tried to access can't be access with your player kind. (Tried to access admin commands as player or vice versa, etc.)";
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
                case 0xA8:
                    return "[0xA8] \"kind\" is missing.";
                case 0xA9:
                    return "[0xA9] \"kind\" couldn't be resolved to a valid unit kind. (Can't resolve \"playerUnit\", \"shot\" or \"explosion\".)";
                case 0xAA:
                    return "[0xAA] Can't replace a non editable unit like \"playerunit\", \"shot\" or \"explosion\".";
                case 0xB0:
                    return "[0xB0] The parameter you did specify was either null or didn't contain proper content.";
                case 0xB1:
                    return "[0xB1] The parameter you did specify exceeded the maximum size.";
                case 0xB2:
                    return "[0xB2] You dishonored the naming criteria of units. Only allowed characters are: space, dot, minus, underscore, a-z, A-Z and unicode characters between 192-214, 216-246 and 248-687.";
                case 0xC0:
                    return "[0xC0] We couldn't connect to the specified endpoint. Maybe a typo?";
                case 0xC1:
                    return "[0xC1] The specified auth key has been declined.";
                case 0xC2:
                    return "[0xC2] You are currently online. (You can only logon once with each account.) Please note: If your game just crashed: In such a case your account and ships are still lingering around so you have to wait round about 30 seconds before retrying.";
                case 0xC3:
                    return "[0xC3] This Universegroup seems to be full. Try another one.";
                case 0xCF:
                    return "[0xCF] Something went wrong while connecting but we don't know what and don't have any more infomration. You may try your luck with the inner exception.";
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
