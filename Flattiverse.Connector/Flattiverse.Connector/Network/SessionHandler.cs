using System.Diagnostics;

namespace Flattiverse.Connector.Network;

internal class SessionHandler
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

    public async Task<Session> Get()
    {
        Session session;

        position++;
        byte tPosition;

        while (Connection.Connected)
        {
            for (int count = 0; count < 16; count++, position++)
            {
                tPosition = position;

                if (tPosition == 0)
                    tPosition = 1;
                
                // It may be wasteful but probability is really low that we have to retry here.
                session = new Session(this, tPosition);

                if (Interlocked.CompareExchange(ref sessions[tPosition], session, null) is null)
                    return session;
            }
            
            await Task.Delay(10);
        }

        throw new GameException(0xFE, Connection.DisconnectReason);
    }

    public void Answer(Packet packet)
    {
        Session? session = sessions[packet.Header.Session];

        if (session is not null)
        {
            session.Resolve(packet);
            sessions[packet.Header.Session] = null;
        }
#if DEBUG
        else
            Debug.Fail($"Received Packet with set session(={packet.Header.Session}) but there was no known session.");
#endif
    }

    public void TerminateConnections(string? reason)
    {
        for (int position = 0; position < sessions.Length; position++)
        {
            sessions[position]?.Reset(reason);
            sessions[position] = null;
        }
    }
}