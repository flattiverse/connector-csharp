using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Account;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector;

/// <summary>
/// An exception happened in the area of flattiverse.
/// </summary>
public class GameException : Exception
{
    public readonly int ErrorNumber;

    internal GameException(int errorNumber) : base ($"[0x{errorNumber:X02}] Unknown error message.")
    {
        ErrorNumber = errorNumber;
    }
    
    internal GameException(int errorNumber, string message) : base (message)
    {
        ErrorNumber = errorNumber;
    }
    
    internal GameException(int errorNumber, string message, Exception exception) : base (message, exception)
    {
        ErrorNumber = errorNumber;
    }

    internal static bool TryParseGameException(PacketReader packetReader, [NotNullWhen(true)] out GameException? exception)
    {
        byte various0;
        
        if (packetReader.Command == 0xFF && packetReader.Read(out byte code))
            switch (code)
            {
                case 0x01:
                    exception = new CantConnectGameException();
                    return true;
                case 0x02:
                    exception = new InvalidProtocolVersionGameException();
                    return true;
                case 0x03:
                    exception = new AuthFailedGameException();
                    return true;
                case 0x04:
                    if (packetReader.Read(out various0))
                        exception = new WrongAccountStateGameException((AccountStatus)various0);
                    else
                        exception = new WrongAccountStateGameException();
                    return true;
                case 0x05:
                    exception = new InvalidOrMissingTeamGameException();
                    return true;
                case 0x08:
                    if (packetReader.Read(out various0))
                    {
                        exception = new ServerFullOfPlayerKindGameException((PlayerKind)various0);
                        return true;
                    }

                    exception = null;
                    return false;
                case 0x0C:
                    exception = new SessionsExhaustedException();
                    return true;
                case 0x0F: // We don't care about the real reason because actually this exception shouldn't be ever communicated.
                    exception = new ConnectionTerminatedGameException(null);
                    return true;
                case 0x10:
                    exception = new SpecifiedElementNotFoundGameException();
                    return true;
                case 0x11:
                    exception = new CantCallThisConcurrentGameException();
                    return true;
            }

        exception = null;
        return false;
    }
}