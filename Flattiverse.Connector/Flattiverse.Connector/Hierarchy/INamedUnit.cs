namespace Flattiverse.Connector.Hierarchy;

/// <summary>
/// Interface for a unit with a name.
/// </summary>
public interface INamedUnit
{
    /// <summary>
    /// The name of the unit.
    /// </summary>
    string Name { get; }
}