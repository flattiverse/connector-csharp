namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when the server announces the compile profile it was built with.
/// </summary>
public class CompiledWithMessageEvent : FlattiverseEvent
{
    /// <summary>
    /// The maximum amount of players supported by this server binary.
    /// </summary>
    public readonly byte MaxPlayersSupported;

    /// <summary>
    /// The compile symbol that selected the server profile.
    /// </summary>
    public readonly string Symbol;

    /// <summary>
    /// A user-facing message describing the compile profile.
    /// </summary>
    public readonly string Message;

    internal CompiledWithMessageEvent(byte maxPlayersSupported, string symbol)
    {
        MaxPlayersSupported = maxPlayersSupported;
        Symbol = symbol;
        Message = $"This server has been compiled with support for up to {MaxPlayersSupported} players ({Symbol}).";
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.CompiledWithMessage;

    /// <inheritdoc />
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} {Message}";
}
