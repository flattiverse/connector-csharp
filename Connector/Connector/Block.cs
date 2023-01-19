using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    internal class Block : IDisposable
    {
        public string Id;

        private TaskCompletionSource taskCompletionSource;
        private BlockManager manager;
        public Packet? Packet;

        public Block(BlockManager manager, string Id) 
        {
            this.Id = Id;
            this.manager = manager;
            taskCompletionSource = new TaskCompletionSource();
        }

        public async Task Wait() 
        {
            await taskCompletionSource.Task;
        }

        public void Answer(Packet packet) 
        { 
            Packet = packet;
            taskCompletionSource.SetResult();
        }

        public void Dispose()
        {
            manager.Unblock(Id);
        }
    }
}
