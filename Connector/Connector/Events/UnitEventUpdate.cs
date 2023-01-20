using Flattiverse.Units;
using System.Text.Json;

namespace Flattiverse.Events
{
    public class UnitEventUpdate : UnitEvent
    {
        public readonly Unit Unit;

        internal UnitEventUpdate(JsonElement element) : base(element)
        {
            Unit = Unit.DeseializeJson(element);
        }
    }
}
