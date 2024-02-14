namespace Flattiverse.Connector.Network;

class Session
{
    public readonly SessionHandler Handler;
    public readonly byte Id;

    public Session(SessionHandler handler, byte id)
    {
        Handler = handler;
        Id = id;
    }

    /// <summary>
    /// Gives you a packet to use, setup with the corresponding session.
    /// </summary>
    public Packet Packet => new Packet(Id);
}