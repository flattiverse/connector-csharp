namespace Flattiverse
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class Command : Attribute
    {
        public readonly string Name;

        public Command(string name)
        {
            Name = name;
        }
    }
}
