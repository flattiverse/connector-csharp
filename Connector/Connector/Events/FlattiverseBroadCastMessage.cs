using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseBroadCastMessage : FlattiverseMessage
    {

        internal FlattiverseBroadCastMessage(Connection connection, JsonElement element) : base(connection, element)
        {
          
        }
    }
}
