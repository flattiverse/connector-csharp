using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Static interceptor magazine subsystem of a modern ship.
/// </summary>
public class StaticInterceptorMagazineSubsystem : DynamicInterceptorMagazineSubsystem
{
    internal StaticInterceptorMagazineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        return base.CreateRuntimeEvent();
    }
}
