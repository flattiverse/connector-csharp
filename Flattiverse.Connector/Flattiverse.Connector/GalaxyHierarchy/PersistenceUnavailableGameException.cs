namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Persistent storage is currently unavailable for this login attempt.
/// </summary>
public class PersistenceUnavailableGameException : GameException
{
    internal PersistenceUnavailableGameException() : base(0x07, "[0x07] Persistent storage is currently unavailable. Please try again later.")
    {
    }
}
