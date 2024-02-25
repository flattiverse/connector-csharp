using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    public class GalaxyConfig
    {
        private string name;
        public string Description;
        public GameType GameType;
        public int MaxPlayers;

        // TODO MALUK these could all be ushort
        public int MaxPlatformsGalaxy;
        public int MaxProbesGalaxy;
        public int MaxDronesGalaxy;
        public int MaxShipsGalaxy;
        public int MaxBasesGalaxy;

        // TODO MALUK these could all be ushort
        public int MaxPlatformsTeam;
        public int MaxProbesTeam;
        public int MaxDronesTeam;
        public int MaxShipsTeam;
        public int MaxBasesTeam;

        // TODO MALUK these could all be byte
        public int MaxPlatformsPlayer;
        public int MaxProbesPlayer;
        public int MaxDronesPlayer;
        public int MaxShipsPlayer;
        public int MaxBasesPlayer;

        /// <summary>
        /// The name of the configured unit.
        /// </summary>
        /// <exception cref="GameException">0x32 may be thrown, if the name violates rules.</exception>
        public string Name
        {
            get => name;
            set
            {
                if (!Utils.CheckName32(value))
                    throw new GameException(0x31);

                name = value;
            }
        }

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
