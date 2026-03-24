using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic projectile launcher subsystem of a controllable.
/// </summary>
public class DynamicShotLauncherSubsystem : Subsystem
{
    private const float RelativeMovementMinimum = 0.1f;
    private const float RelativeMovementMaximum = 3f;
    private const ushort TicksMinimum = 2;
    private const ushort TicksMaximum = 140;
    private const float LoadMinimum = 2.5f;
    private const float LoadMaximum = 25f;
    private const float DamageMinimum = 1f;
    private const float DamageMaximum = 20f;

    private Vector _relativeMovement;
    private ushort _ticks;
    private float _load;
    private float _damage;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal DynamicShotLauncherSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _relativeMovement = new Vector();
        _ticks = 0;
        _load = 0f;
        _damage = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    /// <summary>
    /// The minimum allowed relative shot speed.
    /// </summary>
    public float MinimumRelativeMovement
    {
        get { return RelativeMovementMinimum; }
    }

    /// <summary>
    /// The maximum allowed relative shot speed.
    /// </summary>
    public float MaximumRelativeMovement
    {
        get { return RelativeMovementMaximum; }
    }

    /// <summary>
    /// The minimum allowed shot lifetime in ticks.
    /// </summary>
    public ushort MinimumTicks
    {
        get { return TicksMinimum; }
    }

    /// <summary>
    /// The maximum allowed shot lifetime in ticks.
    /// </summary>
    public ushort MaximumTicks
    {
        get { return TicksMaximum; }
    }

    /// <summary>
    /// The minimum allowed shot load.
    /// </summary>
    public float MinimumLoad
    {
        get { return LoadMinimum; }
    }

    /// <summary>
    /// The maximum allowed shot load.
    /// </summary>
    public float MaximumLoad
    {
        get { return LoadMaximum; }
    }

    /// <summary>
    /// The minimum allowed shot damage.
    /// </summary>
    public float MinimumDamage
    {
        get { return DamageMinimum; }
    }

    /// <summary>
    /// The maximum allowed shot damage.
    /// </summary>
    public float MaximumDamage
    {
        get { return DamageMaximum; }
    }

    /// <summary>
    /// The last server-side shot movement request processed for the current tick.
    /// </summary>
    public Vector RelativeMovement
    {
        get { return new Vector(_relativeMovement); }
    }

    /// <summary>
    /// The last server-side shot lifetime processed for the current tick.
    /// </summary>
    public ushort Ticks
    {
        get { return _ticks; }
    }

    /// <summary>
    /// The last server-side shot load processed for the current tick.
    /// </summary>
    public float Load
    {
        get { return _load; }
    }

    /// <summary>
    /// The last server-side shot damage processed for the current tick.
    /// </summary>
    public float Damage
    {
        get { return _damage; }
    }

    /// <summary>
    /// The energy consumed by the launcher during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed by the launcher during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed by the launcher during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the resource costs for one dynamic shot request.
    /// </summary>
    /// <param name="relativeMovement">The relative shot movement vector.</param>
    /// <param name="ticks">The shot lifetime in ticks.</param>
    /// <param name="load">The explosion load applied when the shot expires.</param>
    /// <param name="damage">The damage the shot should inflict.</param>
    /// <param name="energy">The resulting energy cost.</param>
    /// <param name="ions">The resulting ion cost.</param>
    /// <param name="neutrinos">The resulting neutrino cost.</param>
    /// <returns><see langword="true"/> if the input is valid; otherwise <see langword="false"/>.</returns>
    public bool CalculateCost(Vector relativeMovement, ushort ticks, float load, float damage, out float energy, out float ions,
        out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampRange(relativeMovement, RelativeMovementMinimum, RelativeMovementMaximum, out relativeMovement) != InvalidArgumentKind.Valid)
            return false;

        if (ticks < TicksMinimum || ticks > TicksMaximum)
            return false;

        if (RangeTolerance.ClampRange(load, LoadMinimum, LoadMaximum, out load) != InvalidArgumentKind.Valid)
            return false;

        if (RangeTolerance.ClampRange(damage, DamageMinimum, DamageMaximum, out damage) != InvalidArgumentKind.Valid)
            return false;

        float speed = relativeMovement.Length;
        float speed01 = (speed - RelativeMovementMinimum) / (RelativeMovementMaximum - RelativeMovementMinimum);
        float ticks01 = (ticks - TicksMinimum) / (float)(TicksMaximum - TicksMinimum);
        float load01 = (load - LoadMinimum) / (LoadMaximum - LoadMinimum);
        float damage01 = (damage - DamageMinimum) / (DamageMaximum - DamageMinimum);

        energy = 10f
            + 250f * speed01 * speed01 * speed01
            + 240f * ticks01 * ticks01
            + 600f * load01 * load01
            + 700f * damage01 * damage01;

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Requests one shot for the next server tick.
    /// The vector length, load and damage values are clipped when they are only slightly outside the configured range.
    /// The tick count is not clipped.
    /// </summary>
    /// <param name="relativeMovement">
    /// The relative shot movement vector. Its length must be in the range [0.1f; 3f].
    /// </param>
    /// <param name="ticks">The shot lifetime in ticks. [2; 140]</param>
    /// <param name="load">The explosion load applied when the shot expires. [2.5f; 25f]</param>
    /// <param name="damage">The damage the shot should inflict. [1f; 20f]</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if an argument is invalid.</exception>
    public async Task Shoot(Vector relativeMovement, ushort ticks, float load, float damage)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind movementValidity = RangeTolerance.ClampRange(relativeMovement, RelativeMovementMinimum, RelativeMovementMaximum, out relativeMovement);

        if (movementValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(movementValidity, "relativeMovement");

        if (ticks < TicksMinimum)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "ticks");

        if (ticks > TicksMaximum)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "ticks");

        InvalidArgumentKind loadValidity = RangeTolerance.ClampRange(load, LoadMinimum, LoadMaximum, out load);

        if (loadValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(loadValidity, "load");

        InvalidArgumentKind damageValidity = RangeTolerance.ClampRange(damage, DamageMinimum, DamageMaximum, out damage);

        if (damageValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(damageValidity, "damage");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x88;
            writer.Write(Controllable.Id);
            relativeMovement.Write(ref writer);
            writer.Write(ticks);
            writer.Write(load);
            writer.Write(damage);
        });
    }

    internal void ResetRuntime()
    {
        _relativeMovement = new Vector();
        _ticks = 0;
        _load = 0f;
        _damage = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(Vector relativeMovement, ushort ticks, float load, float damage, SubsystemStatus status,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _relativeMovement = relativeMovement;
        _ticks = ticks;
        _load = load;
        _damage = damage;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicShotLauncherSubsystemEvent(Controllable, Slot, Status, _relativeMovement, _ticks, _load, _damage,
            _consumedEnergyThisTick, _consumedIonsThisTick, _consumedNeutrinosThisTick);
    }
}
