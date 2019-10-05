using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    /// <summary>
    /// This represents a server you connected to.
    /// </summary>
    public class Server : IDisposable
    {
        private Connection connection;

        private Universe[] universes;

        /// <summary>
        /// All the universes available.
        /// </summary>
        public readonly UniversalHolder<Universe> Universes;

        private TaskCompletionSource<object> waiter;

        /// <summary>
        /// Creates a new instance of a server connection without connecting (use login for this).
        /// </summary>
        public Server()
        {
            universes = new Universe[16];

            Universes = new UniversalHolder<Universe>(universes);
        }

        /// <summary>
        /// Login to the flattiverse server.
        /// </summary>
        /// <param name="username">Your username.</param>
        /// <param name="password">Your password.</param>
        /// <returns>Nothing. (Just a task fpr async/await-pattern.)</returns>
        public async Task Login(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException("username", "username can't be null.");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Invalid username.", "username");

            if (password == null)
                throw new ArgumentNullException("password", "password can't be null.");

            connection = new Connection();

            connection.Disconnected += disconnected;
            connection.Received += received;

            waiter = new TaskCompletionSource<object>();

            await connection.Connect(username, password);

            await waiter.Task;

            waiter = null;
        }

        /// <summary>
        /// Login to the flattiverse server.
        /// </summary>
        /// <param name="username">Your username.</param>
        /// <param name="hash">Your hashed password. (Use Crypto.HashPassword.)</param>
        /// <returns>Nothing. (Just a task fpr async/await-pattern.)</returns>
        public async Task Login(string username, byte[] hash)
        {
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

            waiter = new TaskCompletionSource<object>();

            await connection.Connect(username, hash);

            await waiter.Task;

            waiter = null;
        }

        private void received(List<Packet> packets)
        {
            foreach (Packet packet in packets)
            {
                switch (packet.Command)
                {
                    case 0x0F: // Login Completed
                        waiter?.SetResult(null);
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
                            universes[packet.BaseAddress] = new Universe(packet);
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
                            universes[packet.BaseAddress].teams[packet.SubAddress].updateFromPacket(packet);
                        break;
                }
            }
        }

        private void disconnected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases all ressources of this instance.
        /// </summary>
        public void Dispose()
        {
            connection.Close();
        }
    }
}
