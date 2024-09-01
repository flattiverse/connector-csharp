namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown, if you try to call a command where you don't have access to.
/// </summary>
public class PermissionFailedGameException : GameException
{
    internal PermissionFailedGameException() : base(0x13, "[0x13] Permission denied. Did you try to call a command where you don't have access to?")
    {
    }
}