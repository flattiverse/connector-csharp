using System.Text;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the server initializes or updates the mirrored galaxy settings snapshot.
/// </summary>
public class UpdatedGalaxySettingsEvent : FlattiverseEvent
{
    /// <summary>
    /// Previous settings snapshot.
    /// <see langword="null" /> when the connector receives the first settings snapshot after connect.
    /// </summary>
    public readonly GalaxySettingsSnapshot? Old;

    /// <summary>
    /// Latest settings snapshot after the update.
    /// </summary>
    public readonly GalaxySettingsSnapshot New;

    internal UpdatedGalaxySettingsEvent(GalaxySettingsSnapshot? oldSettings, GalaxySettingsSnapshot newSettings)
    {
        Old = oldSettings;
        New = newSettings;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxySettingsUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Old is null)
            return
                $"{Stamp:HH:mm:ss.fff} Galaxy settings initialized: " +
                $"GameMode={New.GameMode}, Name=\"{New.Name}\", Description=\"{New.Description}\", " +
                $"MaxPlayers={New.MaxPlayers}, MaxSpectators={New.MaxSpectators}, " +
                $"GalaxyMaxTotalShips={New.GalaxyMaxTotalShips}, GalaxyMaxClassicShips={New.GalaxyMaxClassicShips}, GalaxyMaxModernShips={New.GalaxyMaxModernShips}, " +
                $"TeamMaxTotalShips={New.TeamMaxTotalShips}, TeamMaxClassicShips={New.TeamMaxClassicShips}, TeamMaxModernShips={New.TeamMaxModernShips}, " +
                $"PlayerMaxTotalShips={New.PlayerMaxTotalShips}, PlayerMaxClassicShips={New.PlayerMaxClassicShips}, PlayerMaxModernShips={New.PlayerMaxModernShips}, Maintenance={New.Maintenance}, RequiresSelfDisclosure={New.RequiresSelfDisclosure}, RequiredAchievement={New.RequiredAchievement ?? "<none>"}.";

        GalaxySettingsSnapshot oldSettings = Old;
        StringBuilder builder = new StringBuilder($"{Stamp:HH:mm:ss.fff} Galaxy settings updated: ");
        bool appendedAtLeastOneChange = false;

        if (oldSettings.GameMode != New.GameMode)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GameMode={oldSettings.GameMode}->{New.GameMode}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.Name != New.Name)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Name=\"{oldSettings.Name}\"->\"{New.Name}\"");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.Description != New.Description)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Description=\"{oldSettings.Description}\"->\"{New.Description}\"");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.MaxPlayers != New.MaxPlayers)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"MaxPlayers={oldSettings.MaxPlayers}->{New.MaxPlayers}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.MaxSpectators != New.MaxSpectators)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"MaxSpectators={oldSettings.MaxSpectators}->{New.MaxSpectators}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.GalaxyMaxTotalShips != New.GalaxyMaxTotalShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GalaxyMaxTotalShips={oldSettings.GalaxyMaxTotalShips}->{New.GalaxyMaxTotalShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.GalaxyMaxClassicShips != New.GalaxyMaxClassicShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GalaxyMaxClassicShips={oldSettings.GalaxyMaxClassicShips}->{New.GalaxyMaxClassicShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.GalaxyMaxModernShips != New.GalaxyMaxModernShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GalaxyMaxModernShips={oldSettings.GalaxyMaxModernShips}->{New.GalaxyMaxModernShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.TeamMaxTotalShips != New.TeamMaxTotalShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"TeamMaxTotalShips={oldSettings.TeamMaxTotalShips}->{New.TeamMaxTotalShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.TeamMaxClassicShips != New.TeamMaxClassicShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"TeamMaxClassicShips={oldSettings.TeamMaxClassicShips}->{New.TeamMaxClassicShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.TeamMaxModernShips != New.TeamMaxModernShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"TeamMaxModernShips={oldSettings.TeamMaxModernShips}->{New.TeamMaxModernShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.PlayerMaxTotalShips != New.PlayerMaxTotalShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"PlayerMaxTotalShips={oldSettings.PlayerMaxTotalShips}->{New.PlayerMaxTotalShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.PlayerMaxClassicShips != New.PlayerMaxClassicShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"PlayerMaxClassicShips={oldSettings.PlayerMaxClassicShips}->{New.PlayerMaxClassicShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.PlayerMaxModernShips != New.PlayerMaxModernShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"PlayerMaxModernShips={oldSettings.PlayerMaxModernShips}->{New.PlayerMaxModernShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.Maintenance != New.Maintenance)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"Maintenance={oldSettings.Maintenance}->{New.Maintenance}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.RequiresSelfDisclosure != New.RequiresSelfDisclosure)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"RequiresSelfDisclosure={oldSettings.RequiresSelfDisclosure}->{New.RequiresSelfDisclosure}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.RequiredAchievement != New.RequiredAchievement)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"RequiredAchievement={oldSettings.RequiredAchievement ?? "<none>"}->{New.RequiredAchievement ?? "<none>"}");
            appendedAtLeastOneChange = true;
        }

        if (!appendedAtLeastOneChange)
            return $"{Stamp:HH:mm:ss.fff} Galaxy settings updated without effective field changes.";

        builder.Append('.');
        return builder.ToString();
    }
}
