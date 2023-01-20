using System.Text.Json;

namespace Flattiverse.Events
{
    public class UnitEventUpdate : UnitEvent
    {
        internal UnitEventUpdate(JsonElement element) : base(element)
        {
        }
    }
}
