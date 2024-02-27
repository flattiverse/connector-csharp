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

        JoinedPlayer = 0x04,
        PartedPlayer = 0x05,
        
        JoinedControllable = 0x08,
        PartedControllable = 0x09,
        
        PlayerChat = 0x0C,
        TeamChat = 0x0D,
        GalaxyChat = 0x0E,
        
        GalaxyTick = 0xFF
    }
}
