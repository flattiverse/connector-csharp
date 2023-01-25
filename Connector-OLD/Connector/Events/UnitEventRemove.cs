using System.Text.Json;

namespace Flattiverse.Events
{
    public class UnitEventRemove : UnitEvent
    {
        public readonly string Name;

        internal UnitEventRemove(JsonElement element) : base (element)
        {
            if (!Utils.Traverse(element, out Name, false, "name"))
                throw new InvalidDataException("Event does not contain valid name property.");
        }
    }
}
