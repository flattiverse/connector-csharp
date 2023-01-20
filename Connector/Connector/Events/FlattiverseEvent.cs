using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseEvent
    {
        internal static FlattiverseEvent parse(Connection connection, JsonElement element)
        {
            string kind;

            if (!Utils.Traverse(element, out kind, false, "kind"))
                throw new InvalidDataException("Event does not contain valid kind property.");

            switch (kind)
            {
                case "newUnit":
                    return new UnitEventNew(element);
                case "updateUnit":
                    return new UnitEventUpdate(element);
                case "removeUnit":
                    return new UnitEventRemove(element);
                case "broadcastMessage":
                    return new FlattiverseBroadCastMessage(connection, element);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
