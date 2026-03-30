using System.Linq;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event emitted when a switch changes the state of one or more gates.
/// </summary>
public class GateSwitchedEvent : FlattiverseEvent
{
    /// <summary>
    /// Cluster containing the switch and gates.
    /// </summary>
    public readonly Cluster Cluster;

    /// <summary>
    /// Optional player who triggered the switch.
    /// </summary>
    public readonly Player? InvokerPlayer;

    /// <summary>
    /// Optional controllable that triggered the switch.
    /// </summary>
    public readonly ControllableInfo? InvokerControllableInfo;

    /// <summary>
    /// Name of the triggered switch.
    /// </summary>
    public readonly string SwitchName;

    /// <summary>
    /// Final states of the affected gates.
    /// </summary>
    public readonly GateStateChange[] Gates;

    internal GateSwitchedEvent(Cluster cluster, string switchName, Player? invokerPlayer, ControllableInfo? invokerControllableInfo,
        GateStateChange[] gates)
    {
        Cluster = cluster;
        SwitchName = switchName;
        InvokerPlayer = invokerPlayer;
        InvokerControllableInfo = invokerControllableInfo;
        Gates = gates;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GateSwitched;

    /// <inheritdoc/>
    public override string ToString()
    {
        string gateText = Gates.Length == 0 ? "no linked gates" : string.Join(", ", Gates.Select(static gate => gate.ToString()));

        if (InvokerPlayer is null || InvokerControllableInfo is null)
            return $"{Stamp:HH:mm:ss.fff} Switch \"{SwitchName}\" in cluster {Cluster.Name} changed {gateText}.";

        return $"{Stamp:HH:mm:ss.fff} Switch \"{SwitchName}\" in cluster {Cluster.Name} changed {gateText} by {InvokerPlayer.Name} / {InvokerControllableInfo.Name}.";
    }
}
