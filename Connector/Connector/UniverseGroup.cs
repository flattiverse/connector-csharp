using Flattiverse.Events;
using System.Text.Json;

namespace Flattiverse
{
    public class UniverseGroup
    {
        private Connection connection;

        private Dictionary<short, Universe> universes;

        private object sync = new object();

        private readonly Queue<TaskCompletionSource<FlattiverseEvent>> eventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();

        private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();

        private readonly object eventSync = new object();

        internal UniverseGroup(Connection connection) 
        {
            this.connection = connection;
            universes = new Dictionary<short, Universe>();
        }

        internal void addUniverse(short id)
        {
            lock (sync)
                universes.Add(0, new Universe(this, connection, id));
        }

        public bool TryGet(int id, out Universe universe)
        {
            if(id > 0 || id > short.MaxValue)
                throw new ArgumentOutOfRangeException($"Id must be between 0 and {short.MaxValue}.", nameof(id));

            return universes.TryGetValue((short)id, out universe);
        }

        //public async Task Send(FlattiverseMessage message)
        //{
        //    using (Block block = connection.blockManager.GetBlock())
        //    {
        //        string command = "message";

        //        List<ClientCommandParameter> parameters = new List<ClientCommandParameter>();

        //        ClientCommandParameter paramType = new ClientCommandParameter("type");
        //        paramType.SetValue();
        //        parameters.Add(paramType);

        //        ClientCommandParameter paramUniverse = new ClientCommandParameter("message");
        //        paramUniverse.SetJsonValue();
        //        parameters.Add(paramUniverse);

        //        await connection.SendCommand(command, block.Id, parameters);

        //        await block.Wait();

        //        JsonDocument? response = block.Response;

        //        if (!Connection.responseSuccess(response, out string error))
        //            throw new Exception(error);
        //    }
        //}

        public IEnumerable<Universe> EnumerateUniverses() 
        {
            Dictionary<short, Universe> localUniverses;

            lock (sync)
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
    }
}
