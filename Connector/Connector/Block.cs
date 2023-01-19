using System.Text.Json;

namespace Flattiverse
{
    internal class Block : IDisposable
    {
        public string Id;

        private TaskCompletionSource taskCompletionSource;
        private BlockManager manager;
        public JsonDocument? Response;

        public Block(BlockManager manager, string Id) 
        {
            this.Id = Id;
            this.manager = manager;
            taskCompletionSource = new TaskCompletionSource();
        }

        public async Task Wait() 
        {
            await taskCompletionSource.Task;

            if (Response == null)
                throw new Exception("Disconnected from server.");
        }

        public void Answer(JsonDocument? response) 
        { 
            Response = response;
            taskCompletionSource.SetResult();
        }

        public void Dispose()
        {
            manager.Unblock(Id);
        }
    }
}
