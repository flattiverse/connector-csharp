using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class NewUserEvent : UserEvent
    {
        internal NewUserEvent(JsonElement element) : base(element)
        {
        }
    }
}
