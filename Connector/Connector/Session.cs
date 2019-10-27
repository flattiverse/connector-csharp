using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    class Session : IDisposable
    {
        private byte id;

        public readonly Connection Connection;

        private TaskCompletionSource<Packet> tcs;

        public Session(Connection connection)
        {
            Connection = connection;

            tcs = new TaskCompletionSource<Packet>();
        }

        internal void Setup(byte id)
        {
            this.id = id;
        }

        public int ID => id;

        public Packet Request => new Packet() { Session = id };

        internal void Answer(Packet packet)
        {
            if (packet.Command == 0xFF)
            {


                return;
            }

            tcs.SetResult(packet);
        }

        internal async Task<Packet> Wait()
        {
            return await tcs.Task;
        }

        public void Dispose()
        {
            Connection.DeleteSession(this);
        }
    }
}
