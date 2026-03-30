namespace Flattiverse.Connector;

/// <summary>
/// General-purpose helpers used across the connector packages.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Validates a Flattiverse name against the connector's local name rules.
    /// </summary>
    /// <param name="name">Candidate name to validate.</param>
    /// <returns>
    /// <see langword="true" /> if the name satisfies the local length, whitespace, and character-set rules;
    /// otherwise <see langword="false" />.
    /// </returns>
    public static bool CheckName(string? name)
    {
        if (name is null || name.Length < 2 || name.Length > 32)
            return false;

        if (name.StartsWith(' ') || name.EndsWith(' '))
            return false;

        foreach (char c in name)
        {
            if (c >= 'a' && c <= 'z')
                continue;

            if (c >= 'A' && c <= 'Z')
                continue;

            if (c >= '0' && c <= '9')
                continue;

            if (c >= 192 && c <= 214)
                continue;

            if (c >= 216 && c <= 246)
                continue;

            if (c >= 248 && c <= 687)
                continue;

            if (c == ' ' || c == '.' || c == '_' || c == '-')
                continue;

            return false;
        }

        return true;
    }
}
