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

            hullMax = reader.ReadDouble();
            shieldsMax = reader.ReadDouble();
            energyMax = reader.ReadDouble();
            ionMax = reader.ReadDouble();

            if (reduced)
                return;

            hull = reader.ReadDouble();
            shields = reader.ReadDouble();
            energy = reader.ReadDouble();
            ion = reader.ReadDouble();
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
