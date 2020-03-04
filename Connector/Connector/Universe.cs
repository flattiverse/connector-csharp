using Flattiverse.Utils;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Flattiverse
{
    /// <summary>
    /// An universe.
    /// </summary>
    public class Universe : UniversalEnumerable
    {
        /// <summary>
        /// The ID of the universe.
        /// </summary>
        public readonly ushort ID;

        private string name;
        private string description;

        private uint ownerID;

        private Difficulty difficulty;
        private UniverseMode mode;

        private UniverseStatus status;
        private Privileges defaultPrivileges;

        private ushort maxPlayers;
        private ushort maxPlayersPerTeam;
        private byte maxShipsPerPlayer;
        private ushort maxShipsPerTeam;

        internal Team[] teams;

        internal Galaxy[] galaxies;

        /// <summary>
        /// The server where this universe is hosted.
        /// </summary>
        public readonly Server Server;

        /// <summary>
        /// The teams residing in this universe.
        /// </summary>
        public readonly UniversalHolder<Team> Teams;

        /// <summary>
        /// The galaxies in this universe.
        /// </summary>
        public readonly UniversalHolder<Galaxy> Galaxies;

        private List<UniverseSystem> systems;

        internal Universe(Server server, Packet packet)
        {
            Server = server;
            ID = packet.BaseAddress;

            teams = new Team[16];
            Teams = new UniversalHolder<Team>(teams);

            galaxies = new Galaxy[32];
            Galaxies = new UniversalHolder<Galaxy>(galaxies);

            systems = new List<UniverseSystem>();

            updateFromPacket(packet);
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadStringNonNull();
            description = reader.ReadStringNonNull();

            difficulty = (Difficulty)reader.ReadByte();
            mode = (UniverseMode)reader.ReadByte();

            ownerID = reader.ReadUInt32();

            maxPlayers = reader.ReadUInt16();
            maxPlayersPerTeam = reader.ReadUInt16();
            maxShipsPerPlayer = reader.ReadByte();
            maxShipsPerTeam = reader.ReadUInt16();

            status = (UniverseStatus)reader.ReadByte();
            defaultPrivileges = (Privileges)reader.ReadByte();
        }

        /// <summary>
        /// Queries all privileges assigned to this universe.
        /// </summary>
        /// <returns>An enumerator returning KeyValuePairs of Account and Privileges. The entry can be orphaned, when the corresponding Account is null.</returns>
        public IEnumerable<KeyValuePair<Account, Privileges>> QueryPrivileges()
        {
            using (System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(false))
            {
                List<KeyValuePair<uint, Privileges>> ids = new List<KeyValuePair<uint, Privileges>>();

                using (Session session = Server.connection.NewSession())
                {
                    Packet packet = session.Request;

                    packet.Command = 0x44;
                    packet.BaseAddress = ID;

                    Server.connection.Send(packet);
                    Server.connection.Flush();

                    ThreadPool.QueueUserWorkItem(async delegate {
                        // I hate you for forcing me to do this, microsoft. Really.
                        packet = await session.Wait().ConfigureAwait(false);
                        are.Set();
                    });

                    are.WaitOne();

                    BinaryMemoryReader reader = packet.Read();

                    while (reader.Size > 0)
                        ids.Add(new KeyValuePair<uint, Privileges>(reader.ReadUInt32(), (Privileges)reader.ReadByte()));
                }

                Account account = null;

                foreach (KeyValuePair<uint, Privileges> kvp in ids)
                {
                    ThreadPool.QueueUserWorkItem(async delegate {
                        // I hate you for forcing me to do this, microsoft. Really.
                        account = (await Server.QueryAccount(kvp.Key).ConfigureAwait(false)) ?? new Account(Server, kvp.Key);
                        are.Set();
                    });

                    are.WaitOne();

                    yield return new KeyValuePair<Account, Privileges>(account, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Changes the privileges of an account of this universe. Use the same Privileges as this.DefaultPrivileges to remove the ACL (access control list) entry.
        /// </summary>
        /// <param name="account">The account to change the settings.</param>
        /// <param name="privileges">The privileges to change the settings to.</param>
        public async Task AlterPrivileges(Account account, Privileges privileges)
        {
            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x45;
                packet.BaseAddress = ID;
                packet.ID = account.ID;
                packet.Helper = (byte)privileges;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks whether the name is already used and meets the requirements.
        /// </summary>
        /// <param name="name">The name that should be checked.</param>
        /// <returns>True, when the the name meets the requirements and is not yet used. Returns false when requirements don't match or the name is in use in this universe. Also returns false when all universes are offline.</returns>
        public async Task<bool> CheckName(string name)
        {
            if (name == null || !Units.Unit.CheckName(name))
                return false;

            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x64;
                packet.BaseAddress = ID;

                packet.Write().Write(name);

                Server.connection.Send(packet);
                Server.connection.Flush();

                return (await session.Wait().ConfigureAwait(false)).Read().ReadBoolean();
            }
        }

        /// <summary>
        /// Queries the owner account.
        /// </summary>
        /// <returns></returns>
        public async Task<Account> QueryOwner()
        {
            return await Server.QueryAccount(ownerID);
        }

        /// <summary>
        /// Sending a message to all players in this universe.
        /// </summary>
        /// <param name="message">The chat message that should be send to all players of the universe.</param>
        /// <exception cref="ArgumentException">The chat message is larger than 256 characters.</exception>
        /// <exception cref="ArgumentNullException">The chat message is null.</exception>
        public async Task Chat(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "The chat message can't be null.");

            if (message.Length > 256)
                throw new ArgumentException("The chat message is not allowed to have more than 256 characters!", nameof(message));

            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x66;
                packet.BaseAddress = ID;

                packet.Write().Write(message);

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Joins the universe and selects the team where fewer players are in. If a tournament is
        /// active it will join you the team of the tournament you are on or deny the access.
        /// </summary>
        /// <exception cref="Flattiverse.JoinRefusedException">Thrown, when the universe is full, or you don't have access because of another reason.</exception>
        public async Task Join()
        {
            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1A;

                // TODO: Autoselection.
                packet.BaseAddress = ID;
                packet.SubAddress = 0x00;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Joins the universe with the specified team.
        /// </summary>
        /// <param name="team">The team you want to be on.</param>
        /// <exception cref="Flattiverse.JoinRefusedException">Thrown, when the universe is full, or you don't have access because of another reason.</exception>
        public async Task Join(Team team)
        {
            if (team == null)
                throw new ArgumentNullException("team", "team can't be null.");

            bool teamAvailable = false;

            foreach (Team t in teams)
                if (t == team)
                {
                    teamAvailable = true;
                    break;
                }

            if (!teamAvailable)
                throw new ArgumentException("The given team must be part of the universe you want to join.", "team");

            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1A;
                packet.BaseAddress = ID;
                packet.SubAddress = team.ID;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates a new ship under your control.
        /// </summary>
        /// <param name="name">The name of the ship.</param>
        /// <returns>The controllable.</returns>
        public async Task<Controllable> NewShip(string name)
        {
            if (Server.Player.Universe != this || Server.Player.Team == null)
                throw new InvalidOperationException("You need to join as a player (with team assignment) first.");

            if (!Units.Unit.CheckName(name))
                throw new ArgumentException("name doesn't match unit naming criteria.", "name");

            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0xB0;

                packet.Write().Write(name);

                Server.connection.Send(packet);
                Server.connection.Flush();

                return Server.controllables[(await session.Wait().ConfigureAwait(false)).SubAddress];
            }
        }

        /// <summary>
        /// Closes your session with the universe.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown, you are not in the universe or when you still have active controllables.</exception>
        public async Task Part()
        {
            using (Session session = Server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1B;
                packet.BaseAddress = ID;
                packet.SubAddress = 0xFF;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The name of this universe.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The description of this universe.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// The difficulty of this universe.
        /// </summary>
        public Difficulty Difficulty => difficulty;

        /// <summary>
        /// The gamemode of this universe.
        /// </summary>
        public UniverseMode Mode => mode;

        /// <summary>
        /// The status of this universe.
        /// </summary>
        public UniverseStatus Status => status;

        /// <summary>
        /// The default privileges of this universe.
        /// </summary>
        public Privileges DefaultPrivileges => defaultPrivileges;

        /// <summary>
        /// The maximum amount of players for this universe.
        /// </summary>
        public int MaxPlayers => maxPlayers;

        /// <summary>
        /// The maximum amount of players for a team in this universe.
        /// </summary>
        public int MaxPlayersPerTeam => maxPlayersPerTeam;

        /// <summary>
        /// The maximum amount of ships per player of this universe.
        /// </summary>
        public int MaxShipsPerPlayer => maxShipsPerPlayer;

        /// <summary>
        /// The maximum amount of ships per team of this universe.
        /// </summary>
        public int MaxShipsPerTeam => maxShipsPerTeam;

        /// <summary>
        /// The system configuration in the universe.
        /// </summary>
        public ReadOnlyCollection<UniverseSystem> Systems => new ReadOnlyCollection<UniverseSystem>(systems);

        internal void updateSystems(Packet packet)
        {
            List<UniverseSystem> systems = new List<UniverseSystem>();

            BinaryMemoryReader reader = packet.Read();

            for (int position = 0; reader.Size > 0; position++)
            {
                UniverseSystem universeSystem = new UniverseSystem(position, reader.ReadByte());

                if (universeSystem.InUse)
                    systems.Add(universeSystem);
            }

            this.systems = systems;
        }
    }
}
