using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class UserEvent : FlattiverseEvent
    {
        public readonly string UserName;

        internal UserEvent(JsonElement element)
        {
            if(!Utils.Traverse(element, out string name, false, "name"))
                throw new InvalidDataException("Event does not contain valid name property.");

            UserName = name;
        }

    }
}
