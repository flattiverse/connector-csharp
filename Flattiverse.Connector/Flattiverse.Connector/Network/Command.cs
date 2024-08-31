namespace Flattiverse.Connector.Network;

[AttributeUsage(AttributeTargets.Method)]
class Command : Attribute
{
    public readonly byte CommandCode;

    public Command(byte command)
    {
        CommandCode = command;
    }
}