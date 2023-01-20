using Flattiverse.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
