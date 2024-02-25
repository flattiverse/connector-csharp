using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    public class Player : INamedUnit
    {
        public readonly byte ID;
        private readonly string name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        private bool active;

        internal readonly ControllableInfo?[] controllableInfos;
        public readonly UniversalHolder<ControllableInfo> ControllableInfos;

        public bool Active => active;

        internal Player(byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            active = true;
            ID = id;
            Kind = kind;
            Team = team;
            
            name = reader.ReadString();
            Console.WriteLine(reader.ReadUInt32()); // Just to read the Ping Value.

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

        public override string ToString()
        {
            return $"Player [{ID}] {name}({Kind})";
        }
    }
}
