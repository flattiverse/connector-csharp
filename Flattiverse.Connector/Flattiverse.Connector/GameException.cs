using Flattiverse.Connector.Network;
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

        /// <summary>
        /// This exception has to be replaced!
        /// </summary>
        public static GameException TODO => new GameException(0xFF);

        internal GameException(int error) : base(ParsedMessage(error, null))
        {
            Code = error;
        }

        internal GameException(int error, Exception innerException) : base(ParsedMessage(error, null), innerException)
        {
            Code = error;
        }

        internal GameException(int error, string? info) : base(ParsedMessage(error, info))
        {
            Code = error;
        }

        internal GameException(int error, string? info, Exception innerException) : base(ParsedMessage(error, info), innerException)
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
                case 0x30: // The requested element doesn't exist.
                    if (info is null)
                        return $"[0x30] The requested element doesn't exist or can't be accessed.";
                    
                    return $"[0x30] {info}";
                case 0xE0: // The requested command doesn't exist.
                    return "[0xE0] Unauthorized request. You probably aren't the right kind of client: Player, Spectator or Admin.";
                case 0xF0: // Unspecified connection issues.
                    return $"[0xF0] An unknown error occurred while connecting to the flattiverse server: {info}";
                case 0xF1: // Couldn't establish tcp/ip connection.
                    return "[0xF1] Couldn't connect to the universe server: Are you online? Is flattiverse still online?";
                case 0xF2:// HTTP/502 or HTTP/504
                    return "[0xF2] The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.";
                case 0xF3:// HTTP/400
                    return "[0xF3] The call could not be processed. Either must didn't make a WebSocket call or the database is not available.";
                case 0xF4:// HTTP/401
                    return "[0xF4] Authorization failed, possibly because one of these reasons: auth parameter missing, ambiguous or unknown, or no spectators allowed.";
                case 0xF5:// HTTP/409
                    return "[0xF5] The connector you are using is outdated.";
                case 0xF6:// HTTP/412
                    return "[0xF6] Login failed because you're already online.";
                case 0xF7:// HTTP/415
                    return "[0xF7] Specified team doesn't exist or can't be selected.";
                case 0xFE: // Connection has been closed.
                    if (info is null)
                        return $"[0xFE] The network connection has been closed.";

                    return $"[0xFE] The network connection has been closed: {info}";
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

        internal static GameException? Check(Packet packet)
        {
            if (packet.Header.Command == 0xFF)
                if (packet.Header.Size > 0)
                    return new GameException(packet.Header.Param0, packet.Read().ReadString());
                else
                    return new GameException(packet.Header.Param0);

            return null;
        }
    }
}
