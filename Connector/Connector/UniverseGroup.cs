using Flattiverse.Events;
using System.Text.Json;

namespace Flattiverse
{
    public class UniverseGroup
    {
        private Connection connection;

        private Dictionary<short, Universe> universes;

        private object universeSync = new object();

        private readonly Queue<TaskCompletionSource<FlattiverseEvent>> eventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();

        private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();

        private readonly object eventSync = new object();

        internal readonly TaskCompletionSource initialTcs;

        private readonly Dictionary<string, User> users;

        private readonly object userSync = new object();

        internal UniverseGroup(Connection connection) 
        {
            this.connection = connection;
            universes = new Dictionary<short, Universe>();
            users = new Dictionary<string, User>();
            initialTcs = new TaskCompletionSource();
        }

        internal void parseInitialization(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Array)
                throw new Exception("Payload is not of type array");

            foreach (JsonElement subElement in element.EnumerateArray())
            {
                if(!Utils.Traverse(subElement, out string kind, false, "kind"))
                    throw new Exception("Payload array element kind property not valid.");

                switch (kind)
                {
                    case "universeUpdate":
                        if (!Utils.Traverse(subElement, out int universeId, "universe"))
                            throw new Exception("Payload array element universe property not valid.");

                        addUniverse((short)universeId);
                        break;
                    case "userUpdate":
                        if (!Utils.Traverse(subElement, out string name, false, "name"))
                            throw new Exception("Payload array element name property not valid.");

                        addUser(name);
                        break;

                    default:
                        throw new Exception("Payload array element kind property not valid.");
                }

            }

            ThreadPool.QueueUserWorkItem(delegate { initialTcs.SetResult(); });
        }

        private void addUser(string name)
        {
            lock (userSync)
                users.Add(name, new User(name));
        }

        internal void addUniverse(short id)
        {
            lock (universeSync)
                universes.Add(id, new Universe(this, connection, id));
        }

        public bool TryGetUser(string name, out User user)
        {
            lock (userSync)
                return users.TryGetValue(name, out user);
        }

        public IEnumerable<User> EnumerateUsers()
        {
            Dictionary<string, User> localUsers;

            lock (userSync)
                localUsers = new Dictionary<string, User>(users);

            foreach (KeyValuePair<string, User> userKvP in localUsers)
                yield return userKvP.Value;
        }

        public bool TryGetUniverse(int id, out Universe universe)
        {
            if(id > 0 || id > short.MaxValue)
                throw new ArgumentOutOfRangeException($"Id must be between 0 and {short.MaxValue}.", nameof(id));

            lock (universeSync)
                return universes.TryGetValue((short)id, out universe);
        }

        public IEnumerable<Universe> EnumerateUniverses() 
        {
            Dictionary<short, Universe> localUniverses;

            lock (universeSync)
                localUniverses= new Dictionary<short, Universe>(universes);

            foreach (KeyValuePair<short, Universe> universeKvP in localUniverses)
                yield return universeKvP.Value;
        }

        public async Task<FlattiverseEvent> NextEvent()
        {
            FlattiverseEvent? fvEvent;
            TaskCompletionSource<FlattiverseEvent> tcs;

            lock (eventSync)
            {
                if(pendingEvents.TryDequeue(out fvEvent))
                    return fvEvent;

                tcs = new TaskCompletionSource<FlattiverseEvent>();
                eventWaiters.Enqueue(tcs);
            }

            return await tcs.Task;
        }

        internal void pushEvent(FlattiverseEvent fvEvent)
        {
            TaskCompletionSource<FlattiverseEvent> tcs;

            lock (eventSync) 
            { 
                if(eventWaiters.TryDequeue(out tcs))
                {
                    ThreadPool.QueueUserWorkItem(delegate { tcs.SetResult(fvEvent); });
                    return;
                }

                pendingEvents.Enqueue(fvEvent);
            }
        }

        public async Task SendBroadCastMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("Message can not be null or empty", nameof(message));

            if (message.Length > 2048)
                throw new ArgumentException("Message cant be longer than 2048 characters", nameof(message));

            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "message";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter kindParam = new ClientCommandParameter("kind");
                kindParam.SetValue("broadcastMessage");

                string data;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, Connection.jsonOptions))
                    {
                        writer.WriteStartObject();

                        writer.WriteString("text", message);

                        writer.WriteEndObject();
                    }

                    ms.Position = 0;
                    data = new StreamReader(ms).ReadToEnd();
                }

                ClientCommandParameter messageParam = new ClientCommandParameter("message");

                messageParam.SetJsonValue(data);

                parameters.Add(messageParam);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument? response = block.Response;

                if (!Connection.responseSuccess(response!, out string? error))
                    throw new Exception(error);
            }
        }
    }
}
