namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// The base class for FlattiverseEvents.
    /// </summary>
    public class FlattiverseEvent
    {
        /// <summary>
        /// The timestamp when this event has been received or generated.
        /// </summary>
        public readonly DateTime Stamp;

        internal FlattiverseEvent()
        {
            Stamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public virtual EventKind Kind => throw new NotImplementedException("Somebody fucked up.");
    }
}
