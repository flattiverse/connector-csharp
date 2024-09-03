using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// An info about a ClassicShipPlayerUnit which exists.
/// </summary>
public class ClassicShipControllableInfo : ControllableInfo
{
    internal ClassicShipControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive) :
        base(galaxy, player, id, name, alive)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ClassicShipPlayerUnit;
}