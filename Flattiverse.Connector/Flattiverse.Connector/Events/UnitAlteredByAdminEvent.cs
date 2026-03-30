namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a previously known unit was changed by admin map editing.
/// This event is a cache invalidation hint, not a full replacement unit snapshot.
/// </summary>
public class UnitAlteredByAdminEvent : FlattiverseEvent
{
    /// <summary>
    /// The cluster id of the altered unit.
    /// </summary>
    public readonly byte ClusterId;

    /// <summary>
    /// The name of the altered unit.
    /// </summary>
    public readonly string Name;

    internal UnitAlteredByAdminEvent(byte clusterId, string name) : base()
    {
        ClusterId = clusterId;
        Name = name;
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.UnitAlteredByAdmin;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Unit altered by admin: ClusterId={ClusterId}, Name=\"{Name}\".";
    }
}
