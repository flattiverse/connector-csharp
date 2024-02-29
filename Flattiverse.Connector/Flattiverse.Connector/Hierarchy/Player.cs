using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// A player that is registered in a galaxy and can register ships in a cluster.
    /// </summary>
    public class Player : INamedUnit
    {
        /// <summary>
        /// The galaxy the player is in.
        /// </summary>
        public readonly Galaxy Galaxy;
        
        /// <summary>
        /// The ID of the player. This is unique in the galaxy.
        /// </summary>
        public readonly byte Id;

        private readonly string name;

        /// <summary>
        /// The kind of the player.
        /// </summary>
        /// <remarks>
        /// Can be Player, Spectator or Admin.
        /// </remarks>
        public readonly PlayerKind Kind;

        /// <summary>
        /// The team the player belongs to.
        /// </summary>
        public readonly Team Team;

        private bool active;

        internal readonly ControllableInfo?[] controllableInfos;

        /// <summary>
        /// A collection of all controllable infos the player knows about.
        /// </summary>
        public readonly UniversalHolder<ControllableInfo> ControllableInfos;

        /// <summary>
        /// This flag indicates if the player is still part of the active simulation.
        /// </summary>
        public bool IsActive => active;

        internal Player(Galaxy galaxy, byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            Galaxy = galaxy;
            active = true;
            Id = id;
            Kind = kind;
            Team = team;
            
            name = reader.ReadString();
            reader.ReadUInt32(); // Just to read the Ping Value.

            controllableInfos = new ControllableInfo?[256];
            ControllableInfos = new UniversalHolder<ControllableInfo>(controllableInfos);
        }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name => name;
        
        internal void Deactivate()
        {
            active = false;
        }

        internal void AddControllableInfo(ControllableInfo info)
        {
            controllableInfos[info.Id] = info;
        }

        internal void RemoveControllableInfo(int id)
        {
            if (controllableInfos[id] is ControllableInfo info)
            {
                info.Deactivate();
                controllableInfos[id] = null;
            }
        }
        
        /// <summary>
        /// Sends a chat message to the player.
        /// </summary>
        /// <param name="message">A message with a maximum of 512 chars.</param>
        /// <exception cref="GameException"></exception>
        public async Task Chat(string message)
        {
            if (!active)
                throw new GameException(0x22);
            
            if (!Utils.CheckMessage(message))
                throw new GameException(0x31);
            
            Session session = await Galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x20;
            packet.Header.Id0 = Id;

            packet.Header.Size = (ushort)System.Text.Encoding.UTF8.GetBytes(message.AsSpan(), packet.Payload.AsSpan(8, 1024));

            await session.SendWait(packet);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Player [{Id}] {name} ({Kind})";
        }
    }
}
