using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Railgun subsystem of a controllable.
/// </summary>
public class ClassicRailgunSubsystem : Subsystem
{
    private float _projectileSpeed;
    private ushort _projectileLifetime;
    private float _energyCost;
    private float _metalCost;

    private RailgunDirection _direction;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ClassicRailgunSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) : base(controllable, name, exists, slot)
    {
        _projectileSpeed = 4f;
        _projectileLifetime = 250;
        _energyCost = 300f;
        _metalCost = 1f;
        _direction = RailgunDirection.None;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ClassicRailgunSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _projectileSpeed = 0f;
        _projectileLifetime = 0;
        _energyCost = 0f;
        _metalCost = 0f;
        _direction = RailgunDirection.None;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float projectileSpeed) ||
            !reader.Read(out ushort projectileLifetime) ||
            !reader.Read(out float energyCost) ||
            !reader.Read(out float metalCost) ||
            !reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetCapabilities(projectileSpeed, projectileLifetime, energyCost, metalCost);
        UpdateRuntime((RailgunDirection)direction, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    /// <summary>
    /// Rail projectile relative speed.
    /// </summary>
    public float ProjectileSpeed
    {
        get { return _projectileSpeed; }
    }

    /// <summary>
    /// Rail projectile lifetime in ticks.
    /// </summary>
    public ushort ProjectileLifetime
    {
        get { return _projectileLifetime; }
    }

    /// <summary>
    /// Energy consumed by one rail shot.
    /// </summary>
    public float EnergyCost
    {
        get { return _energyCost; }
    }

    /// <summary>
    /// Metal consumed by one rail shot.
    /// </summary>
    public float MetalCost
    {
        get { return _metalCost; }
    }

    internal void SetCapabilities(float projectileSpeed, ushort projectileLifetime, float energyCost, float metalCost)
    {
        _projectileSpeed = Exists ? projectileSpeed : 0f;
        _projectileLifetime = Exists ? projectileLifetime : (ushort)0;
        _energyCost = Exists ? energyCost : 0f;
        _metalCost = Exists ? metalCost : 0f;

        RefreshTier();
    }

    internal void CopyFrom(ClassicRailgunSubsystem other)
    {
        CopyBaseFrom(other);
        _projectileSpeed = other._projectileSpeed;
        _projectileLifetime = other._projectileLifetime;
        _energyCost = other._energyCost;
        _metalCost = other._metalCost;
        _direction = other._direction;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
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

        energy = _energyCost;
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

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime((RailgunDirection)direction, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        if (this is ModernRailgunSubsystem)
            return new ModernRailgunSubsystemEvent(Controllable, Slot, Status, _direction, _consumedEnergyThisTick, _consumedIonsThisTick,
                _consumedNeutrinosThisTick);

        return new ClassicRailgunSubsystemEvent(Controllable, Slot, Status, _direction, _consumedEnergyThisTick, _consumedIonsThisTick,
            _consumedNeutrinosThisTick);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        byte maximumTier = (byte)(TierInfos.Count - 1);
        bool modern = this is ModernRailgunSubsystem;

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetRailgun(tier, modern, out float projectileSpeed, out ushort projectileLifetime, out float energyCost,
                out float metalCost, out float load);

            if (Matches(_projectileSpeed, projectileSpeed) && _projectileLifetime == projectileLifetime
                && Matches(_energyCost, energyCost) && Matches(_metalCost, metalCost))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
