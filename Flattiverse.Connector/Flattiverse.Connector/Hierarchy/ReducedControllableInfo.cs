using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ReducedControllableInfo
    {
        private Cluster cluster;

        public readonly string Name;

        private ShipDesign shipDesign;

        private Player player;

        private int playerId;

        private int shipDesignId;

        private int upgradeIndex;

        public ShipDesign ShipDesign => shipDesign;

        public Player Player => player;

        public int PlayerId => playerId;
        public int ShipDesignId => shipDesignId;
        public int UpgradeIndex => upgradeIndex;

        internal ReducedControllableInfo(Cluster cluster, PacketReader reader) 
        {
            this.cluster = cluster;

            Name = reader.ReadString();

            playerId = reader.ReadInt32();
            shipDesignId = reader.ReadInt32();

            shipDesign = cluster.Galaxy.Ships[shipDesignId];
            player = cluster.Galaxy.GetPlayer(playerId);
            upgradeIndex = reader.ReadInt32();
        }
    }
}
