using Flattiverse.Connector;
using Flattiverse.Connector.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        FlattiverseEvent @event;

        using (UniverseGroup universeGroup = new UniverseGroup("ws://127.0.0.1", "0123456789112345678921234567893123456789412345678951234567896123"))
        {
            universeGroup.SendTrashToTheServerButDontWait();

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
                }
            }
        }
    }
}