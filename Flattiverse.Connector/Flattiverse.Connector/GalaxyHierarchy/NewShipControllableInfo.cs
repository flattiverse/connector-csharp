using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// An info about a NewShipPlayerUnit which exists.
/// </summary>
public class NewShipControllableInfo : ControllableInfo
{
    internal NewShipControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive) :
        base(galaxy, player, id, name,  alive)
    {
    }
    
    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.NewShipPlayerUnit;
}