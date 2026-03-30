using System.Reflection;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Reflection-built lookup table that dispatches incoming packets to private <see cref="GalaxyHierarchy.Galaxy" /> handlers.
/// </summary>
class CommandRouter
{
    private Galaxy _galaxy;

    private CommandEntry?[] _commands;
    
    /// <summary>
    /// Builds the reflected incoming-command dispatch table for one galaxy instance.
    /// </summary>
    /// <param name="galaxy">Galaxy instance whose private handler methods are scanned.</param>
    public CommandRouter(Galaxy galaxy)
    {
        _galaxy = galaxy;

        _commands = new CommandEntry?[256];

        foreach (MethodInfo methodInfo in typeof(Galaxy).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            Command? command = methodInfo.GetCustomAttribute<Command>();

            if (command is null)
                continue;

            _commands[command.CommandCode] = new CommandEntry(galaxy, command.CommandCode, methodInfo);
        }
    }

    /// <summary>
    /// Dispatches the current incoming packet to its reflected handler when one exists.
    /// </summary>
    /// <param name="reader">Reader positioned at the incoming packet.</param>
    /// <returns><see langword="true" /> if a matching handler existed; otherwise <see langword="false" />.</returns>
    public bool Call(PacketReader reader)
    {
        CommandEntry? entry = _commands[reader.Command];
        
        if (entry is null)
            return false;
        
        return entry.Call(reader);
    }
}
