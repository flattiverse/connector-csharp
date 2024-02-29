using Flattiverse.Connector.Network;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// The configuration used by a cluster.
    /// </summary>
    /// <see cref="Cluster" />
    public class ClusterConfig
    {
        private string name;

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

        private ClusterConfig()
        {
            Name = string.Empty;
        }

        internal ClusterConfig(ClusterConfig cluster)
        {
            Name = cluster.Name;
        }

        internal ClusterConfig(PacketReader reader)
        {
            Name = reader.ReadString();
        }

        internal static ClusterConfig Default => new ClusterConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
        }
    }
}
