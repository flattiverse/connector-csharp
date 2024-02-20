using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        public readonly Cluster Cluster;

        internal UnitEvent(Galaxy galaxy/*, JsonElement element*/) : base()
        {

            Cluster = galaxy.Clusters[0];
        }
    }
}
