using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : Unit
    {
        private readonly Cluster cluster;
        private readonly Player player;
        private readonly ControllableInfo controllableInfo;

        private readonly double radius;
        private readonly double gravity;

        private double thruster;
        private double nozzle;
        private double turnrate;
        private double weaponAmmo;

        private double direction;
        private Vector position;
        private Vector movement;

        private bool active;

        public Player Player => player;
        public ControllableInfo ControllableInfo => controllableInfo;
        public double Thruster => thruster;
        public double Nozzle => nozzle;
        public double Turnrate => turnrate;
        public double WeaponAmmo => weaponAmmo;
        public bool IsActive => active;

        internal PlayerUnit(Cluster cluster, PacketReader reader) : base(reader)
        {
            this.cluster = cluster;

            player = cluster.Galaxy.GetPlayer(reader.ReadByte());
            controllableInfo = player.ControllableInfos[reader.ReadByte()];

            radius = reader.ReadDouble();
            gravity = reader.ReadDouble();
            thruster = reader.ReadDouble();
            nozzle = reader.ReadDouble();
            turnrate = reader.ReadDouble();
            weaponAmmo = reader.ReadDouble();

            direction = reader.ReadDouble();
            position = new Vector(reader);
            movement = new Vector(reader);

            active = true;
        }

        internal override void Update(PacketReader reader)
        {
            base.Update(reader);

            thruster = reader.ReadDouble();
            nozzle = reader.ReadDouble();
            turnrate = reader.ReadDouble();
            weaponAmmo = reader.ReadDouble();
            
            direction = reader.ReadDouble();
            position = new Vector(reader);
            movement = new Vector(reader);
        }
        
        public override double Direction => direction;
        public override Vector Position => position;
        public override Vector Movement => movement;
        public override Cluster Cluster => cluster;
        public override double Radius => radius; 
        public override double Gravity => gravity;
        public override Team Team => Player.Team;
        public override UnitKind Kind => UnitKind.PlayerUnit;
        
        internal void Deactivate()
        {
            active = false;
        }
    }
}