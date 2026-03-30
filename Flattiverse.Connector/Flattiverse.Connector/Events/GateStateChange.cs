namespace Flattiverse.Connector.Events;

/// <summary>
/// Final state of one gate after a switch action.
/// </summary>
public class GateStateChange
{
    /// <summary>
    /// Name of the affected gate.
    /// </summary>
    public readonly string GateName;

    /// <summary>
    /// Final closed state.
    /// </summary>
    public readonly bool Closed;

    internal GateStateChange(string gateName, bool closed)
    {
        GateName = gateName;
        Closed = closed;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{GateName}={(Closed ? "Closed" : "Open")}";
    }
}
