using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
                Exception exception;

                switch (packet.Helper)
                {
                    case 0x10:
                        exception = new JoinRefusedException(packet.SubAddress);
                        break;
                    case 0x11:
                        exception = new PartException(packet.SubAddress);
                        break;
                    case 0xFF:
                        BinaryMemoryReader reader = packet.Read();
                        exception = new InvalidProgramException($"!!! INVALID EXCEPTION COUGHT BY SERVER !!!\n\nThe server has cought a \"{reader.ReadString()}\" and did just forward this to the client (you). The exception has the following message:\n\n{reader.ReadString()}\n\nAnd the following stack trace:\n\n{reader.ReadString()}\n\nIf you are in the C# course of the HS-Esslingen: Contact your teacher.");
                        break;
                    default:
                        exception = new Exception($"Unknown Exception Code: 0x{packet.Helper.ToString()}.");
                        break;
                }

                ThreadPool.QueueUserWorkItem(delegate { tcs.SetException(exception); });

                return;
            }

            ThreadPool.QueueUserWorkItem(delegate { tcs.SetResult(packet); });
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
