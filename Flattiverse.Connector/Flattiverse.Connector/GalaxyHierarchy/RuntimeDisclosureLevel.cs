namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Self-disclosed runtime automation level for one aspect.
/// </summary>
public enum RuntimeDisclosureLevel : byte
{
    /// <summary>
    /// This capability is not implemented.
    /// </summary>
    Unsupported = 0,

    /// <summary>
    /// A human issues the concrete action manually.
    /// </summary>
    Manual = 1,

    /// <summary>
    /// A human acts directly and software assists.
    /// </summary>
    Assisted = 2,

    /// <summary>
    /// Software executes a concrete user-selected target automatically.
    /// </summary>
    Automated = 3,

    /// <summary>
    /// Software acts autonomously within a broader mission or policy.
    /// </summary>
    Autonomous = 4,

    /// <summary>
    /// An AI system controls the aspect.
    /// </summary>
    AiControlled = 5,
}
