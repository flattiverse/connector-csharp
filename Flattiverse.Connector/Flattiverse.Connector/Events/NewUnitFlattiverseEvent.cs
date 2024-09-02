using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// An event which informs about a new unit.
/// </summary>
public class NewUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal NewUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }
    
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.NewUnit;

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Unit.Team is null)
            return
                $"{Stamp:HH:mm:ss.fff} New Unit in cluster {Unit.Cluster.Name} of Kind {Unit.Kind} with name {Unit.Name} on position {Unit.Position} and with radius {Unit.Radius:F} and gravity {Unit.Gravity:0.000}.";

        return
            $"{Stamp:HH:mm:ss.fff} New Unit in cluster {Unit.Cluster.Name} and with team {Unit.Team.Name} of Kind {Unit.Kind} with name {Unit.Name} on position {Unit.Position} and with radius {Unit.Radius:F} and gravity {Unit.Gravity:0.000}.";
    }
}