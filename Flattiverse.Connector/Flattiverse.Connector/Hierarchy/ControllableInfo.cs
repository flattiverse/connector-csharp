using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    internal class ControllableInfo
    {
        private Cluster cluster;

        public readonly string Name;

        private ShipDesign shipDesign;

        private Player player;

        private int playerId;

        private int shipDesignId;

        private int upgradeIndex;

        private double hull;
        private double hullMax;

        private double shields;
        private double shieldsMax;

        private double energy;
        private double energyMax;

        private double ion;
        private double ionMax;

        public ShipDesign ShipDesign => shipDesign;
        public Player Player => player;
        public int PlayerId => playerId;
        public int ShipDesignId => shipDesignId;
        public int UpgradeIndex => upgradeIndex;
        public double Hull => hull;
        public double HullMax => hullMax;
        public double Shields => shields;
        public double ShieldsMax => shieldsMax;
   
        public double Energy => energy;
        public double EnergyMax => energyMax;
 
        public double Ion => ion;
        public double IonMax => ionMax;
   

        internal ControllableInfo(Cluster cluster, PacketReader reader)
        {
            this.cluster = cluster;

            Name = reader.ReadString();

            playerId = reader.ReadInt32();
            shipDesignId = reader.ReadInt32();

            shipDesign = cluster.Galaxy.Ships[shipDesignId];
            player = cluster.Galaxy.GetPlayer(playerId);
            upgradeIndex = reader.ReadInt32();

            hull = reader.Read2U(10);
            hullMax = reader.Read2U(10);

            shields = reader.Read2U(10);
            shieldsMax = reader.Read2U(10);

            energy = reader.Read4U(10);
            energyMax = reader.Read4U(10);

            ion = reader.Read2U(100);
            ionMax = reader.Read2U(100);
        }
    }
}
