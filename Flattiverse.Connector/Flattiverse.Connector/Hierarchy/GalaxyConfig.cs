using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    public class GalaxyConfig
    {
        public string Name;
        public string Description;
        public GameType GameType;
        public int MaxPlayers;

        public int MaxPlatformsUniverse;
        public int MaxProbesUniverse;
        public int MaxDronesUniverse;
        public int MaxShipsUniverse;
        public int MaxBasesUniverse;

        public int MaxPlatformsTeam;
        public int MaxProbesTeam;
        public int MaxDronesTeam;
        public int MaxShipsTeam;
        public int MaxBasesTeam;

        public int MaxPlatformsPlayer;
        public int MaxProbesPlayer;
        public int MaxDronesPlayer;
        public int MaxShipsPlayer;
        public int MaxBasesPlayer;

        public GalaxyConfig(Galaxy galaxy)
        {
            Name = galaxy.Name;
            Description = galaxy.Description;
            GameType = galaxy.GameType;
            MaxPlayers = galaxy.MaxPlayers;

            MaxPlatformsUniverse = galaxy.MaxPlatformsUniverse;
            MaxProbesUniverse = galaxy.MaxProbesUniverse;
            MaxDronesUniverse = galaxy.MaxDronesUniverse;
            MaxShipsUniverse = galaxy.MaxShipsUniverse;
            MaxBasesUniverse = galaxy.MaxBasesUniverse;

            MaxPlatformsTeam = galaxy.MaxPlatformsTeam;
            MaxProbesTeam = galaxy.MaxProbesTeam;
            MaxDronesTeam = galaxy.MaxDronesTeam;
            MaxShipsTeam = galaxy.MaxShipsTeam;
            MaxBasesTeam = galaxy.MaxBasesTeam;

            MaxPlatformsPlayer = galaxy.MaxPlatformsPlayer;
            MaxProbesPlayer = galaxy.MaxProbesPlayer;
            MaxDronesPlayer = galaxy.MaxDronesPlayer;
            MaxShipsPlayer = galaxy.MaxShipsPlayer;
            MaxBasesPlayer = galaxy.MaxBasesPlayer;
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write(Description);
            writer.Write((byte)GameType);
            writer.Write(MaxPlayers);
            writer.Write(MaxPlatformsUniverse);
            writer.Write(MaxProbesUniverse);
            writer.Write(MaxDronesUniverse);
            writer.Write(MaxShipsUniverse);
            writer.Write(MaxBasesUniverse);
            writer.Write(MaxPlatformsTeam);
            writer.Write(MaxProbesTeam);
            writer.Write(MaxDronesTeam);
            writer.Write(MaxShipsTeam);
            writer.Write(MaxBasesTeam);
            writer.Write(MaxPlatformsPlayer);
            writer.Write(MaxProbesPlayer);
            writer.Write(MaxDronesPlayer);
            writer.Write(MaxShipsPlayer);
            writer.Write(MaxBasesPlayer);
        }
    }
}
