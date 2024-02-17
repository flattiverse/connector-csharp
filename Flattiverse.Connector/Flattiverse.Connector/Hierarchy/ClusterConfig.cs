using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ClusterConfig
    {
        public string Name;

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
