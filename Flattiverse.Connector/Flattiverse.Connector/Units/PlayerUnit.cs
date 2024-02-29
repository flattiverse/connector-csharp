using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// Player units are controlled by other players.
    /// Can be either firend or enemy based on team affilitaion.
    /// </summary>
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

        /// <summary>
        /// The player controlling this unit.
        /// </summary>
        public Player Player => player;

        /// <summary>
        /// The ship information of the player.
        /// </summary>
        /// <remarks>
        /// Can be reduced if it belong to a enemy team.
        /// </remarks>
        public ControllableInfo ControllableInfo => controllableInfo;

        /// <summary>
        /// The current thruster value of the ship.
        /// Can either be positive or negative.
        /// This is the ships forward/backward acceleration in km/tick².
        /// </summary>
        /// <remarks>
        /// A positive thruster value will make your ship advance forward relative to its orientation.
        /// A negative thruster value will make your ship advance backwards relative to its orientation.
        /// The negative value is usually limited to smaller value than the positive one.
        /// </remarks>
        public double Thruster => thruster;

        /// <summary>
        /// The current nozzle value of the ship.
        /// Can either be positive or negative.
        /// This is the ships turning/angular acceleration in degrees/tick².
        /// </summary>
        /// <remarks>
        /// A positive nozzle value will increase the Angle, a negative decrease.
        /// </remarks>
        public double Nozzle => nozzle;

        /// <summary>
        /// The turnrate of the ship in degrees/tick.
        /// This is the current angular velocity of the ship.
        /// Can be positive or negative.
        /// </summary>
        public double Turnrate => turnrate;

        /// <summary>
        /// The current ammunition reserve of the weapon.
        /// </summary>
        public double WeaponAmmo => weaponAmmo;

        /// <summary>
        /// Indicates if the unit is part of the active simulation.
        /// </summary>
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

        /// <inheritdoc/>        
        public override double Direction => direction;

        /// <inheritdoc/>
        public override Vector Position => position;

        /// <inheritdoc/>
        public override Vector Movement => movement;

        /// <inheritdoc/>
        public override Cluster Cluster => cluster;

        /// <inheritdoc/>
        public override double Radius => radius;

        /// <inheritdoc/>
        public override double Gravity => gravity;

        /// <inheritdoc/>
        public override Team Team => Player.Team;

        /// <inheritdoc/>
        public override UnitKind Kind => UnitKind.PlayerUnit;

        /// <inheritdoc/>
        public override Mobility Mobility => Mobility.Mobile;

        internal void Deactivate()
        {
            active = false;
        }
    }
}