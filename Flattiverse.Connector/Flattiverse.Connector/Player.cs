using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    class Player
    {
        public readonly byte ID;
        public readonly string Name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        public Player(byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            ID = id;
            Kind = kind;
            Team = team;
            
            Name = reader.ReadString();
        }
    }
}
