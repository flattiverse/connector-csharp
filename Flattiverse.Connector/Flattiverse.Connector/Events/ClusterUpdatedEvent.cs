using System.Text;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a cluster has been updated.
/// </summary>
public class ClusterUpdatedEvent : FlattiverseEvent
{
    /// <summary>
    /// Cluster snapshot before the update.
    /// </summary>
    public readonly ClusterSnapshot Old;

    /// <summary>
    /// Cluster snapshot after the update.
    /// </summary>
    public readonly ClusterSnapshot New;

    internal ClusterUpdatedEvent(ClusterSnapshot oldCluster, ClusterSnapshot newCluster)
    {
        Old = oldCluster;
        New = newCluster;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ClusterUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder($"{Stamp:HH:mm:ss.fff} Cluster updated: Id={New.Id}, ");
        bool appendedAtLeastOneChange = false;

        if (Old.Name != New.Name)
        {
            builder.Append($"Name=\"{Old.Name}\"->\"{New.Name}\"");
            appendedAtLeastOneChange = true;
        }

        if (Old.Active != New.Active)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Active={Old.Active}->{New.Active}");
            appendedAtLeastOneChange = true;
        }

        if (Old.Start != New.Start)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Start={Old.Start}->{New.Start}");
            appendedAtLeastOneChange = true;
        }

        if (Old.Respawn != New.Respawn)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Respawn={Old.Respawn}->{New.Respawn}");
            appendedAtLeastOneChange = true;
        }

        if (!appendedAtLeastOneChange)
            return $"{Stamp:HH:mm:ss.fff} Cluster updated without effective field changes: Id={New.Id}.";

        builder.Append('.');
        return builder.ToString();
    }
}
