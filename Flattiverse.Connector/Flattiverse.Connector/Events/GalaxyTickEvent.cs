namespace Flattiverse.Connector.Events;

public class GalaxyTickEvent : FlattiverseEvent
{
    public override EventKind Kind => EventKind.GalaxyTick;

    internal GalaxyTickEvent()
    { }
    
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Just a little reminder that 50ms are over again.";
    }
}