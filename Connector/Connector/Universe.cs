using Flattiverse.Utils;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        internal Team?[] teams;

        internal Galaxy?[] galaxies;

        /// <summary>
        /// The server where this universe is hosted.
        /// </summary>
        public readonly Server Server;

        /// <summary>
        /// The teams residing in this universe.
        /// </summary>
        public readonly UniversalHolder<Team?> Teams;

        /// <summary>
        /// The galaxies in this universe.
        /// </summary>
        public readonly UniversalHolder<Galaxy?> Galaxies;

        private List<UniverseSystem> systems;

        internal Universe(Server server, Packet packet)
        {
            Server = server;
            ID = packet.BaseAddress;

            teams = new Team[16];
            Teams = new UniversalHolder<Team?>(teams);

            galaxies = new Galaxy[32];
            Galaxies = new UniversalHolder<Galaxy?>(galaxies);

            systems = new List<UniverseSystem>();

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
        /// Joins the universe and selects the team where fewer players are in. If a tournament is
        /// active it will join you the team of the tournament you are on or deny the access.
        /// </summary>
        /// <exception cref="Flattiverse.JoinRefusedException">Thrown, when the universe is full, or you don't have access because of another reason.</exception>
        public async Task Join()
        {
            using (Session session = Server.connection!.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1A;

                // TODO: Autoselection.
                packet.BaseAddress = ID;
                packet.SubAddress = 0x00;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait();
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

            foreach (Team? t in teams)
                if (t == team)
                {
                    teamAvailable = true;
                    break;
                }

            if (!teamAvailable)
                throw new ArgumentException("The given team must be part of the universe you want to join.", "team");

            using (Session session = Server.connection!.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1A;
                packet.BaseAddress = ID;
                packet.SubAddress = team.ID;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait();
            }
        }

        /// <summary>
        /// Closes your session with the universe.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown, you are not in the universe or when you still have active controllables.</exception>
        public async Task Part()
        {
            using (Session session = Server.connection!.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0x1B;
                packet.BaseAddress = ID;
                packet.SubAddress = 0xFF;

                Server.connection.Send(packet);
                Server.connection.Flush();

                await session.Wait();
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

            while (reader.Size > 0)
            {
                UniverseSystem universeSystem = new UniverseSystem(ref reader);

                if (universeSystem.InUse)
                    systems.Add(universeSystem);
            }

            this.systems = systems;
        }
    }
}
