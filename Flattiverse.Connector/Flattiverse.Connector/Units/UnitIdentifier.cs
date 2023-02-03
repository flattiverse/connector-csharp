namespace Flattiverse.Connector.Units
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class UnitIdentifier : Attribute
    {
        public readonly string Identifier;

        internal UnitIdentifier(string identifier)
        {
            Identifier = identifier;
        }
    }
}