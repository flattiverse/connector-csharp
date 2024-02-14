using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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

        internal GameException(int error) : base(ParsedMessage(error, null))
        {
            Code = error;
        }

        internal GameException(int error, Exception innerException) : base(ParsedMessage(error, null), innerException)
        {
            Code = error;
        }

        internal GameException(int error, string info, Exception innerException) : base(ParsedMessage(error, info), innerException)
        {
            Code = error;
        }

        internal GameException(string info) : base(ParsedMessage(0xFF, info))
        {
            Code = 0xFF;
        }

        private static string ParsedMessage(int code, string? info)
        {
            switch (code)
            {
                case 0xF0: // Connection issues.
                    return $"[0xF0] An unknown error occurred while connecting to the flattiverse server: {info}";
                case 0xF1:
                    return "[0xF1] Couldn't connect to the universe server: Are you online? Is flattiverse still online?";
                case 0xF2:
                    return "[0xF2] The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.";
                case 0xFF:
                    return info ?? "[0xFF] Generic exception thrown without additional error message (info is null).";
                default:
                    return $"[0x{code:X02}] Unspecified GameException code 0x{code:X02} received. Outdated connector somehow?!";
            }
        }

        internal static GameException ParseHttpCode(string code)
        {
            switch (code)
            {
                case "502":
                case "504":
                    return new GameException(0xF2); //The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.
                case "400":
                    return new GameException(0xF3); //You must make a WebSocket call or database is not available.
                case "401":
                    return new GameException(0xF4); //Unauthorized.
                case "409":
                    return new GameException(0xF5); //Outdated connector.
                case "412":
                    return new GameException(0xF6); //You are currently online.
                case "415":
                    return new GameException(0xF7); //Specified team doesn't exist or can't be selected.
                default:
                    return new GameException(0xF1); //Couldn't connect to the universe server: Are you online? Is flattiverse still online?
            }
        }
    }
}
