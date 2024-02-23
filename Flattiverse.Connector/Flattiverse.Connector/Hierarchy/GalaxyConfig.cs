using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class GalaxyConfig
    {
        public string Name;
        public string Description;
        public GameType GameType;
        public int MaxPlayers;

        public int MaxPlatformsGalaxy;
        public int MaxProbesGalaxy;
        public int MaxDronesGalaxy;
        public int MaxShipsGalaxy;
        public int MaxBasesGalaxy;

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

        internal GalaxyConfig(GalaxyConfig galaxy)
        {
            Name = galaxy.Name;
            Description = galaxy.Description;
            GameType = galaxy.GameType;
            MaxPlayers = galaxy.MaxPlayers;

            MaxPlatformsGalaxy = galaxy.MaxPlatformsGalaxy;
            MaxProbesGalaxy = galaxy.MaxProbesGalaxy;
            MaxDronesGalaxy = galaxy.MaxDronesGalaxy;
            MaxShipsGalaxy = galaxy.MaxShipsGalaxy;
            MaxBasesGalaxy = galaxy.MaxBasesGalaxy;

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

        internal GalaxyConfig(PacketReader reader)
        {
            Name = reader.ReadString();
            Description = reader.ReadString();
            GameType = (GameType)reader.ReadByte();
            MaxPlayers = reader.ReadByte();
            MaxPlatformsGalaxy = reader.ReadUInt16();
            MaxProbesGalaxy = reader.ReadUInt16();
            MaxDronesGalaxy = reader.ReadUInt16();
            MaxShipsGalaxy = reader.ReadUInt16();
            MaxBasesGalaxy = reader.ReadUInt16();
            MaxPlatformsTeam = reader.ReadUInt16();
            MaxProbesTeam = reader.ReadUInt16();
            MaxDronesTeam = reader.ReadUInt16();
            MaxShipsTeam = reader.ReadUInt16();
            MaxBasesTeam = reader.ReadUInt16();
            MaxPlatformsPlayer = reader.ReadByte();
            MaxProbesPlayer = reader.ReadByte();
            MaxDronesPlayer = reader.ReadByte();
            MaxShipsPlayer = reader.ReadByte();
            MaxBasesPlayer = reader.ReadByte();
        }

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write(Description);
            writer.Write((byte)GameType);
            writer.Write(MaxPlayers);
            writer.Write(MaxPlatformsGalaxy);
            writer.Write(MaxProbesGalaxy);
            writer.Write(MaxDronesGalaxy);
            writer.Write(MaxShipsGalaxy);
            writer.Write(MaxBasesGalaxy);
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
