namespace Flattiverse.Connector.Events;

/// <summary>
/// This event informs about a unit that has been altered by an admin through map editing.
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

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.UnitAlteredByAdmin;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Unit altered by admin: ClusterId={ClusterId}, Name=\"{Name}\".";
    }
}
