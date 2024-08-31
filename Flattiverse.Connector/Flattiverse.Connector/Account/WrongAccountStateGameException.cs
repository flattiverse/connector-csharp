namespace Flattiverse.Connector.Account;

/// <summary>
/// The game server declined logging in due to your account status.
/// </summary>
public class WrongAccountStateGameException : GameException
{
    /// <summary>
    /// The account status your account has.
    /// </summary>
    public readonly AccountStatus Status;
    
    internal WrongAccountStateGameException() : base(0x04, "[0x04] Your account is in the wrong state - however, this connector version doesn't understand the state submitted.")
    {
        Status = AccountStatus.Unknown;
    }
    
    internal WrongAccountStateGameException(AccountStatus status) : base(0x04, FormulateException(status))
    {
        Status = status;
    }

    private static string FormulateException(AccountStatus status)
    {
        switch (status)
        {
            default:
                return "[0x04] Your account is in the wrong state - however, this connector version doesn't understand the state submitted.";
            case AccountStatus.OptIn:
                return "[0x04] You need to opt-in first to use the game.";
            case AccountStatus.ReOptIn:
                return "[0x04] You need to re-opt-in first to use the game.";
//            case AccountStatus.User:
//                return "[0x04] Well, the game server should have you let in. Please report this issue to info@flattiverse.com.";
            case AccountStatus.Banned:
                return "[0x04] Your account has been banned from using the game.";
            case AccountStatus.Deleted:
                return "[0x04] Your account is deleted.";
        }
    }
}