namespace Flattiverse.Connector.Network;

class Session
{
    public readonly SessionHandler Handler;
    public readonly byte Id;

    private TaskCompletionSource<Packet> tcs;

    public Session(SessionHandler handler, byte id)
    {
        Handler = handler;
        Id = id;
    }

    /// <summary>
    /// Gives you a packet to use, setup with the corresponding session.
    /// </summary>
    public Packet Packet => new Packet(Id);

    public async Task<Packet> SendWait(Packet packet)
    {
        if (!Handler.Connection.Connected)
            throw new GameException(0xFE, Handler.Connection.DisconnectReason);
        
        packet.Header.Session = Id;

        tcs = new TaskCompletionSource<Packet>();
        
        await Handler.Connection.Send(packet);

        packet = await tcs.Task;

        if (packet.Header.Command == 0xFF) // Exception received.
            if (packet.Header.Size == 0)
                throw new GameException(packet.Header.Param0);
            else
                throw new GameException(packet.Header.Param0, System.Text.Encoding.UTF8.GetString(packet.Payload, packet.Offset, packet.Payload.Length));

        return packet;
    }

    public void Resolve(Packet packet)
    {
        tcs.SetResult(packet);
    }

    public void Reset(string? reason)
    {
        tcs.TrySetException(new GameException(0xFE, reason));
    }
}