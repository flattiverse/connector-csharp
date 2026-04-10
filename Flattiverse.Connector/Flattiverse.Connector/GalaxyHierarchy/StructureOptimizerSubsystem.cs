using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Passive structure-optimizer subsystem of a controllable.
/// </summary>
public class StructureOptimizerSubsystem : Subsystem
{
    private float _reductionPercent;

    internal StructureOptimizerSubsystem(Controllable controllable, bool exists, float reductionPercent) :
        base(controllable, "StructureOptimizer", exists, SubsystemSlot.StructureOptimizer)
    {
        _reductionPercent = 0f;
        SetReductionPercent(reductionPercent);
    }

    internal StructureOptimizerSubsystem(Controllable controllable, PacketReader reader) :
        base(controllable, "StructureOptimizer", false, SubsystemSlot.StructureOptimizer)
    {
        _reductionPercent = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable structure optimizer state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) || !reader.Read(out float reductionPercent))
            throw new InvalidDataException("Couldn't read controllable structure optimizer state.");

        SetReductionPercent(reductionPercent);
        SetReportedTier(tier);
    }

    /// <summary>
    /// Percentage of raw structure load reduced by this subsystem.
    /// </summary>
    public float ReductionPercent
    {
        get { return _reductionPercent; }
    }

    internal void SetReductionPercent(float reductionPercent)
    {
        _reductionPercent = Exists ? reductionPercent : 0f;
        RefreshTier();
    }

    internal void CopyFrom(StructureOptimizerSubsystem other)
    {
        CopyBaseFrom(other);
        _reductionPercent = other._reductionPercent;
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        for (byte tier = 1; tier <= ShipUpgradeBalancing.GetMaximumTier(Slot); tier++)
            if (Matches(_reductionPercent, ShipBalancing.GetStructureOptimizerReductionPercent(tier)))
            {
                SetTier(tier);
                return;
            }

        SetTier(0);
    }
}
