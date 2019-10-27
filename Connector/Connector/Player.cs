using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// This class represents a player currently in the game.
    /// </summary>
    public class Player : UniversalEnumerable
    {
        /// <summary>
        /// The server over which you have received this player infos.
        /// </summary>
        public readonly Server Server;

        private int id;
        private string name;
        private bool online;
        private float ping;

        private uint account;

        private Universe universe;
        private Team team;

        internal Player(Server server, Packet packet)
        {
            Server = server;

            id = packet.BaseAddress;
            account = packet.ID;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();
            online = reader.ReadBoolean();
            ping = reader.ReadSingle();
        }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// true, if the player is still online. false otherwise. If false the player may have lingering ships.
        /// </summary>
        public bool Online => online;

        /// <summary>
        /// The ping in seconds of the players connection.
        /// </summary>
        public float Ping => ping;

        /// <summary>
        /// The universe the player did join. null, if the player isn't in an universe.
        /// </summary>
        public Universe Universe => universe;

        /// <summary>
        /// The team the player is on. null, if the player isn't in an universe.
        /// </summary>
        public Team Team => team;

        internal void UpdatePing(Packet packet)
        {
            ping = packet.Read().PeekSingle();
        }

        internal void UpdatePlayerAssignment(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            if (reader.Size == 0)
            {
                universe = null;
                team = null;

                return;
            }

            universe = Server.universes[reader.ReadUInt16()];

            if (universe == null)
                return;

            team = universe.teams[reader.ReadByte()];
        }
    }
}
