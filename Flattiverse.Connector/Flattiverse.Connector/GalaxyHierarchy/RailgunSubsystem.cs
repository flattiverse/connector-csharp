using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Railgun subsystem of a controllable.
/// </summary>
public class RailgunSubsystem : Subsystem
{
    private const float ProjectileSpeedValue = 4f;
    private const ushort ProjectileLifetimeValue = 250;
    private const float EnergyCostValue = 300f;
    private const float MetalCostValue = 1f;

    private RailgunDirection _direction;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal RailgunSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) : base(controllable, name, exists, slot)
    {
        _direction = RailgunDirection.None;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    /// <summary>
    /// Rail projectile relative speed.
    /// </summary>
    public float ProjectileSpeed
    {
        get { return ProjectileSpeedValue; }
    }

    /// <summary>
    /// Rail projectile lifetime in ticks.
    /// </summary>
    public ushort ProjectileLifetime
    {
        get { return ProjectileLifetimeValue; }
    }

    /// <summary>
    /// Energy consumed by one rail shot.
    /// </summary>
    public float EnergyCost
    {
        get { return EnergyCostValue; }
    }

    /// <summary>
    /// Metal consumed by one rail shot.
    /// </summary>
    public float MetalCost
    {
        get { return MetalCostValue; }
    }

    /// <summary>
    /// The direction processed during the current server tick.
    /// </summary>
    public RailgunDirection Direction
    {
        get { return _direction; }
    }

    /// <summary>
    /// The energy consumed by the railgun during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed by the railgun during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed by the railgun during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the resource costs for one rail shot.
    /// </summary>
    public bool CalculateCost(out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        energy = EnergyCostValue;
        return true;
    }

    /// <summary>
    /// Fires the railgun forward.
    /// </summary>
    public Task FireFront()
    {
        return Fire(0x9A);
    }

    /// <summary>
    /// Fires the railgun backward.
    /// </summary>
    public Task FireBack()
    {
        return Fire(0x9B);
    }

    private async Task Fire(byte command)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = command;
            writer.Write(Controllable.Id);
        });
    }

    internal void ResetRuntime()
    {
        _direction = RailgunDirection.None;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(RailgunDirection direction, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick)
    {
        _direction = direction;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new RailgunSubsystemEvent(Controllable, Slot, Status, _direction, _consumedEnergyThisTick, _consumedIonsThisTick,
            _consumedNeutrinosThisTick);
    }
}
