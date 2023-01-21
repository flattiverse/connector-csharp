using Flattiverse.Events;
using System;
using System.Text.Json;

namespace Flattiverse
{
    public class UniverseGroup : IDisposable
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

        public delegate void ConnectionHandler(Exception? ex);

        public event ConnectionHandler? ConnectionClosed;


        public UniverseGroup(Uri host)
        {
            connection = new Connection(host, this);

            universes = new Dictionary<short, Universe>();
            users = new Dictionary<string, User>();
            initialTcs = new TaskCompletionSource();
        }

        public UniverseGroup(string host, string apiKey, bool https)
        {
            connection = new Connection(host, apiKey, https, this);

            universes = new Dictionary<short, Universe>();
            users = new Dictionary<string, User>();
            initialTcs = new TaskCompletionSource();
        }

        public async Task ConnectAsync()
        {
            await connection.ConnectAsync();
        }

        internal void ConnectionClose(Exception? exception)
        {
            if (ConnectionClosed != null)
                ConnectionClosed(exception);
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

        internal void addUser(string name)
        {
            lock (userSync)
                users.TryAdd(name, new User(name));
        }

        internal void removeUser(string name)
        {
            lock (userSync)
                users.Remove(name);
        }

        internal void addUniverse(short id)
        {
            lock (universeSync)
                universes.Add(id, new Universe(this, connection, id));
        }

        public bool TryGetUser(string name, out User? user)
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

        public bool TryGetUniverse(int id, out Universe? universe)
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
            TaskCompletionSource<FlattiverseEvent>? tcs;

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
                kindParam.SetValue("broadcast");
                parameters.Add(kindParam);

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

        public async Task SendUniMessage(string message, User user)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("Message can not be null or empty.", nameof(message));

            if (message.Length > 2048)
                throw new ArgumentException("Message cant be longer than 2048 characters", nameof(message));

            if (user == null)
                throw new ArgumentNullException("user can not be null.", nameof(user));

            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "message";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter kindParam = new ClientCommandParameter("kind");
                kindParam.SetValue("uni");
                parameters.Add(kindParam);

                string data;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, Connection.jsonOptions))
                    {
                        writer.WriteStartObject();

                        writer.WriteString("receiver", user.Name);
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

        public async Task CreateUniverse(string name, double xBounds, double yBounds)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name can not be null or empty.", nameof(name));

            if (xBounds < 100)
                throw new ArgumentException("xBounds must be at least 100", nameof(xBounds));

            if (yBounds < 100)
                throw new ArgumentException("yBounds must be at least 100", nameof(yBounds));

            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "createUniverse";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter paramName = new ClientCommandParameter("name");
                paramName.SetValue(name);
                parameters.Add(paramName);

                ClientCommandParameter paramXBounds = new ClientCommandParameter("xBounds");
                paramXBounds.SetValue(xBounds);
                parameters.Add(paramXBounds);

                ClientCommandParameter paramYBounds = new ClientCommandParameter("yBounds");
                paramYBounds.SetValue(yBounds);
                parameters.Add(paramYBounds);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument response = block.Response!;

                if (!Connection.responseSuccess(response, out string? error))
                    throw new Exception(error);
            }
        }

        public async Task SetUnitThruster(string unitName, double value)
        {
            if (string.IsNullOrEmpty(unitName))
                throw new ArgumentNullException("UnitName can not be null or empty.", nameof(unitName));

            if (unitName.Length > 32)
                throw new ArgumentException("UnitName can not be longer than 32.", nameof(unitName));

            if (value < 0 || value > 0.025)
                throw new ArgumentOutOfRangeException(nameof(value), "Thruster value must be between 0 and 0.025");

            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "thruster";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter paramName = new ClientCommandParameter("name");
                paramName.SetValue(unitName);
                parameters.Add(paramName);

                ClientCommandParameter paramValue = new ClientCommandParameter("value");
                paramValue.SetValue(value);
                parameters.Add(paramValue);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument response = block.Response!;

                if (!Connection.responseSuccess(response, out string? error))
                    throw new Exception(error);
            }
        }

        public async Task SetUnitNuzzle(string unitName, double value)
        {
            if (string.IsNullOrEmpty(unitName))
                throw new ArgumentNullException("UnitName can not be null or empty.", nameof(unitName));

            if (unitName.Length > 32)
                throw new ArgumentException("UnitName can not be longer than 32.", nameof(unitName));

            if (value < -2 || value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "Nuzzle value must be between -2 and 2");

            using (Block block = connection.blockManager.GetBlock())
            {
                string command = "controlNuzzle";

                List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

                ClientCommandParameter paramName = new ClientCommandParameter("name");
                paramName.SetValue(unitName);
                parameters.Add(paramName);

                ClientCommandParameter paramValue = new ClientCommandParameter("value");
                paramValue.SetValue(value);
                parameters.Add(paramValue);

                await connection.SendCommand(command, block.Id, parameters);

                await block.Wait();

                JsonDocument response = block.Response!;

                if (!Connection.responseSuccess(response, out string? error))
                    throw new Exception(error);
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
