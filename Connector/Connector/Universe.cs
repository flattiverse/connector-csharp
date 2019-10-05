using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

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
        private byte maxShipsPerPlayer;
        private ushort maxShipsPerTeam;

        internal Team[] teams;

        /// <summary>
        /// The teams residing in this universe.
        /// </summary>
        public readonly UniversalHolder<Team> Teams;

        internal Universe(Packet packet)
        {
            ID = packet.BaseAddress;

            teams = new Team[16];
            Teams = new UniversalHolder<Team>(teams);

            updateFromPacket(packet);
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();
            description = reader.ReadString();

            difficulty = (Difficulty)reader.ReadByte();
            mode = (UniverseMode)reader.ReadByte();

            ownerID = reader.ReadUInt32();

            maxPlayers = reader.ReadUInt16();
            maxShipsPerPlayer = reader.ReadByte();
            maxShipsPerTeam = reader.ReadUInt16();

            status = (UniverseStatus)reader.ReadByte();
            defaultPrivileges = (Privileges)reader.ReadByte();
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
        /// The maximum amount of ships per player of this universe.
        /// </summary>
        public int MaxShipsPerPlayer => maxShipsPerPlayer;

        /// <summary>
        /// The maximum amount of ships per team of this universe.
        /// </summary>
        public int MaxShipsPerTeam => maxShipsPerTeam;
    }
}
