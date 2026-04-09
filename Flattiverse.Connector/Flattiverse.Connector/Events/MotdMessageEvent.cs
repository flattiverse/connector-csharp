using System.Text;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Server-originated message of the day shown during login.
/// </summary>
public class MotdMessageEvent : SystemChatEvent
{
    internal MotdMessageEvent(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.MotdMessage;

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        bool first = true;

        foreach (string rawLine in Message.Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');

            if (first)
            {
                builder.Append($"{Stamp:HH:mm:ss.fff} [MOTD] ");
                first = false;
            }
            else
                builder.Append("\n              [MOTD] ");

            builder.Append(line);
        }

        return builder.ToString();
    }
}
