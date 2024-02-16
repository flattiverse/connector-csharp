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

        public ClusterConfig(Cluster cluster)
        {
            Name = cluster.Name;
        }
        internal static ClusterConfig Default => new ClusterConfig();

        internal void Write(PacketWriter writer)
        {
            writer.Write(Name);
        }
    }
}
