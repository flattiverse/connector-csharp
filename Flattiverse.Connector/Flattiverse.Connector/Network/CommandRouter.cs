using System.Reflection;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Network;

class CommandRouter
{
    private Galaxy _galaxy;

    private CommandEntry?[] _commands;
    
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

    public bool Call(PacketReader reader)
    {
        CommandEntry? entry = _commands[reader.Command];

        if (entry is null)
            return false;
        
        return entry.Call(reader);
    }
}