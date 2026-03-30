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

    internal void Update(bool exists, float energyCost)
    {
        _exists = exists;
        _energyCost = exists ? energyCost : 0f;
    }
}
