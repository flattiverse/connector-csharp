using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Account;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector;

/// <summary>
/// Base type for protocol- and connector-level failures raised by the Flattiverse connector.
/// </summary>
public class GameException : Exception
{
    /// <summary>
    /// Numeric connector/protocol error code.
    /// </summary>
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
        byte bte;
        string str;
        
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
                    if (packetReader.Read(out bte))
                        exception = new WrongAccountStateGameException((AccountStatus)bte);
                    else
                        exception = new WrongAccountStateGameException();
                    return true;
                case 0x05:
                    exception = new TeamSelectionFailedGameException();
                    return true;
                case 0x06:
                    exception = new SelfDisclosureRequiredGameException();
                    return true;
                case 0x07:
                    exception = new PersistenceUnavailableGameException();
                    return true;
                case 0x08:
                    if (packetReader.Read(out bte))
                    {
                        exception = new ServerFullOfPlayerKindGameException((PlayerKind)bte);
                        return true;
                    }

                    exception = null;
                    return false;
                case 0x09:
                    exception = new AccountAlreadyLoggedInGameException();
                    return true;
                case 0x0C:
                    exception = new SessionsExhaustedException();
                    return true;
                case 0x0D:
                    exception = new InvalidDataGameException();
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
                case 0x12:
                    if (packetReader.Read(out bte) && packetReader.Read(out str))
                    {
                        exception = new InvalidArgumentGameException((InvalidArgumentKind)bte, str);
                        return true;
                    }
                    
                    exception = null;
                    return false;
                case 0x13:
                    exception = new PermissionFailedGameException();
                    return true;
                case 0x14:
                    exception = new FloodcontrolTriggeredGameException();
                    return true;
                case 0x15:
                    exception = new UnitConstraintViolationGameException();
                    return true;
                case 0x16:
                    if (packetReader.Read(out bte) && packetReader.Read(out string nodePath) && packetReader.Read(out string hint))
                    {
                        exception = new InvalidXmlNodeValueGameException((InvalidArgumentKind)bte, nodePath, hint);
                        return true;
                    }

                    exception = null;
                    return false;
                case 0x17:
                    exception = new ControllableIsClosingGameException();
                    return true;
                case 0x18:
                    exception = new AvatarNotAvailableGameException();
                    return true;
                case 0x20:
                    exception = new YouNeedToContinueFirstGameException();
                    return true;
                case 0x21:
                    exception = new YouNeedToDieFirstGameException();
                    return true;
                case 0x22:
                    exception = new AllStartLocationsAreOvercrowded();
                    return true;
                case 0x23:
                    if (packetReader.Read(out str))
                    {
                        exception = new MissingAchievementGameException(str);
                        return true;
                    }

                    exception = null;
                    return false;
                case 0x24:
                    if (packetReader.Read(out str))
                    {
                        exception = new TeamNotPlayableGameException(str);
                        return true;
                    }

                    exception = null;
                    return false;
                case 0x30:
                    exception = new CanOnlyShootOncePerTickGameException();
                    return true;
                case 0x31:
                    exception = new TournamentNotConfiguredGameException();
                    return true;
                case 0x32:
                    exception = new TournamentAlreadyConfiguredGameException();
                    return true;
                case 0x33:
                    exception = new TournamentWrongStageGameException();
                    return true;
                case 0x34:
                    exception = new TournamentMapEditingLockedGameException();
                    return true;
                case 0x35:
                    exception = new TournamentRegistrationClosedGameException();
                    return true;
                case 0x36:
                    exception = new TournamentParticipantRequiredGameException();
                    return true;
                case 0x37:
                    exception = new TournamentSpectatingForbiddenGameException();
                    return true;
                case 0x38:
                    exception = new TournamentTeamMismatchGameException();
                    return true;
                case 0x39:
                    exception = new TournamentModeNotAllowedGameException();
                    return true;
                case 0x3A:
                    exception = new PlayerAccessRestrictedGameException();
                    return true;
                case 0x3B:
                    exception = new AdminAccessRestrictedGameException();
                    return true;
                case 0x3C:
                    exception = new StaticMapRebuildInProgressGameException();
                    return true;
                case 0x3D:
                    exception = new StaticMapRebuildLockedGameException();
                    return true;
                case 0x3E:
                    exception = new BinaryChatAckRequiredGameException();
                    return true;
                case 0x3F:
                    exception = new ControllableIsRebuildingGameException();
                    return true;
            }

        exception = null;
        return false;
    }
}
