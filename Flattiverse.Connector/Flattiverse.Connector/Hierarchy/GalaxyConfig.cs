using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// This is a contains the configuration parameters of a galaxy.
    /// </summary>
    public class GalaxyConfig
    {
        private string name;

        /// <summary>
        /// A textual description of the galaxy.
        /// </summary>
        public string Description;

        /// <summary>
        /// The game mode of the galaxy.
        /// </summary>
        /// <remarks>
        /// Can be Mission, STF, Domination or Race
        /// </remarks>
        public GameMode Mode;

        /// <summary>
        /// The maximum amount of concurrent players in the galaxy.
        /// </summary>
        public int MaxPlayers;

        // TODO MALUK these could all be ushort
        /// <summary>
        /// Maximum amount of platforms in the galaxy.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxPlatformsGalaxy;

        /// <summary>
        /// Maximum amount of probes in the galaxy.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxProbesGalaxy;

        /// <summary>
        /// Maximum amount of drones in the galaxy.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxDronesGalaxy;

        /// <summary>
        /// Maximum amount of ships in the galaxy.
        /// </summary>
        public int MaxShipsGalaxy;

        /// <summary>
        /// Maximum amount of bases in the galaxy.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxBasesGalaxy;

        // TODO MALUK these could all be ushort
        /// <summary>
        /// Maximum amount of platforms per team.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxPlatformsTeam;

        /// <summary>
        /// Maximum amount of probes per team.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxProbesTeam;

        /// <summary>
        /// Maximum amount of drones per team.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxDronesTeam;

        /// <summary>
        /// Maximum amount of ships per team.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxShipsTeam;

        /// <summary>
        /// Maximum amount of bases per team.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxBasesTeam;

        // TODO MALUK these could all be byte
        /// <summary>
        /// Maximum amount of platforms per player.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxPlatformsPlayer;

        /// <summary>
        /// Maximum amount of probes per player.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxProbesPlayer;

        /// <summary>
        /// Maximum amount of drones per player.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
        public int MaxDronesPlayer;

        /// <summary>
        /// Maximum amount of ships per player.
        /// </summary>
        public int MaxShipsPlayer;

        /// <summary>
        /// Maximum amount of bases per player.
        /// </summary>
        /// <remarks>
        /// Unused.
        /// </remarks>
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
            Mode = galaxy.Mode;
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
            Mode = (GameMode)reader.ReadByte();
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
            writer.Write((byte)Mode);
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
