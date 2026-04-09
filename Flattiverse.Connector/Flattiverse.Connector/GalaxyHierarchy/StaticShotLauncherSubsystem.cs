using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Static shot launcher subsystem of a modern ship.
/// </summary>
public class StaticShotLauncherSubsystem : DynamicShotLauncherSubsystem
{
    internal StaticShotLauncherSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
    }

    internal StaticShotLauncherSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, reader, slot)
    {
    }

    public float RelativeSpeed
    {
        get { return RelativeMovement.Length; }
    }

    public bool CalculateCost(float relativeSpeed, ushort ticks, float load, float damage, out float energy, out float ions, out float neutrinos)
    {
        return base.CalculateCost(Vector.FromAngleLength(0f, relativeSpeed), ticks, load, damage, out energy, out ions, out neutrinos);
    }

    public async Task Shoot(float relativeSpeed, ushort ticks, float load, float damage)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind movementValidity = RangeTolerance.ClampRange(relativeSpeed, MinimumRelativeMovement, MaximumRelativeMovement,
            out relativeSpeed);

        if (movementValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(movementValidity, "relativeSpeed");

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
            writer.Command = 0xA5;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
            writer.Write(relativeSpeed);
            writer.Write(ticks);
            writer.Write(load);
            writer.Write(damage);
        });
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        return base.CreateRuntimeEvent();
    }
}
