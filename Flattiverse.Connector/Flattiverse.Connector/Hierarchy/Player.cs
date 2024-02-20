using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class Player
    {
        public readonly byte ID;
        public readonly string Name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        internal Player(byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            ID = id;
            Kind = kind;
            Team = team;
            
            Name = reader.ReadString();
        }

        public override string ToString()
        {
            return $"Player [{ID}] {Name}({Kind})";
        }
    }
}
