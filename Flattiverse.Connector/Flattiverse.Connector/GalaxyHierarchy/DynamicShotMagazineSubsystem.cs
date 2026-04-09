using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic shot magazine subsystem of a controllable.
/// </summary>
public class DynamicShotMagazineSubsystem : Subsystem
{
    private float _maximumShots;
    private float _currentShots;

    internal DynamicShotMagazineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximumShots = 0f;
        _currentShots = 0f;
    }

    internal DynamicShotMagazineSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximumShots = 0f;
        _currentShots = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumShots) ||
            !reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetMaximumShots(maximumShots);
        UpdateRuntime(currentShots, (SubsystemStatus)status);
        SetReportedTier(tier);
    }

    /// <summary>
    /// The magazine capacity in shots.
    /// </summary>
    public float MaximumShots
    {
        get { return _maximumShots; }
    }

    /// <summary>
    /// The currently stored shots.
    /// </summary>
    public float CurrentShots
    {
        get { return _currentShots; }
    }

    internal void ResetRuntime()
    {
        _currentShots = 0f;
        ResetRuntimeStatus();
    }

    internal void SetMaximumShots(float maximumShots)
    {
        _maximumShots = Exists ? maximumShots : 0f;

        RefreshTier();
    }

    internal void UpdateRuntime(float currentShots, SubsystemStatus status)
    {
        _currentShots = currentShots;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        UpdateRuntime(currentShots, (SubsystemStatus)status);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicShotMagazineSubsystemEvent(Controllable, Slot, Status, _currentShots);
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
            float maximumShots;
            float startingShots;
            float load;

            if (Slot is SubsystemSlot.DynamicShotMagazine or SubsystemSlot.DynamicInterceptorMagazine)
            {
                if (this is DynamicInterceptorMagazineSubsystem or StaticInterceptorMagazineSubsystem)
                    ShipBalancing.GetDynamicInterceptorMagazine(tier, out maximumShots, out startingShots, out load);
                else
                    ShipBalancing.GetDynamicShotMagazine(tier, out maximumShots, out startingShots, out load);
            }
            else
            {
                if (this is DynamicInterceptorMagazineSubsystem or StaticInterceptorMagazineSubsystem)
                    ShipBalancing.GetStaticInterceptorMagazine(tier, out maximumShots, out startingShots, out load);
                else
                    ShipBalancing.GetStaticShotMagazine(tier, out maximumShots, out startingShots, out load);
            }

            if (Matches(_maximumShots, maximumShots))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
