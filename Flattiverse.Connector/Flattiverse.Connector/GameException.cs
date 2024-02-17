using Flattiverse.Connector.Network;

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
        /// Unknown error
        /// </summary>
        public static byte UnknownError => 0xF0;
        /// <summary>
        /// Couldn't connect to the universe server: Are you online? Is flattiverse still online?
        /// </summary>
        public static byte ConnectionFailed => 0xF1;
        /// <summary>
        /// The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.
        /// </summary>
        public static byte GalaxyOffline => 0xF2;
        /// <summary>
        /// The call could not be processed. Only WebSocket calls are supported.
        /// </summary>
        public static byte InvalidCallType => 0xF3;
        /// <summary>
        /// The call could not be processed. The database is not available.
        /// </summary>
        public static byte DatabaseUnavailable => 0xF3;
        /// <summary>
        /// Authorization failed, auth parameter missing, ambiguous or unknown.
        /// </summary>
        public static byte InvalidAuthParameter => 0xF4;
        /// <summary>
        /// Authorization failed, no spectators allowed.
        /// </summary>
        public static byte NoSpectatorsAllowed => 0xF4;
        /// <summary>
        /// Forbidden. You are not allowed to perform this action (the way you tried).
        /// </summary>
        public static byte ForbiddenPlayerKind => 0xF5;
        /// <summary>
        /// Forbidden. You are not allowed to perform this action (the way you tried).
        /// </summary>
        public static byte Forbidden => 0xF5;
        /// <summary>
        /// The connector you are using is outdated.
        /// </summary>
        public static byte ConnectorOutdated => 0xF6;
        /// <summary>
        /// Login failed because you're already online.
        /// </summary>
        public static byte AlreadyOnline => 0xF7;
        /// <summary>
        /// Specified team doesn't exist.
        /// </summary>
        public static byte TeamNotFound => 0xF8;
        /// <summary>
        /// Specified cluster doesn't exist.
        /// </summary>
        public static byte ClusterNotFound => 0xF8;
        /// <summary>
        /// Specified region doesn't exist.
        /// </summary>
        public static byte RegionNotFound => 0xF8;
        /// <summary>
        /// Specified ship doesn't exist.
        /// </summary>
        public static byte ShipNotFound => 0xF8;
        /// <summary>
        /// Specified upgrade doesn't exist.
        /// </summary>
        public static byte UpgradeNotFound => 0xF8;
        /// <summary>
        /// Command didn't affect any database rows.
        /// </summary>
        public static byte NoRowsAffected => 0xF9;
        /// <summary>
        /// Given value is invalid.
        /// </summary>
        public static byte InvalidValue => 0xFA;
        /// <summary>
        /// Given name already exists.
        /// </summary>
        public static byte NameAlreadyExists => 0xFB;
        /// <summary>
        /// Can't create element because maximum for this kind is reached.
        /// </summary>
        public static byte NoEmptySlot => 0xFC;
        /// <summary>
        /// Connection closed.
        /// </summary>
        public static byte ConnectionClosed => 0xFE;
        /// <summary>
        /// Generic exception
        /// </summary>
        public static byte GenericException => 0xFF;

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
                case 0x31: // The parameter doesn't match required specification.
                    return "[0x31] The specified parameter doesn't match specification: Null? NaN or Infinity? Invalid characters? Out of bounds?";
                case 0x32: // Can't add more to the object. Object full?
                    return "[0x32] The object can't have more of these. Object full?";
                case 0x33: // There is no such Kind.
                    return "[0x33] There is no compatible or available kind.";
                case 0x34: // Not configurable.
                    return "[0x34] You don't have permission to alter this element.";
                case 0xE0: // The requested command doesn't exist.
                    return "[0xE0] Unauthorized request. You probably aren't the right kind of client: Player, Spectator or Admin.";
                
                case 0xF0: // Unknown error.
                    if (info is null)
                        return $"[0xF0] An unknown error occurred.";

                    return $"[0xF0] {info}";
                case 0xF1: // Couldn't establish tcp/ip connection.
                    return "[0xF1] Couldn't connect to the universe server: Are you online? Is flattiverse still online?";
                case 0xF2:// HTTP/502 or HTTP/504
                    return "[0xF2] The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.";
                case 0xF3:// HTTP/400
                    return "[0xF3] The call could not be processed. Either must didn't make a WebSocket call or the database is not available.";
                case 0xF4:// HTTP/401
                    return "[0xF4] Authorization failed, possibly because one of these reasons: auth parameter missing, ambiguous or unknown, or no spectators allowed.";
                case 0xF5:
                    return "[0xF5] Forbidden. You are not allowed to perform this action.";
                case 0xF6:// HTTP/409
                    return "[0xF6] The connector you are using is outdated.";
                case 0xF7:// HTTP/412
                    return "[0xF7] Login failed because you're already online.";
                case 0xF8:// HTTP/415
                    return "[0xF8] Specified element doesn't exist.";
                case 0xF9:
                    return "[0xF9] Command didn't affect any database rows.";
                case 0xFA:
                    if (info is null)
                        return $"[0xFA] Invalid value given.";

                    return $"[0xFA] {info}";
                case 0xFB:
                    return "[0xFB] Given name already exists.";
                case 0xFC:
                    return "[0xFC] Can't create element because maximum for this kind is reached.";
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
                    return new GameException(0xF6); //Outdated connector.
                case "412":
                    return new GameException(0xF7); //You are currently online.
                case "415":
                    return new GameException(0xF8); //Specified team doesn't exist or can't be selected.
                default:
                    return new GameException(0xF1); //Couldn't connect to the universe server: Are you online? Is flattiverse still online?
            }
        }
    }
}
