using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    public class ControllableInfo : INamedUnit
    {
        private Galaxy galaxy;

        public readonly int Id;

        private readonly string name;

        public readonly bool Reduced;

        private ShipDesign shipDesign;

        private Player player;

        private byte[] upgrades;

        private double hull;
        private double hullMax;

        private double shields;
        private double shieldsMax;

        private double energy;
        private double energyMax;

        private double ion;
        private double ionMax;

        private bool active;

        public ShipDesign ShipDesign => shipDesign;
        public Player Player => player;
        public double Hull => hull;
        public double HullMax => hullMax;
        public double Shields => shields;
        public double ShieldsMax => shieldsMax;
   
        public double Energy => energy;
        public double EnergyMax => energyMax;
 
        public double Ion => ion;
        public double IonMax => ionMax;

        public bool Active => active;

        public string Name => name;

        internal ControllableInfo(Galaxy galaxy, Player player, PacketReader reader, int id, bool reduced)
        {
            active = true;
            this.galaxy = galaxy;
            this.player = player;
            Id = id;
            Reduced = reduced;

            name = reader.ReadString();

            shipDesign = galaxy.ShipsDesigns[reader.ReadByte()];
            
            upgrades = reader.ReadBytes(32);

            hullMax = reader.Read2U(10);
            shieldsMax = reader.Read2U(10);
            energyMax = reader.Read4U(10);
            ionMax = reader.Read2U(100);

            if (reduced)
                return;

            hull = reader.Read2U(10);
            shields = reader.Read2U(10);
            energy = reader.Read4U(10);
            ion = reader.Read2U(100);
        }

        internal void Update(PacketReader reader)
        {
            
        }

        internal void Deactivate()
        {
            active = false;
        }
    }
}
