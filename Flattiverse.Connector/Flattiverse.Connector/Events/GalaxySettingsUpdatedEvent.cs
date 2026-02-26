using System.Text;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when the server updates galaxy settings.
/// </summary>
public class GalaxySettingsUpdatedEvent : FlattiverseEvent
{
    /// <summary>
    /// The previous settings snapshot. Null when this is the first update.
    /// </summary>
    public readonly GalaxySettingsSnapshot? Old;

    /// <summary>
    /// The latest settings snapshot after the update.
    /// </summary>
    public readonly GalaxySettingsSnapshot New;

    internal GalaxySettingsUpdatedEvent(GalaxySettingsSnapshot? oldSettings, GalaxySettingsSnapshot newSettings)
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
                $"GalaxyMaxTotalShips={New.GalaxyMaxTotalShips}, GalaxyMaxClassicShips={New.GalaxyMaxClassicShips}, GalaxyMaxNewShips={New.GalaxyMaxNewShips}, GalaxyMaxBases={New.GalaxyMaxBases}, " +
                $"TeamMaxTotalShips={New.TeamMaxTotalShips}, TeamMaxClassicShips={New.TeamMaxClassicShips}, TeamMaxNewShips={New.TeamMaxNewShips}, TeamMaxBases={New.TeamMaxBases}, " +
                $"PlayerMaxTotalShips={New.PlayerMaxTotalShips}, PlayerMaxClassicShips={New.PlayerMaxClassicShips}, PlayerMaxNewShips={New.PlayerMaxNewShips}, PlayerMaxBases={New.PlayerMaxBases}.";

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

        if (oldSettings.GalaxyMaxNewShips != New.GalaxyMaxNewShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GalaxyMaxNewShips={oldSettings.GalaxyMaxNewShips}->{New.GalaxyMaxNewShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.GalaxyMaxBases != New.GalaxyMaxBases)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"GalaxyMaxBases={oldSettings.GalaxyMaxBases}->{New.GalaxyMaxBases}");
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

        if (oldSettings.TeamMaxNewShips != New.TeamMaxNewShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"TeamMaxNewShips={oldSettings.TeamMaxNewShips}->{New.TeamMaxNewShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.TeamMaxBases != New.TeamMaxBases)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"TeamMaxBases={oldSettings.TeamMaxBases}->{New.TeamMaxBases}");
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

        if (oldSettings.PlayerMaxNewShips != New.PlayerMaxNewShips)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"PlayerMaxNewShips={oldSettings.PlayerMaxNewShips}->{New.PlayerMaxNewShips}");
            appendedAtLeastOneChange = true;
        }

        if (oldSettings.PlayerMaxBases != New.PlayerMaxBases)
        {
            if (appendedAtLeastOneChange)
                builder.Append(", ");

            builder.Append($"PlayerMaxBases={oldSettings.PlayerMaxBases}->{New.PlayerMaxBases}");
            appendedAtLeastOneChange = true;
        }

        if (!appendedAtLeastOneChange)
            return $"{Stamp:HH:mm:ss.fff} Galaxy settings updated without effective field changes.";

        builder.Append('.');
        return builder.ToString();
    }
}
