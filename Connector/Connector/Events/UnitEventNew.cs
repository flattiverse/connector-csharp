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
            Unit = Unit.DeseializeJson(element);
        }
    }
}
