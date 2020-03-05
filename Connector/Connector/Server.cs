using Flattiverse.Utils;
using Flattiverse.Events;
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
    public sealed class Server : IDisposable
    {
        internal Connection connection;

        internal Universe[] universes;

        private Player[] players;

        internal Controllable[] controllables;

        /// <summary>
        /// All the universes available.
        /// </summary>
        public readonly UniversalHolder<Universe> Universes;

        /// <summary>
        /// All the players registered to flattiverse.
        /// </summary>
        public readonly UniversalHolder<Player> Players;

        /// <summary>
        /// All controllables unter the control of the connected player.
        /// </summary>
        public readonly UniversalHolder<Controllable> Controllables;

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
        /// which change something within your event horizon or units which leave your event horizon.
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

            Universes = new UniversalHolder<Universe>(universes);

            players = new Player[65536];

            Players = new UniversalHolder<Player>(players);

            controllables = new Controllable[256];

            Controllables = new UniversalHolder<Controllable>(controllables);

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
            if (connection != null && online)
                throw new AlreadyConnectedException();
            
            if (connection != null)
                throw new InstanceWasBurnedException("This server instance was used once and has then been disconnected. Create a new instance.");

            if (username == null)
                throw new ArgumentNullException("username", "username can't be null.");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Invalid username.", "username");

            if (password == null)
                throw new ArgumentNullException("password", "password can't be null.");

            connection = new Connection();

            connection.Disconnected += disconnected;
            connection.Received += received;

            TaskCompletionSource<object> lWaiter = new TaskCompletionSource<object>();

            waiter = lWaiter;

            await connection.Connect(username, password).ConfigureAwait(false);

            await waiter.Task.ConfigureAwait(false);

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
            if (connection != null && online)
                throw new AlreadyConnectedException();

            if (connection != null)
                throw new InstanceWasBurnedException("This server instance was used once and has then been disconnected. Create a new instance.");

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

            TaskCompletionSource<object> lWaiter = new TaskCompletionSource<object>();

            waiter = lWaiter;

            await connection.Connect(username, hash).ConfigureAwait(false);

            await lWaiter.Task.ConfigureAwait(false);

            waiter = null;
            online = true;
        }

        private void received(List<Packet> packets)
        {
            foreach (Packet packet in packets)
            {
                if (packet.SessionUsed)
                {
                    connection?.ProcessSessionPacket(packet);
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
                        players[packet.BaseAddress]?.UpdatePing(packet);
                        break;
                    case 0x0E: // Player Assignment
                        {
                            Player requestPlayer = players[packet.BaseAddress];

                            // Debug.Assert(requestPlayer != null, "Player doesn't exist in local database.");

                            if (requestPlayer == null)
                                break;

                            if (packet.Read().Size == 0)
                            {
                                // TODO: Wenn wir selbst betroffen sind: Alle Controllables zerstören.

                                if (player != null && requestPlayer.Universe == player.Universe)
                                    EnqueueMetaEvent(new PlayerPartedEvent(requestPlayer));

                                requestPlayer.UpdatePlayerAssignment(packet);
                            }
                            else
                            {
                                requestPlayer.UpdatePlayerAssignment(packet);

                                if (player != null && requestPlayer.Universe == player.Universe)
                                    EnqueueMetaEvent(new PlayerJoinedEvent(requestPlayer));
                            }
                        }
                        break;
                    case 0x0F: // Login Completed or Denied
                        {
                            TaskCompletionSource<object> lWaiter = waiter;

                            waiter = null;

                            if (packet.Helper == 0x00)
                            {
                                player = players[packet.BaseAddress];

                                ThreadPool.QueueUserWorkItem(delegate { lWaiter?.SetResult(null); });
                            }
                            else
                                ThreadPool.QueueUserWorkItem(delegate { lWaiter?.SetException(new ClientRefusedException((RefuseReason)packet.Helper)); });
                        }
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
                            universes[packet.BaseAddress].teams[packet.SubAddress]?.updateFromPacket(packet);
                        break;
                    case 0x12: // Universe\Galaxy Metainfo Updated
                        if (packet.BaseAddress > universes.Length || universes[packet.BaseAddress] == null)
                            break;

                        if (packet.Read().Size == 0)
                            universes[packet.BaseAddress].galaxies[packet.SubAddress] = null;
                        else if (universes[packet.BaseAddress].galaxies[packet.SubAddress] == null)
                            universes[packet.BaseAddress].galaxies[packet.SubAddress] = new Galaxy(universes[packet.BaseAddress], packet);
                        else
                            universes[packet.BaseAddress].galaxies[packet.SubAddress]?.updateFromPacket(packet);
                        break;
                    case 0x13: // Universe\Systems Metainfo Updated
                        if (packet.BaseAddress > universes.Length || universes[packet.BaseAddress] == null)
                            break;

                        universes[packet.BaseAddress].updateSystems(packet);
                        break;
                    case 0x80: // Heartbeat received
                        EnqueueMetaEvent(HeartbeatEvent.Event);
                        break;
                    case 0x81: // Player Chat received
                        {
                            BinaryMemoryReader reader = packet.Read();
                            string message = reader.ReadString();
                            Player player = null;

                            foreach (Player tmpPlayer in Players)
                            {

                                if (tmpPlayer.id == packet.BaseAddress)
                                {
                                    player = tmpPlayer;
                                    break;
                                }
                            }

                            EnqueueMetaEvent(new PlayerChatEvent(message, player, this.player));
                            break;
                        }
                    case 0x82: // Universe Chat received
                        {
                            BinaryMemoryReader reader = packet.Read();
                            string message = reader.ReadString();
                            Player player = null;

                            foreach (Player tmpPlayer in Players)
                            {

                                if (tmpPlayer.id == packet.BaseAddress)
                                {
                                    player = tmpPlayer;
                                    break;
                                }
                            }

                            EnqueueMetaEvent(new UniverseChatEvent(message, player, player.Universe));
                            break;
                        }
                    case 0x83: // Team Chat received
                        {
                            BinaryMemoryReader reader = packet.Read();
                            string message = reader.ReadString();
                            Player player = null;

                            foreach (Player tmpPlayer in Players)
                            {

                                if (tmpPlayer.id == packet.BaseAddress)
                                {
                                    player = tmpPlayer;
                                    break;
                                }
                            }

                            EnqueueMetaEvent(new TeamChatEvent(message, player, player.Team));
                            break;
                        }
                    case 0x88: // New Unit
                        {
                            Units.Unit unit = Units.Unit.FromPacket(player.Universe, packet);

                            if (unit != null)
                                EnqueueMetaEvent(new NewUnitEvent(unit));
                        }
                        break;
                    case 0x89: // Updated Unit
                        {
                            Units.Unit unit = Units.Unit.FromPacket(player.Universe, packet);

                            if (unit != null)
                                EnqueueMetaEvent(new UpdatedUnitEvent(unit));
                        }
                        break;
                    case 0x90: // Deleted Unit
                        {
                            BinaryMemoryReader reader = packet.Read();

                            if (reader.Size == 0)
                                break;

                            EnqueueMetaEvent(new GoneUnitEvent(reader.ReadString()));
                        }
                        break;
                    case 0xC0: // Create Controllable
                        controllables[packet.SubAddress] = new Controllable(this, player.Universe, packet);
                        break;
                    case 0xC1: // Structural Update Controllable
                        {
                            BinaryMemoryReader reader = packet.Read();

                            controllables[packet.SubAddress]?.updateStructural(player.Universe, ref reader);
                        }
                        break;
                    case 0xC2: // Controllable deleted.
                        controllables[packet.SubAddress]?.deactivate();
                        controllables[packet.SubAddress] = null;
                        break;
                    case 0xC3: // Regular Update Controllable
                        {
                            BinaryMemoryReader reader = packet.Read();

                            controllables[packet.SubAddress]?.updateRegular(ref reader);
                        }
                        break;
                }
            }
        }

        internal void EnqueueMetaEvent(FlattiverseEvent @event)
        {
            FlattiverseEventHandler handler;

            switch (@event.Group)
            {
                case FlattiverseEventGroup.Meta:
                    handler = MetaEvent;
                    break;
                case FlattiverseEventGroup.Status:
                    handler = StatusEvent;
                    break;
                case FlattiverseEventGroup.Scan:
                    handler = ScanEvent;
                    break;
                case FlattiverseEventGroup.Chat:
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
                {
                    TaskCompletionSource<object> tWaiter = waiter;

                    if (tWaiter != null)
                    {
                        waiter = null;

                        ThreadPool.QueueUserWorkItem(delegate { tWaiter.SetResult(null); });
                    }
                }

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

        internal async Task<Account> QueryAccount(uint id)
        {
            using (Session session = connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x40;

                // TODO: Autoselection.
                packet.ID = id;

                connection.Send(packet);
                connection.Flush();

                packet = await session.Wait().ConfigureAwait(false);

                BinaryMemoryReader reader = packet.Read();

                if (reader.Size == 0)
                    return null;

                return new Account(this, ref reader);
            }
        }

        /// <summary>
        /// Querys the account with the given name.
        /// </summary>
        /// <param name="name">The name you are looking for.</param>
        /// <returns>The account.</returns>
        public async Task<Account> QueryAccount(string name)
        {
            if (!Account.CheckName(name))
                throw new IllegalNameException();

            using (Session session = connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x40;

                ManagedBinaryMemoryWriter writer = packet.Write();
                writer.Write(name);

                connection.Send(packet);
                connection.Flush();

                packet = await session.Wait().ConfigureAwait(false);

                BinaryMemoryReader reader = packet.Read();

                if (reader.Size == 0)
                    throw new AccountDoesntExistException(name);

                return new Account(this, ref reader);
            }
        }

        /// <summary>
        /// Rephrases the given XML.
        /// </summary>
        /// <param name="xml">The XML to validate.</param>
        /// <returns>Returns reformulated XML by one of the universe instances.</returns>
        public async Task<string> CheckUnitXml(string xml)
        {
            if (xml == null || xml.Length < 5)
                throw new AmbiguousXmlDataException();

            using (Session session = connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x63;

                packet.Write().Write(xml);

                connection.Send(packet);
                connection.Flush();

                return (await session.Wait().ConfigureAwait(false)).Read().ReadString();
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

            TaskCompletionSource<object> lWaiter = null;

            lock (syncEvents)
                if (pollingEvents.Count > 0)
                {
                    foundEvents = pollingEvents;
                    pollingEvents = new Queue<FlattiverseEvent>();
                    return foundEvents;
                }
                else
                {
                    lWaiter = new TaskCompletionSource<object>();

                    waiter = lWaiter;
                }

            if (lWaiter != null)
                await lWaiter.Task.ConfigureAwait(false);

            foundEvents = pollingEvents;
            pollingEvents = new Queue<FlattiverseEvent>();
            return foundEvents;
        }

        /// <summary>
        /// Queries all accounts registered on flattiverse matching the given pattern. However, this will only return the first 256 matches.
        /// </summary>
        /// <param name="name">The name of the account to query. This supports wildcards like the % or ? sign. Use null or "%" here, if you wanna search for all accounts.</param>
        /// <param name="onlyConfirmed">Use true here, if you only wanna get accounts which have been confirmed (opped in). false will also return admin, banned and accounts in optin-status.</param>
        /// <returns>An async foreachable enumerator.</returns>
        public IEnumerable<Account> QueryAccounts(string name, bool onlyConfirmed)
        {
            List<uint> ids = new List<uint>();

            using (AutoResetEvent are = new AutoResetEvent(false))
            {
                using (Session session = connection.NewSession())
                {
                    Packet packet = session.Request;

                    packet.Command = 0x41;

                    ManagedBinaryMemoryWriter writer = packet.Write();

                    writer.Write(name);
                    writer.Write(onlyConfirmed);

                    connection.Send(packet);
                    connection.Flush();

                    ThreadPool.QueueUserWorkItem(async delegate {
                        // I hate you for forcing me to do this, microsoft. Really.
                        packet = await session.Wait().ConfigureAwait(false);
                        are.Set();
                    });

                    are.WaitOne();

                    BinaryMemoryReader reader = packet.Read();

                    while (reader.Size > 0)
                        ids.Add(reader.ReadUInt32());
                }

                Account account = null;

                foreach (uint id in ids)
                {
                    ThreadPool.QueueUserWorkItem(async delegate {
                        // I hate you for forcing me to do this, microsoft. Really.
                        account = await QueryAccount(id).ConfigureAwait(false);
                        are.Set();
                    });

                    are.WaitOne();

                    if (account != null)
                        yield return account;
                }
            }
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
            connection?.Close();
        }
    }
}
