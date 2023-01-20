using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        public readonly int UniverseId;

        internal UnitEvent(JsonElement element)
        {
            if (!traverse(element, out UniverseId, "universe"))
                throw new InvalidDataException("universe doesn't exist or is incompatible.");
        }
    }
}
