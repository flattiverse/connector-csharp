namespace Flattiverse.Connector.Events;

/// <summary>
/// Snapshot of a team state relevant for events.
/// </summary>
public readonly struct TeamSnapshot
{
    /// <summary>
    /// Team id.
    /// </summary>
    public readonly byte Id;

    /// <summary>
    /// Team name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Red color component.
    /// </summary>
    public readonly byte Red;

    /// <summary>
    /// Green color component.
    /// </summary>
    public readonly byte Green;

    /// <summary>
    /// Blue color component.
    /// </summary>
    public readonly byte Blue;

    /// <summary>
    /// True if regular players may join this team.
    /// </summary>
    public readonly bool Playable;

    internal TeamSnapshot(byte id, string name, byte red, byte green, byte blue, bool playable)
    {
        Id = id;
        Name = name;
        Red = red;
        Green = green;
        Blue = blue;
        Playable = playable;
    }
}
