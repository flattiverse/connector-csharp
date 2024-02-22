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

        private Dictionary<string, ControllableInfo> controllableInfos = new Dictionary<string, ControllableInfo>();

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
            controllableInfos[info.Name] = info;
        }

        internal void RemoveControllableInfo(string name)
        {
            if(controllableInfos.TryGetValue(name, out ControllableInfo? info))
            {
                info.Deactivate();
                controllableInfos.Remove(name);
            }
        }

        public override string ToString()
        {
            return $"Player [{ID}] {Name}({Kind})";
        }
    }
}
