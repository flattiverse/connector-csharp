using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Static shot magazine subsystem of a modern ship.
/// </summary>
public class StaticShotMagazineSubsystem : DynamicShotMagazineSubsystem
{
    internal StaticShotMagazineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        return base.CreateRuntimeEvent();
    }
}
