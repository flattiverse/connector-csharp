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

        public Packet Request => new Packet() { Session = id, SessionUsed = true };

        internal void Disconnected()
        {
            ThreadPool.QueueUserWorkItem(delegate { tcs.SetException(new FlattiverseDisconnectedException()); });
        }

        internal void Answer(Packet packet)
        {
            if (packet.Command == 0xFF)
            {
                Exception exception;

                switch (packet.Helper)
                {
                    case 0x01:
                        exception = new UniverseServerUnhandledException();
                        break;
                    case 0x02:
                        exception = new InvalidParameterException();
                        break;
                    case 0x03:
                        exception = new AccountDoesntExistException();
                        break;
                    case 0x04:
                        exception = new OperationRequiresAdminStatusException();
                        break;
                    case 0x05:
                        exception = new PermissionDeniedException(packet.SubAddress);
                        break;
                    case 0x06:
                        exception = new IllegalNameException();
                        break;
                    case 0x07:
                        exception = new UnitDoesntExistException();
                        break;
                    case 0x08:
                        exception = new NoUniverseAssignmentException();
                        break;
                    case 0x09:
                        exception = new WrongStateException();
                        break;
                    case 0x0A:
                        exception = new TooManyEntriesException();
                        break;
                    case 0x10:
                        exception = new JoinRefusedException(packet.SubAddress);
                        break;
                    case 0x11:
                        exception = new PartException(packet.SubAddress);
                        break;
                    case 0x20:
                        exception = new UniverseDoesntExistException();
                        break;
                    case 0x21:
                        exception = new UniverseOfflineException();
                        break;
                    case 0x22:
                        exception = new UniverseGoneWhileExecutingRequestException();
                        break;
                    case 0x23:
                        exception = new NoUniverseAvailableException();
                        break;
                    case 0x24:
                        exception = new GalaxyDoesntExistException();
                        break;
                    case 0x60:
                        exception = new NonEditableUnitException();
                        break;
                    case 0x61:
                        exception = new AmbiguousXmlDataException();
                        break;
                    case 0xFB:
                        {
                            BinaryMemoryReader tmpReader = packet.Read();

                            string message = tmpReader.ReadString();
                            string parameter = tmpReader.ReadString();

                            exception = new ArgumentException(message, parameter);
                        }
                        break;
                    case 0xFC:
                        {
                            BinaryMemoryReader tmpReader = packet.Read();

                            string message = tmpReader.ReadString();
                            string parameter = tmpReader.ReadString();

                            exception = new ArgumentNullException(parameter, message);
                        }
                        break;
                    case 0xFD:
                        {
                            string message = packet.Read().ReadString();

                            exception = new InvalidOperationException(message);
                        }
                        break;
                    case 0xFE:
                        exception = new Exception("This exception will be replaced with an generic exception.");
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
            return await tcs.Task.ConfigureAwait(false);
        }

        public void Dispose()
        {
            Connection.DeleteSession(this);
        }
    }
}
