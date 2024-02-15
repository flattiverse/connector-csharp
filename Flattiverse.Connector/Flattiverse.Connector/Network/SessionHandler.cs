namespace Flattiverse.Connector.Network;

class SessionHandler
{
    private readonly Session?[] sessions;
    
    private byte position;

    public readonly Connection Connection;

    public SessionHandler(Connection connection)
    {
        Connection = connection;
        
        sessions = new Session[256];
        
        position = 0;
    }

    async Task<Session> Get()
    {
        Session session;

        position++;

        while (Connection.Connected)
        {
            for (int count = 0; count < 16; count++, position++)
            {
                session = new Session(this, position);

                if (Interlocked.CompareExchange(ref sessions[position], session, null) is null)
                    return session;
            }
            
            await Task.Delay(10);
        }

        throw new GameException(0xFE, Connection.DisconnectReason);
    }
}