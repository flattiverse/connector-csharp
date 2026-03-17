namespace Flattiverse.Connector;

/// <summary>
/// Runtime state of a subsystem for the current server tick.
/// </summary>
public enum SubsystemStatus : byte
{
    /// <summary>
    /// The subsystem was off and therefore did not act.
    /// </summary>
    Off = 0x00,

    /// <summary>
    /// The subsystem was enabled and successfully performed its work.
    /// </summary>
    Worked = 0x01,

    /// <summary>
    /// The subsystem was enabled but failed, typically because resources were missing.
    /// </summary>
    Failed = 0x02,

    /// <summary>
    /// The subsystem is currently upgrading and therefore unavailable.
    /// </summary>
    Upgrading = 0x03,
}
