namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Specifies the various event kinds for a better switch() experience.
    /// </summary>
    public enum EventKind : byte
    {
        UnitAdded = 0x00,
        UnitUpdated = 0x01,
        UnitVanished = 0x02,

        PlayerAdded = 0x03,
        PlayerRemoved = 0x04,
        
        GalaxyTick = 0x08
    }
}
