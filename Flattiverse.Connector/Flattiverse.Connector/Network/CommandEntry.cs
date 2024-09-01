using System.Reflection;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Network;

class CommandEntry
{
    public readonly byte Command;

    private List<CommandParameter> _parameters;
    private object?[] _executedParameters;

    private readonly MethodInfo _methodInfo;
    private readonly Galaxy _galaxy;

    public CommandEntry(Galaxy galaxy, byte command, MethodInfo methodInfo)
    {
        Command = command;

        _methodInfo = methodInfo;
        
        _parameters = new List<CommandParameter>();

        foreach (ParameterInfo parameter in methodInfo.GetParameters())
            _parameters.Add(new CommandParameter(parameter));
        
        _executedParameters = new object[_parameters.Count];

        _galaxy = galaxy;
    }

    public bool Call(PacketReader reader)
    {
        int position = 0;
        
        foreach (CommandParameter parameter in _parameters)
            if (!parameter.TryRead(_galaxy, reader, out _executedParameters[position++]))
                throw new ArgumentException($" * {_methodInfo.Name} failed reading parameter {parameter.Name}.", parameter.Name);

        _methodInfo.Invoke(_galaxy, _executedParameters);

        return true;
    }
}