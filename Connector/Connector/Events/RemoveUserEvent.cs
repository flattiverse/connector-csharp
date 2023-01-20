using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class RemoveUserEvent : UserEvent
    {
        internal RemoveUserEvent(Connection connection, JsonElement element) : base(connection, element)
        {
            connection.UniverseGroup.addUser(UserName);
        }
    }
}
