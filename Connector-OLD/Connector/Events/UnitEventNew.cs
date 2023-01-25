using Flattiverse.Units;
using System;
using System.Text.Json;

namespace Flattiverse.Events
{
    public class UnitEventNew : UnitEvent
    {
        public readonly Unit Unit;

        internal UnitEventNew(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out JsonElement unitElement, JsonValueKind.Object, "unit"))
                throw new InvalidDataException("Unit property invalid.");

            Unit = Unit.DeseializeJson(unitElement);
        }
    }
}
