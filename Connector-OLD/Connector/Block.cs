using System.Buffers;
using System.Text.Json;

namespace Flattiverse
{
    internal class Block : IDisposable
    {
        public string Id;

        private byte[]? recv;
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

        public void Answer(byte[]? recv, JsonDocument? response)
        {
            this.recv = recv;
            Response = response;
            ThreadPool.QueueUserWorkItem(delegate { taskCompletionSource.SetResult(); });
        }

        public void Dispose()
        {
            if(recv != null)
                ArrayPool<byte>.Shared.Return(recv);

            manager.Unblock(Id);
        }
    }
}
