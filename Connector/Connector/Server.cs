using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flattiverse
{
    /// <summary>
    /// This represents a server you connected to.
    /// </summary>
    public class Server : IDisposable
    {
        internal Connection connection;

        internal Universe?[] universes;

        private Player?[] players;

        /// <summary>
        /// All the universes available.
        /// </summary>
        public readonly UniversalHolder<Universe?> Universes;

        /// <summary>
        /// All the players registered to flattiverse.
        /// </summary>
        public readonly UniversalHolder<Player?> Players;

        private TaskCompletionSource<object> waiter;

        /// <summary>
        /// Specifies the function signature of an flattiverse event event handler.
        /// </summary>
        /// <param name="event">The event which occurred.</param>
        public delegate void FlattiverseEventHandler(FlattiverseEvent @event);

        /// <summary>
        /// This eventhandler will be called whenever a meta event occurs.
        /// 
        /// Meta events are usually no direct game events. Meta events are used to inform you about
        /// players joinning our universe or about newly registered ships of your teammates, etc.
        /// Meta events also gives you the heartbeat of the universe (when internal ticks have been
        /// finished.)
        /// </summary>
        public event FlattiverseEventHandler MetaEvent;

        /// <summary>
        /// This eventhandler will be called whenever a status event occurs.
        /// 
        /// Status updates are updates which inform you about a status change of one of your units.
        /// This includes hull, shield, energy or configuration changes.
        /// </summary>
        public event FlattiverseEventHandler StatusEvent;

        /// <summary>
        /// This eventhandler will be called whenever a scan event occurs.
        /// 
        /// Scan events are updates to scanned units: Units appreaing in your event horizon, units
        /// which change something within your eventhorizon or units which leave your eventhorizon.
        /// </summary>
        public event FlattiverseEventHandler ScanEvent;

        /// <summary>
        /// This eventhandler will be called whenever a chat event occurs.
        /// 
        /// Chat events contain chat messages from other players sent to you or the universe group
        /// you are in or the team you are in. This also includes map pings or binary messages.
        /// </summary>
        public event FlattiverseEventHandler ChatEvent;

        private object syncEvents;
        private bool eventInExecution;
        private Queue<(FlattiverseEventHandler, FlattiverseEvent)> events;
        private Queue<FlattiverseEvent> pollingEvents;

        private bool online;

        private Player player;

        /// <summary>
        /// Creates a new instance of a server connection without connecting (use login for this).
        /// </summary>
        public Server()
        {
            universes = new Universe[16];

            Universes = new UniversalHolder<Universe?>(universes);

            players = new Player[65536];

            Players = new UniversalHolder<Player?>(players);

            syncEvents = new object();
            events = new Queue<(FlattiverseEventHandler, FlattiverseEvent)>();
            pollingEvents = new Queue<FlattiverseEvent>();
        }

        /// <summary>
        /// Login to the flattiverse server.
        /// </summary>
        /// <param name="username">Your username.</param>
        /// <param name="password">Your password.</param>
        /// <returns>Nothing. (Just a task fpr async/await-pattern.)</returns>
        public async Task Login(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException("username", "username can't be null.");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Invalid username.", "username");

            if (password == null)
                throw new ArgumentNullException("password", "password can't be null.");

            connection = new Connection();

            connection.Disconnected += disconnected;
            connection.Received += received;

            waiter = new TaskCompletionSource<object>();

            await connection.Connect(username, password);

            await waiter.Task;

            waiter = null;
            online = true;
        }

        /// <summary>
        /// Login to the flattiverse server.
        /// </summary>
        /// <param name="username">Your username.</param>
        /// <param name="hash">Your hashed password. (Use Crypto.HashPassword.)</param>
        /// <returns>Nothing. (Just a task fpr async/await-pattern.)</returns>
        public async Task Login(string username, byte[] hash)
        {
            if (username == null)
                throw new ArgumentNullException("username", "username can't be null.");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Invalid username.", "username");

            if (hash == null)
                throw new ArgumentNullException("hash", "hash can't be null.");

            if (hash.Length != 16)
                throw new ArgumentNullException("hash", "hash must consist of 16 bytes.");

            connection = new Connection();

            connection.Disconnected += disconnected;
            connection.Received += received;

            waiter = new TaskCompletionSource<object>();

            await connection.Connect(username, hash);

            await waiter.Task;

            waiter = null;
        }

        private void received(List<Packet> packets)
        {
            foreach (Packet packet in packets)
            {
                if (packet.SessionUsed)
                {
                    connection.ProcessSessionPacket(packet);
                    continue;
                }

                switch (packet.Command)
                {
                    case 0x0A: // Player Removed.
                        players[packet.BaseAddress] = null;
                        break;
                    case 0x0B: // Player Created.
                        players[packet.BaseAddress] = new Player(this, packet);
                        break;
                    case 0x0C: // Player Defragmented.
                        {
                            int oldAddress = packet.Read().ReadUInt16();

                            players[packet.BaseAddress] = players[oldAddress];
                            players[oldAddress] = null;
                        }
                        break;
                    case 0x0D: // Player Ping Update
                        players[packet.BaseAddress].UpdatePing(packet);
                        break;
                    case 0x0E: // Player Assignment
                        {
                            Player requestPlayer = players[packet.BaseAddress];

                            Debug.Assert(requestPlayer != null, "Player doesn't exist in local database.");

                            if (requestPlayer == null)
                                break;

                            if (packet.Read().Size == 0)
                            {
                                if (requestPlayer.Universe == player.Universe)
                                    EnqueueMetaEvent(new PlayerPartedEvent(requestPlayer));

                                requestPlayer.UpdatePlayerAssignment(packet);
                            }
                            else
                            {
                                requestPlayer.UpdatePlayerAssignment(packet);

                                if (requestPlayer.Universe == player.Universe)
                                    EnqueueMetaEvent(new PlayerJoinedEvent(requestPlayer));
                            }
                        }
                        break;
                    case 0x0F: // Login Completed or Denied
                        if (packet.Helper == 0x00)
                        {
                            player = players[packet.BaseAddress];

                            ThreadPool.QueueUserWorkItem(delegate { waiter?.SetResult(null); });
                        }
                        else
                            ThreadPool.QueueUserWorkItem(delegate { waiter?.SetException(new ClientRefusedException((RefuseReason)packet.Helper)); });
                        break;
                    case 0x10: // Universe Metainfo Updated
                        if (packet.BaseAddress > universes.Length)
                        {
                            Universe[] nUniverses = new Universe[packet.BaseAddress];

                            Array.Copy(universes, 0, nUniverses, 0, universes.Length);

                            universes = nUniverses;
                            Universes.updateDatabasis(universes);
                        }

                        if (packet.Read().Size == 0)
                            universes[packet.BaseAddress] = null;
                        else if (universes[packet.BaseAddress] == null)
                            universes[packet.BaseAddress] = new Universe(this, packet);
                        else
                            universes[packet.BaseAddress].updateFromPacket(packet);
                        break;
                    case 0x11: // Universe\Team Metainfo Updated
                        if (packet.BaseAddress > universes.Length || universes[packet.BaseAddress] == null)
                            break;

                        if (packet.Read().Size == 0)
                            universes[packet.BaseAddress].teams[packet.SubAddress] = null;
                        else if (universes[packet.BaseAddress].teams[packet.SubAddress] == null)
                            universes[packet.BaseAddress].teams[packet.SubAddress] = new Team(universes[packet.BaseAddress], packet);
                        else
                            universes[packet.BaseAddress].teams[packet.SubAddress].updateFromPacket(packet);
                        break;
                    case 0x12: // Universe\Galaxy Metainfo Updated
                        if (packet.BaseAddress > universes.Length || universes[packet.BaseAddress] == null)
                            break;

                        if (packet.Read().Size == 0)
                            universes[packet.BaseAddress].galaxies[packet.SubAddress] = null;
                        else if (universes[packet.BaseAddress].galaxies[packet.SubAddress] == null)
                            universes[packet.BaseAddress].galaxies[packet.SubAddress] = new Galaxy(universes[packet.BaseAddress], packet);
                        else
                            universes[packet.BaseAddress].galaxies[packet.SubAddress].updateFromPacket(packet);
                        break;
                    case 0x13: // Universe\Systems Metainfo Updated
                        if (packet.BaseAddress > universes.Length || universes[packet.BaseAddress] == null)
                            break;

                        universes[packet.BaseAddress].updateSystems(packet);
                        break;
                }
            }
        }

        internal void EnqueueMetaEvent(FlattiverseEvent @event)
        {
            FlattiverseEventHandler handler;

            switch (@event.Kind)
            {
                case FlattiverseEventKind.Meta:
                    handler = MetaEvent;
                    break;
                case FlattiverseEventKind.Status:
                    handler = StatusEvent;
                    break;
                case FlattiverseEventKind.Scan:
                    handler = ScanEvent;
                    break;
                case FlattiverseEventKind.Chat:
                    handler = ChatEvent;
                    break;
                default:
                    handler = null;
                    break;
            }

            if (handler == null)
            {
                pollingEvents.Enqueue(@event);

                lock (syncEvents)
                    if (waiter != null)
                        ThreadPool.QueueUserWorkItem(delegate { waiter.SetResult(null); });

                return;
            }

            lock (syncEvents)
                if (eventInExecution)
                    events.Enqueue((handler, @event));
                else
                {
                    eventInExecution = true;
                    events.Enqueue((handler, @event));
                    ThreadPool.QueueUserWorkItem(delegate { executeEvents(); });
                }
        }

        private void executeEvents()
        {
            FlattiverseEventHandler handler;
            FlattiverseEvent @event;

            while (true)
            {
                lock (syncEvents)
                {
                    if (events.Count == 0)
                    {
                        eventInExecution = false;
                        return;
                    }

                    (handler, @event) = events.Dequeue();
                }

                try
                {
                    handler(@event);
                }
                catch { }
            }
        }

        /// <summary>
        /// Gathers events. It waits blockingly for new events if there are no events.
        /// </summary>
        /// <remarks>Don't execute this in parallel with itself or PollEvents().</remarks>
        /// <returns>A queue of events.</returns>
        public async Task<Queue<FlattiverseEvent>> GatherEvents()
        {
            if (!online)
                return new Queue<FlattiverseEvent>();

            Queue<FlattiverseEvent> foundEvents;

            lock (syncEvents)
                if (pollingEvents.Count > 0)
                {
                    foundEvents = pollingEvents;
                    pollingEvents = new Queue<FlattiverseEvent>();
                    return foundEvents;
                }
                else
                    waiter = new TaskCompletionSource<object>();

            await waiter.Task;

            waiter = null;

            foundEvents = pollingEvents;
            pollingEvents = new Queue<FlattiverseEvent>();
            return foundEvents;
        }

        /// <summary>
        /// Polls for events. This method will return immediately, even if there are no events.
        /// </summary>
        /// <remarks>Don't execute this in parallel with itself or GatherEvents().</remarks>
        /// <returns>A queue of events.</returns>
        public Queue<FlattiverseEvent> PollEvents()
        {
            Queue<FlattiverseEvent> foundEvents = pollingEvents;
            pollingEvents = new Queue<FlattiverseEvent>();
            return foundEvents;
        }

        /// <summary>
        /// Your player instance.
        /// </summary>
        public Player Player => player;

        /// <summary>
        /// Specifies if this server is still connected with us.
        /// </summary>
        public bool Online => online;

        private void disconnected()
        {
            online = false;
        }

        /// <summary>
        /// Releases all ressources of this instance.
        /// </summary>
        public void Dispose()
        {
            connection.Close();
        }
    }
}
