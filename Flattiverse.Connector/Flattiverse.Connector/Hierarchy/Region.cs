namespace Flattiverse.Connector.Hierarchy
{
    public class Region : INamedUnit
    {
        public readonly Galaxy Galaxy;
        public readonly Cluster Cluster;
        public readonly byte ID;

        private string name;

        /// <summary>
        /// The name of the region.
        /// </summary>
        public string Name => name;
    }
}
