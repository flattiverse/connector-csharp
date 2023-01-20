using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class UnitEventRemove : UnitEvent
    {
        public readonly string Name;

        internal UnitEventRemove(JsonElement element) : base (element)
        {
            if (!traverse(element, out Name, "name"))
                throw new InvalidDataException("name doesn't exist or isn't a string.");
        }
    }
}
