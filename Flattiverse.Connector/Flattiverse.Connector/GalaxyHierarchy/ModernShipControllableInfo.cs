using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Roster entry for one new-ship controllable of a player.
/// </summary>
public class ModernShipControllableInfo : ControllableInfo
{
    internal ModernShipControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive) :
        base(galaxy, player, id, name,  alive)
    {
    }
    
    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ModernShipPlayerUnit;
}
