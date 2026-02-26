using System.Text;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a team has been updated.
/// </summary>
public class TeamUpdatedEvent : FlattiverseEvent
{
    /// <summary>
    /// Team snapshot before the update.
    /// </summary>
    public readonly TeamSnapshot Old;

    /// <summary>
    /// Team snapshot after the update.
    /// </summary>
    public readonly TeamSnapshot New;

    internal TeamUpdatedEvent(TeamSnapshot oldTeam, TeamSnapshot newTeam)
    {
        Old = oldTeam;
        New = newTeam;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TeamUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder($"{Stamp:HH:mm:ss.fff} Team updated: Id={New.Id}, ");
        bool appendedAtLeastOneChange = false;

        if (Old.Name != New.Name)
        {
            builder.Append($"Name=\"{Old.Name}\"->\"{New.Name}\"");
            appendedAtLeastOneChange = true;
        }

        if (Old.Red != New.Red)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Red={Old.Red}->{New.Red}");
            appendedAtLeastOneChange = true;
        }

        if (Old.Green != New.Green)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Green={Old.Green}->{New.Green}");
            appendedAtLeastOneChange = true;
        }

        if (Old.Blue != New.Blue)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Blue={Old.Blue}->{New.Blue}");
            appendedAtLeastOneChange = true;
        }

        if (!appendedAtLeastOneChange)
            return $"{Stamp:HH:mm:ss.fff} Team updated without effective field changes: Id={New.Id}.";
        
        builder.Append('.');
        return builder.ToString();
    }
}
