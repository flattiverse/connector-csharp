using System.Reflection;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Network;

/// <summary>
/// Reflection-backed dispatch entry for one incoming packet command.
/// </summary>
class CommandEntry
{
    /// <summary>
    /// Incoming protocol command byte handled by this entry.
    /// </summary>
    public readonly byte Command;

    private List<CommandParameter> _parameters;
    private object?[] _executedParameters;

    private readonly MethodInfo _methodInfo;
    private readonly Galaxy _galaxy;

    /// <summary>
    /// Creates one reflection-backed dispatch entry for an incoming command handler.
    /// </summary>
    /// <param name="galaxy">Galaxy instance on which the handler will be invoked.</param>
    /// <param name="command">Handled protocol command byte.</param>
    /// <param name="methodInfo">Handler method metadata.</param>
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

    /// <summary>
    /// Reads all handler parameters from the current packet and invokes the reflected handler method.
    /// </summary>
    /// <param name="reader">Reader positioned at the incoming packet.</param>
    /// <returns>Always <see langword="true" /> if parameter binding and invocation succeeded.</returns>
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
