namespace Flattiverse.Connector.Network;

/// <summary>
/// Attribute that marks one private <see cref="GalaxyHierarchy.Galaxy" /> method as an incoming packet handler.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
class Command : Attribute
{
    /// <summary>
    /// Incoming protocol command byte handled by the annotated method.
    /// </summary>
    public readonly byte CommandCode;

    /// <summary>
    /// Marks one method as the handler for the specified incoming protocol command.
    /// </summary>
    /// <param name="command">Handled protocol command byte.</param>
    public Command(byte command)
    {
        CommandCode = command;
    }
}
