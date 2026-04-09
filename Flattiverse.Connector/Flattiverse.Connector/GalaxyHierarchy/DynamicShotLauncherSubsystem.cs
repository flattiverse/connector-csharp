using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic projectile launcher subsystem of a controllable.
/// </summary>
public class DynamicShotLauncherSubsystem : Subsystem
{
    private float _minimumRelativeMovement;
    private float _maximumRelativeMovement;
    private ushort _minimumTicks;
    private ushort _maximumTicks;
    private float _minimumLoad;
    private float _maximumLoad;
    private float _minimumDamage;
    private float _maximumDamage;

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
        _minimumRelativeMovement = 0.1f;
        _maximumRelativeMovement = 3f;
        _minimumTicks = 2;
        _maximumTicks = 140;
        _minimumLoad = 2.5f;
        _maximumLoad = 25f;
        _minimumDamage = 1f;
        _maximumDamage = 20f;
        _relativeMovement = new Vector();
        _ticks = 0;
        _load = 0f;
        _damage = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal DynamicShotLauncherSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _minimumRelativeMovement = 0f;
        _maximumRelativeMovement = 0f;
        _minimumTicks = 0;
        _maximumTicks = 0;
        _minimumLoad = 0f;
        _maximumLoad = 0f;
        _minimumDamage = 0f;
        _maximumDamage = 0f;
        _relativeMovement = new Vector();
        _ticks = 0;
        _load = 0f;
        _damage = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float minimumRelativeMovement) ||
            !reader.Read(out float maximumRelativeMovement) ||
            !reader.Read(out ushort minimumTicks) ||
            !reader.Read(out ushort maximumTicks) ||
            !reader.Read(out float minimumLoad) ||
            !reader.Read(out float maximumLoad) ||
            !reader.Read(out float minimumDamage) ||
            !reader.Read(out float maximumDamage) ||
            !Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetCapabilities(minimumRelativeMovement, maximumRelativeMovement, minimumTicks, maximumTicks, minimumLoad, maximumLoad,
            minimumDamage, maximumDamage);
        UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    /// <summary>
    /// The minimum allowed relative shot speed.
    /// </summary>
    public float MinimumRelativeMovement
    {
        get { return _minimumRelativeMovement; }
    }

    /// <summary>
    /// The maximum allowed relative shot speed.
    /// </summary>
    public float MaximumRelativeMovement
    {
        get { return _maximumRelativeMovement; }
    }

    /// <summary>
    /// The minimum allowed shot lifetime in ticks.
    /// </summary>
    public ushort MinimumTicks
    {
        get { return _minimumTicks; }
    }

    /// <summary>
    /// The maximum allowed shot lifetime in ticks.
    /// </summary>
    public ushort MaximumTicks
    {
        get { return _maximumTicks; }
    }

    /// <summary>
    /// The minimum allowed shot load.
    /// </summary>
    public float MinimumLoad
    {
        get { return _minimumLoad; }
    }

    /// <summary>
    /// The maximum allowed shot load.
    /// </summary>
    public float MaximumLoad
    {
        get { return _maximumLoad; }
    }

    /// <summary>
    /// The minimum allowed shot damage.
    /// </summary>
    public float MinimumDamage
    {
        get { return _minimumDamage; }
    }

    /// <summary>
    /// The maximum allowed shot damage.
    /// </summary>
    public float MaximumDamage
    {
        get { return _maximumDamage; }
    }

    internal void SetCapabilities(float minimumRelativeMovement, float maximumRelativeMovement, ushort minimumTicks, ushort maximumTicks,
        float minimumLoad, float maximumLoad, float minimumDamage, float maximumDamage)
    {
        _minimumRelativeMovement = Exists ? minimumRelativeMovement : 0f;
        _maximumRelativeMovement = Exists ? maximumRelativeMovement : 0f;
        _minimumTicks = minimumTicks;
        _maximumTicks = maximumTicks;
        _minimumLoad = Exists ? minimumLoad : 0f;
        _maximumLoad = Exists ? maximumLoad : 0f;
        _minimumDamage = Exists ? minimumDamage : 0f;
        _maximumDamage = Exists ? maximumDamage : 0f;

        RefreshTier();
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

        if (RangeTolerance.ClampRange(relativeMovement, _minimumRelativeMovement, _maximumRelativeMovement, out relativeMovement) != InvalidArgumentKind.Valid)
            return false;

        if (ticks < _minimumTicks || ticks > _maximumTicks)
            return false;

        if (RangeTolerance.ClampRange(load, _minimumLoad, _maximumLoad, out load) != InvalidArgumentKind.Valid)
            return false;

        if (RangeTolerance.ClampRange(damage, _minimumDamage, _maximumDamage, out damage) != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateShotLaunchEnergy(relativeMovement.Length, ticks, load, damage);

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

        InvalidArgumentKind movementValidity = RangeTolerance.ClampRange(relativeMovement, _minimumRelativeMovement, _maximumRelativeMovement, out relativeMovement);

        if (movementValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(movementValidity, "relativeMovement");

        if (ticks < _minimumTicks)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "ticks");

        if (ticks > _maximumTicks)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "ticks");

        InvalidArgumentKind loadValidity = RangeTolerance.ClampRange(load, _minimumLoad, _maximumLoad, out load);

        if (loadValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(loadValidity, "load");

        InvalidArgumentKind damageValidity = RangeTolerance.ClampRange(damage, _minimumDamage, _maximumDamage, out damage);

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

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicShotLauncherSubsystemEvent(Controllable, Slot, Status, _relativeMovement, _ticks, _load, _damage,
            _consumedEnergyThisTick, _consumedIonsThisTick, _consumedNeutrinosThisTick);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        byte maximumTier = (byte)(TierInfos.Count - 1);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumRelativeMovement;
            ushort maximumTicks;
            float maximumLoad;
            float maximumDamage;

            if (Slot is SubsystemSlot.DynamicShotLauncher)
                ShipBalancing.GetDynamicShotLauncher(tier, out maximumRelativeMovement, out maximumTicks, out maximumLoad,
                    out maximumDamage, out float _);
            else
                ShipBalancing.GetStaticShotLauncher(tier, out maximumRelativeMovement, out maximumTicks, out maximumLoad,
                    out maximumDamage, out float _);

            if (Matches(_maximumRelativeMovement, maximumRelativeMovement) && _maximumTicks == maximumTicks
                && Matches(_maximumLoad, maximumLoad) && Matches(_maximumDamage, maximumDamage))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
