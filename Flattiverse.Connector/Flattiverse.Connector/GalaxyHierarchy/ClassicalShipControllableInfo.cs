using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// An info about a ClassicalShipPlayerUnit which exists.
/// </summary>
public class ClassicalShipControllableInfo : ControllableInfo
{
    internal ClassicalShipControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive) :
        base(galaxy, player, id, name, alive)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ClassicalShipPlayerUnit;
}