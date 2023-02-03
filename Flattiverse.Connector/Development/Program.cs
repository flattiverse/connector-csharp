using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using System.Globalization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        FlattiverseEvent @event;

        //Matz
        string apiUser = "0123456789112345678921234567893123456789412345678951234567896123";
        string apiAdmn = "9876342587963245879623458976234589762345ACBACBACABCEDFEDFDEFEDFE";

        //Togo
        string apiUser2 = "BABABABABABABABABBBBBBBBBBBBBBBBBBBBB319480573420958723458796345";
        string apiAdmn2 = "BABABABABABABABABCCCCCCCCCCCCCCCCCCCC319480573420958723458796345";

        string apiSpec = "0000000000000000000000000000000000000000000000000000000000000000";
        string apiShit = "000000000000000000000000000000000000000000000000000000000000D3AD";

        // TOG: Etwas aufräumen bitte, so das nur noch eine Connection hergestellt wird und wir eine Unit anlegen können.

        using (UniverseGroup universeGroup = new UniverseGroup("ws://127.0.0.1", apiAdmn))
        using (UniverseGroup universeGroup2 = new UniverseGroup("ws://127.0.0.1", apiAdmn2))
        {
            Console.WriteLine($"uG.Name = {universeGroup.Name}.");

            await universeGroup.GetUniverse("Training ground")!.SetUnit("TOG macht hier eine valide Unit rein.");

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
                    case RemovedUnitEvent removedUnitEvent:
                        Console.WriteLine($"RemovedUnitEvent Event: {removedUnitEvent.Name}");
                        break;
                }
            }
        }
    }
}