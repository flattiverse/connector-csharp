using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic interceptor magazine subsystem of a controllable.
/// </summary>
public class DynamicInterceptorMagazineSubsystem : DynamicShotMagazineSubsystem
{
    internal DynamicInterceptorMagazineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
    }

    internal DynamicInterceptorMagazineSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, reader, slot)
    {
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicInterceptorMagazineSubsystemEvent(Controllable, Slot, Status, CurrentShots);
    }
}
