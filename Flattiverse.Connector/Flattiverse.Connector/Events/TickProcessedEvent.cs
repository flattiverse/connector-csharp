using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the completion of a tick in the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("tickProcessed")]
    public class TickProcessedEvent : FlattiverseEvent
    {
        public readonly TimeSpan ProcessingTime;

        internal TickProcessedEvent(UniverseGroup group, JsonElement element) : base()
        {
            Utils.Traverse(element, out double time, "processingTime");
            ProcessingTime = new TimeSpan((long)time * 10000);
        }

        public override EventKind Kind => EventKind.TickProcessed;
    }
}
