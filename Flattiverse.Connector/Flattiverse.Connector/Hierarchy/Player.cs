using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    public class Player : INamedUnit
    {
        public readonly Galaxy Galaxy;
        
        public readonly byte ID;
        private readonly string name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        private bool active;

        internal readonly ControllableInfo?[] controllableInfos;
        public readonly UniversalHolder<ControllableInfo> ControllableInfos;

        public bool Active => active;

        internal Player(Galaxy galaxy, byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            Galaxy = galaxy;
            active = true;
            ID = id;
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
            packet.Header.Id0 = ID;

            packet.Header.Size = (ushort)System.Text.Encoding.UTF8.GetBytes(message.AsSpan(), packet.Payload.AsSpan(8, 1024));

            await session.SendWait(packet);
        }

        public override string ToString()
        {
            return $"Player [{ID}] {name} ({Kind})";
        }
    }
}
