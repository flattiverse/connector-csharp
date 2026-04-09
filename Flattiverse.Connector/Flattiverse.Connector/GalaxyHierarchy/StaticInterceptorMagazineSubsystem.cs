using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;

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

    internal StaticInterceptorMagazineSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, reader, slot)
    {
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        return base.CreateRuntimeEvent();
    }
}
