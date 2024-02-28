using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : Unit
    {
        private Cluster cluster;

        private Player player;
        private ControllableInfo controllableInfo;

        private double size;
        private double weight;
        private double thruster;
        private double nozzle;
        private double turnrate;
        private double weaponAmmo;

        private double direction;
        private Vector position;
        private Vector movement;

        private bool active;

        public Player Player => player;
        public override Cluster Cluster => cluster;
        public ControllableInfo ControllableInfo => controllableInfo;
        public override double Radius => size;
        public double Weight => weight;
        public double Thruster => thruster;
        public double Nozzle => nozzle;
        public double Turnrate => turnrate;
        public double WeaponAmmo => weaponAmmo;
        public override double Direction => direction;
        public override Vector Position => position;
        public override Vector Movement => movement;
        public bool IsActive => active;

        internal PlayerUnit(Cluster cluster, PacketReader reader) : base(reader)
        {
            this.cluster = cluster;

            player = cluster.Galaxy.GetPlayer(reader.ReadByte());
            controllableInfo = this.player.ControllableInfos[reader.ReadByte()];

            size = reader.ReadDouble();
            weight = reader.ReadDouble();
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

        public override UnitKind Kind => UnitKind.PlayerUnit;
        
        internal void Deactivate()
        {
            active = false;
        }
    }
}