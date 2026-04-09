using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a jump-drive subsystem on a scanned player unit.
/// </summary>
public class JumpDriveSubsystemInfo
{
    private bool _exists;
    private float _energyCost;

    internal JumpDriveSubsystemInfo()
    {
        _exists = false;
        _energyCost = 0f;
    }

    internal JumpDriveSubsystemInfo(JumpDriveSubsystemInfo other)
    {
        _exists = other._exists;
        _energyCost = other._energyCost;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Energy required for a single jump activation.
    /// The actual destination depends on the worm hole being used, not on the subsystem itself.
    /// </summary>
    public float EnergyCost
    {
        get { return _energyCost; }
    }

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _energyCost = 0f;
            return true;
        }

        return reader.Read(out _energyCost);
    }
}
