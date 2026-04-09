using Flattiverse.Connector.Network;

using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Hull integrity subsystem of a controllable.
/// </summary>
public class HullSubsystem : Subsystem
{
    private float _maximum;
    private float _current;

    internal HullSubsystem(Controllable controllable, string name, bool exists, float maximum, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximum = 0f;
        _current = 0f;

        SetMaximum(maximum);
    }

    internal HullSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximum = 0f;
        _current = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable hull state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximum) ||
            !reader.Read(out float current) ||
            !reader.Read(out byte status))
            throw new InvalidDataException("Couldn't read controllable hull state.");

        SetMaximum(maximum);
        UpdateRuntime(current, (SubsystemStatus)status);
        SetReportedTier(tier);
    }

    internal static HullSubsystem CreateClassicShipHull(Controllable controllable)
    {
        return new HullSubsystem(controllable, "Hull", true, 50f, SubsystemSlot.Hull);
    }

    /// <summary>
    /// The maximum hull integrity.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The current hull integrity.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    internal void SetMaximum(float maximum)
    {
        _maximum = Exists ? maximum : 0f;
        RefreshTier();

        if (_current > _maximum)
            _current = _maximum;
    }

    internal void ResetRuntime()
    {
        _current = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float current, SubsystemStatus status)
    {
        _current = current;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float current) || !reader.Read(out byte status))
            return false;

        UpdateRuntime(current, (SubsystemStatus)status);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new HullSubsystemEvent(Controllable, Slot, Status, _current);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        for (byte tier = 1; tier <= ShipUpgradeBalancing.GetMaximumTier(Slot); tier++)
        {
            ShipBalancing.GetHull(tier, out float maximum, out float load);

            if (Matches(_maximum, maximum))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
