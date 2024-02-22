using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    public class Player
    {
        public readonly byte ID;
        public readonly string Name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        private bool active;

        private Dictionary<int, ControllableInfo> controllableInfos = new Dictionary<int, ControllableInfo>();

        public bool Active => active;

        internal Player(byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            active = true;
            ID = id;
            Kind = kind;
            Team = team;
            
            Name = reader.ReadString();
        }

        //TODO: Deaktivate when removed
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
            if(controllableInfos.TryGetValue(id, out ControllableInfo? info))
            {
                info.Deactivate();
                controllableInfos.Remove(id);
            }
        }

        public override string ToString()
        {
            return $"Player [{ID}] {Name}({Kind})";
        }
    }
}
