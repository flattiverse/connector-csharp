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
        /// The symphony server over which you have received this player infos.
        /// </summary>
        public readonly Server Server;

        private int id;
        private string name;
        private bool online;
        private float ping;

        private uint account;

        internal Player(Server server, Packet packet)
        {
            Server = server;

            id = packet.BaseAddress;
            account = packet.ID;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();
            online = reader.ReadByte() != 0x00;
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

        internal void UpdatePing(Packet packet)
        {
            ping = packet.Read().PeekSingle();
        }
    }
}
