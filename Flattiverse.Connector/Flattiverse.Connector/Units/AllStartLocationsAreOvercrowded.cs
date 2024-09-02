namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if a call to Continue() fails, because there is no space for you.
/// </summary>
public class AllStartLocationsAreOvercrowded : GameException
{
    internal AllStartLocationsAreOvercrowded() : base(0x22, "[0x22] All start locations are currently overcrowded.")
    {
    }
}