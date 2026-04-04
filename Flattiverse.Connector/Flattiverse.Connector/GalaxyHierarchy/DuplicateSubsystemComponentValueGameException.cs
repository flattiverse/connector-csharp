namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown when one subsystem-metadata usage evaluation receives the same component kind more than once.
/// </summary>
public class DuplicateSubsystemComponentValueGameException : GameException
{
    /// <summary>
    /// The duplicated component kind.
    /// </summary>
    public readonly SubsystemComponentKind ComponentKind;

    internal DuplicateSubsystemComponentValueGameException(SubsystemComponentKind componentKind)
        : base(0x40, $"[0x40] The subsystem component \"{componentKind}\" was supplied more than once.")
    {
        ComponentKind = componentKind;
    }
}
