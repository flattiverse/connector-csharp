using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseBroadCastMessage : FlattiverseMessage
    {

        public FlattiverseBroadCastMessage(Connection connection, JsonElement element) : base(connection, element)
        {
          
        }

    }
}
