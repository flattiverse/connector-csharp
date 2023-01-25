using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseEvent
    {
        internal static FlattiverseEvent parse(Connection connection, JsonElement element, int tick)
        {
            string kind;

            if (!Utils.Traverse(element, out kind, false, "kind"))
                throw new InvalidDataException("Event does not contain valid kind property.");

            switch (kind)
            {
                //Units
                case "newUnit":
                    return new UnitEventNew(element);
                case "updateUnit":
                    return new UnitEventUpdate(element);
                case "removeUnit":
                    return new UnitEventRemove(element);

                //Chat
                case "broadcast":
                    if (!Utils.Traverse(element, out JsonElement subBElement, JsonValueKind.Object, "message"))
                        throw new InvalidDataException("Event does not contain valid message property.");
                    return new BroadCastMessageEvent(subBElement);
                case "uni":
                    if (!Utils.Traverse(element, out JsonElement subUElement, JsonValueKind.Object, "message"))
                        throw new InvalidDataException("Event does not contain valid message property.");
                    return new UniChatMessageEvent(subUElement);

                //User
                case "newUser":
                    NewUserEvent nue = new NewUserEvent(element);
                    connection.UniverseGroup.addUser(nue.UserName);
                    return nue;
                case "removeUser":
                    RemoveUserEvent rue = new RemoveUserEvent(element);
                    connection.UniverseGroup.removeUser(rue.UserName);
                    return rue;

                //General
                case "universeInfo":
                    UniverseInfoEvent uie = new UniverseInfoEvent(element);
                    connection.UniverseGroup.addUniverse(uie.UniverseId);
                    return uie;
                case "tickCompleted":
                    return new TickCompleteEvent(tick);


                default:
                    throw new NotImplementedException();
            }
        }
    }
}
