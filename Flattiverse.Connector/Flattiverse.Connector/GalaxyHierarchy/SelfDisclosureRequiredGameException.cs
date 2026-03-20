namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The galaxy requires self-disclosure data for regular player logins.
/// </summary>
public class SelfDisclosureRequiredGameException : GameException
{
    internal SelfDisclosureRequiredGameException() : base(0x06, "[0x06] Galaxy requires self-disclosure for this login.")
    {
    }
}
