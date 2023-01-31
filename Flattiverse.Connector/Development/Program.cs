using Flattiverse.Connector;
using Flattiverse.Connector.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        FlattiverseEvent @event;

        string apiUser = "0123456789112345678921234567893123456789412345678951234567896123";
        string apiAdmn = "9876342587963245879623458976234589762345ACBACBACABCEDFEDFDEFEDFE";
        string apiSpec = "0000000000000000000000000000000000000000000000000000000000000000";

        //string usedKey = apiUser;
        //string usedKey = apiAdmn;
        string usedKey = apiSpec;

        using (UniverseGroup universeGroup = new UniverseGroup("ws://127.0.0.1", usedKey))
        {
            Console.WriteLine($"uG.Name = {universeGroup.Name}.");

            while (true)
            {
                @event = await universeGroup.NextEvent();

                switch (@event)
                {
                    case RawEvent rawEvent:
                        Console.WriteLine($"RAW Event: {rawEvent.RawData}");
                        break;
                    case FailureEvent failureEvent:
                        Console.WriteLine($"Failure Event: {failureEvent.Message}");
                        break;
                    case UniverseGroupInfoEvent universeGroupInfoEvent:
                        Console.WriteLine($"UniverseGroupInfo Event: {universeGroupInfoEvent.Name}");
                        break;
                    case FullUpdatePlayerEvent fullUpdatePlayerEvent:
                        Console.WriteLine($"FullUpdatePlayerEvent Event: #{fullUpdatePlayerEvent.ID}");
                        break;
                    case PartialUpdatePlayerEvent partialUpdatePlayerEvent:
                        Console.WriteLine($"PartialUpdatePlayerEvent Event: #{partialUpdatePlayerEvent.ID}");
                        break;
                    case RemovedPlayerEvent removedPlayerEvent:
                        Console.WriteLine($"RemovedPlayerEvent Event: #{removedPlayerEvent.ID}");
                        break;
                }
            }
        }
    }
}