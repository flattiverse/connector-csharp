using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// This class contains information about a controllable.
    /// You will receive this information when you join a galaxy and when a controllable is updated.
    /// </summary>
    public class ControllableInfo : INamedUnit
    {
        private Galaxy galaxy;

        /// <summary>
        /// A local unique identifier for the controllable. This is not the same as the player id.
        /// Used to differentiate between multiple controllables of the same player.
        /// </summary>
        public readonly int Id;

        private readonly string name;

        /// <summary>
        /// If the controllable is part of the enemy team, you will receive limited information about it.
        /// </summary>
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

        private bool alive;

        private bool active;

        /// <summary>
        /// ShipDesign contains the basis stats of the ship.
        /// </summary>
        /// <remarks>
        /// Upgrades can be applied, but it wont be reflected in this object.
        /// </remarks>
        public ShipDesign ShipDesign => shipDesign;

        /// <summary>
        /// The player that owns this controllable.
        /// </summary>
        public Player Player => player;

        /// <summary>
        /// The hull level of the controllable.
        /// If it reaches 0, the controllable is destroyed.
        /// The player can call continue to respawn the controllable.
        /// </summary>
        public double Hull => hull;

        /// <summary>
        /// The maximum hull level of the controllable.
        /// </summary>
        /// <seealso cref="Hull"/>
        public double HullMax => hullMax;

        /// <summary>
        /// The current shield value of the ship.
        /// It will be depleted before the hull is damaged.
        /// </summary>
        /// <remarks>
        /// Depending on a value in ShipDesign you may need Ion to recharge the shields.
        /// </remarks>
        public double Shields => shields;

        /// <summary>
        /// The maximum shield level of the controllable.
        /// </summary>
        /// <seealso cref="Shields"/>
        public double ShieldsMax => shieldsMax;
   
        /// <summary>
        /// The current energy value of the ship.
        /// It can be gained via sections/coronas around suns or via the energy reactor of stations.
        /// </summary>
        /// <remarks>
        /// Energy is used for the thruster and the weapons.
        /// It can also be used to repair the hull or recharge the Shields.
        /// </remarks>
        public double Energy => energy;

        /// <summary>
        /// The maximum energy value of the ship.
        /// </summary>
        /// <seealso cref="Energy" />
        public double EnergyMax => energyMax;
 
         /// <summary>
        /// The current ion value of the ship.
        /// Ion is used to recharge the shields.
        /// </summary>
        /// <remarks>
        /// Ios can be gained via corona secions configured in the map.
        /// </remarks>
        public double Ion => ion;

        /// <summary>
        /// The maximum ion value of the ship.
        /// </summary>
        /// <seealso cref="Ion" />
        public double IonMax => ionMax;

        /// <summary>
        /// This flag indicates if the controllable can receive commands other than continue and dispose.
        /// </summary>
        public bool IsAlive => alive;

        /// <summary>
        /// This flag indicates if the controllable is able to be interacted with.
        /// </summary>
        /// <remarks>
        /// If it is inactive and you try to interact with it, you will get a GameException.
        /// </remarks>
        public bool IsActive => active;

        /// <summary>
        /// The name of the controllable.
        /// </summary>
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

        internal void DynamicUpdate(PacketReader reader, bool reduced)
        {
            if (reduced)
                alive = reader.ReadBoolean();
            else
            {
                hull = reader.ReadDouble();
                alive = hull > 0;
                shields = reader.ReadDouble();
                energy = reader.ReadDouble();
                ion = reader.ReadDouble();
            }            
        }

        internal void Deactivate()
        {
            active = false;
        }
    }
}
