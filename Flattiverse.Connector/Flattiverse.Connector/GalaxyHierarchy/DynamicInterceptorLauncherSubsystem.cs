using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic interceptor launcher subsystem of a controllable.
/// </summary>
public class DynamicInterceptorLauncherSubsystem : DynamicShotLauncherSubsystem
{
    internal DynamicInterceptorLauncherSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
    }

    /// <summary>
    /// Requests one interceptor for the next server tick.
    /// </summary>
    public new async Task Shoot(Vector relativeMovement, ushort ticks, float load, float damage)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind movementValidity = RangeTolerance.ClampRange(relativeMovement, MinimumRelativeMovement, MaximumRelativeMovement,
            out relativeMovement);

        if (movementValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(movementValidity, "relativeMovement");

        if (ticks < MinimumTicks)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "ticks");

        if (ticks > MaximumTicks)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "ticks");

        InvalidArgumentKind loadValidity = RangeTolerance.ClampRange(load, MinimumLoad, MaximumLoad, out load);

        if (loadValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(loadValidity, "load");

        InvalidArgumentKind damageValidity = RangeTolerance.ClampRange(damage, MinimumDamage, MaximumDamage, out damage);

        if (damageValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(damageValidity, "damage");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x96;
            writer.Write(Controllable.Id);
            relativeMovement.Write(ref writer);
            writer.Write(ticks);
            writer.Write(load);
            writer.Write(damage);
        });
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicInterceptorLauncherSubsystemEvent(Controllable, Slot, Status, RelativeMovement, Ticks, Load, Damage,
            ConsumedEnergyThisTick, ConsumedIonsThisTick, ConsumedNeutrinosThisTick);
    }
}
