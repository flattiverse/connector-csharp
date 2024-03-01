using System.Diagnostics;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Flattiverse.Connector
{
    /// <summary>
    /// The controllable class represents the ship you are currently controlling.
    /// </summary>
    /// <remarks>
    /// The server only knows about PlayerUnits.
    /// It does not know about the concept of a controllable.
    /// The client however has 2 distinct views of the PlayerUnit:
    /// 1. The Controllable, which is the ship you are currently controlling. It has the most information about the ship.
    /// 2. The PlayerUnit, which is a view of other ships and will be visible when scanned (enemies) or transmited (friends).
    /// </remarks>
    /// <seealso cref="Units.PlayerUnit" />
    public class Controllable : INamedUnit
    {
        private readonly Galaxy galaxy;
        private readonly string name;

        private byte[] activeShipUpgrades;
        
        private double hull;
        private double hullMax;
        private double hullRepair;
        private double shields;
        private double shieldsMax;
        private double shieldsLoad;
        private double radius;
        private double gravity;
        private double energy;
        private double energyMax;
        private double energyCells;
        private double energyReactor;
        private double energyTransfer;
        private double ion;
        private double ionMax;
        private double ionCells;
        private double ionReactor;
        private double ionTransfer;
        private double thruster;
        private double thrusterForwardMax;
        private double thrusterBackwardMax;
        private double nozzle;
        private double nozzleMax;
        private double speedMax;
        private double turnrate;
        private double turnrateMax;
        private double cargoTungsten;
        private double cargoIron;
        private double cargoSilicon;
        private double cargoTritium;
        private double cargoMax;
        private double extractorMax;
        private double weaponSpeed;
        private ushort weaponTime;
        private double weaponLoad;
        private double weaponDamage;
        private double weaponAmmo;
        private double weaponAmmoMax;
        private double weaponAmmoProduction;
        private double direction;

        private Vector position;
        private Vector movement;

        private bool active;

        internal Controllable(Galaxy galaxy, byte id, PacketReader reader)
        {
            this.galaxy = galaxy;

            Id = id;

            name = reader.ReadString();

            ShipDesign = galaxy.ShipsDesigns[reader.ReadByte()];
            
            Debug.Assert(ShipDesign is not null, "Fail loading ship design.");

            radius = reader.ReadDouble();
            gravity = reader.ReadDouble();
            
            activeShipUpgrades = reader.ReadBytes(32);
            
            hullMax = reader.ReadDouble();
            hullRepair = reader.ReadDouble();
            shieldsMax = reader.ReadDouble();
            shieldsLoad = reader.ReadDouble();

            energyMax = reader.ReadDouble();
            energyCells = reader.ReadDouble();
            energyReactor = reader.ReadDouble();
            energyTransfer = reader.ReadDouble();
            ionMax = reader.ReadDouble();
            ionCells = reader.ReadDouble();
            ionReactor = reader.ReadDouble();
            ionTransfer = reader.ReadDouble();

            thrusterForwardMax = reader.ReadDouble();
            thrusterBackwardMax = reader.ReadDouble();
            nozzleMax = reader.ReadDouble();
            speedMax = reader.ReadDouble();
            cargoMax = reader.ReadDouble();
            
            extractorMax = reader.ReadDouble();
            weaponSpeed = reader.ReadDouble();
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.ReadDouble();
            weaponDamage = reader.ReadDouble();
            weaponAmmoMax = reader.ReadDouble();
            weaponAmmoProduction = reader.ReadDouble();

            position = Vector.Null;
            movement = Vector.Null;

            active = true;
        }

        internal void Update(PacketReader reader)
        {
            radius = reader.ReadDouble();
            gravity = reader.ReadDouble();
            
            activeShipUpgrades = reader.ReadBytes(32);
            
            hullMax = reader.ReadDouble();
            hullRepair = reader.ReadDouble();
            shieldsMax = reader.ReadDouble();
            shieldsLoad = reader.ReadDouble();

            energyMax = reader.ReadDouble();
            energyCells = reader.ReadDouble();
            energyReactor = reader.ReadDouble();
            energyTransfer = reader.ReadDouble();
            ionMax = reader.ReadDouble();
            ionCells = reader.ReadDouble();
            ionReactor = reader.ReadDouble();
            ionTransfer = reader.ReadDouble();

            thrusterForwardMax = reader.ReadDouble();
            thrusterBackwardMax = reader.ReadDouble();
            nozzleMax = reader.ReadDouble();
            speedMax = reader.ReadDouble();
            cargoMax = reader.ReadDouble();
            
            extractorMax = reader.ReadDouble();
            weaponSpeed = reader.ReadDouble();
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.ReadDouble();
            weaponDamage = reader.ReadDouble();
            weaponAmmoMax = reader.ReadDouble();
            weaponAmmoProduction = reader.ReadDouble();
        }

        internal void DynamicUpdate(PacketReader reader)
        {
            hull = reader.ReadDouble();
            shields = reader.ReadDouble();
            energy = reader.ReadDouble();
            ion = reader.ReadDouble();
            thruster = reader.ReadDouble();
            nozzle = reader.ReadDouble();
            turnrate = reader.ReadDouble();

            cargoTungsten = reader.ReadDouble();
            cargoIron = reader.ReadDouble();
            cargoSilicon = reader.ReadDouble();
            cargoTritium = reader.ReadDouble();
            weaponAmmo = reader.ReadDouble();
            position = new Vector(reader);
            movement = new Vector(reader);
            direction = reader.ReadDouble();
        }

        internal void Deactivate()
        {
            active = false;
        }

        /// <summary>
        /// The ship design of the controllable.
        /// Currently unimplemented.
        /// </summary>
        public readonly ShipDesign ShipDesign;

        /// <summary>
        /// The name of the controllable.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// A local unique identifier for the controllable. This is not the same as the player id.
        /// Used to differentiate between multiple controllables of the same player.
        /// </summary>
        public readonly byte Id;

        /// <summary>
        /// This flag indicates if the controllable is able to be interacted with.
        /// </summary>
        /// <remarks>
        /// If it is inactive and you try to interact with it, you will get a GameException.
        /// </remarks>
        public bool IsActive => active;

        /// <summary>
        /// This flag indicates if the controllable can receive commands other than continue and dispose.
        /// </summary>
        public bool IsAlive => hull > 0;

        /// <summary>
        /// The hull level of the controllable.
        /// If it reaches 0, the controllable is destroyed.
        /// The player can call continue to respawn the controllable.
        /// </summary>
        public double Hull => hull;

        /// <summary>
        /// The maximum hull level of the ship.
        /// </summary>
        /// <seealso cref="Hull" />
        public double HullMax => hullMax;

        /// <summary>
        /// The direction of the ship in degrees (NOT radians!).
        /// </summary>
        public double Direction => direction;

        /// <summary>
        /// The rate at which the hull is repaired. Consumes Iron and Energy.
        /// </summary>
        /// <remarks>
        /// Repairing is only possible while the ship is not using any other energy consuming functions.
        /// (e.g. Thruster, Weapons, Shields, etc.)
        /// </remarks>
        /// <seealso cref="Hull" />
        public double HullRepair => hullRepair;

        /// <summary>
        /// The current shield value of the ship.
        /// It will be depleted before the hull is damaged.
        /// </summary>
        /// <remarks>
        /// Depending on a value in ShipDesign you may need Ion to recharge the shields.
        /// </remarks>
        public double Shields => shields;

        /// <summary>
        /// The maximum shield value of the ship.
        /// </summary>
        /// <seealso cref="Shields" />
        public double ShieldsMax => shieldsMax;

        /// <summary>
        /// The rate at which the shields are recharged.
        /// </summary>
        /// <seealso cref="Shields" />
        public double ShieldsLoad => shieldsLoad;

        /// <summary>
        /// The radius of the ship. Also affects the hitbox of the ship.
        /// </summary>
        public double Radius => radius;

        /// <summary>
        /// The gravity of the ship. Affects the acceleration and deceleration of the ship.
        /// </summary>
        public double Gravity => gravity;

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
        /// This is a multiplier for the rate of energy gain from suns and stations.
        /// </summary>
        /// <seealso cref="Energy" />
        public double EnergyCells => energyCells;

        /// <summary>
        /// Used by stations to produce energy.
        /// </summary>
        /// <seealso cref="Energy" />
        public double EnergyReactor => energyReactor;

        /// <summary>
        /// The rate at which energy can be transferred to other ships.
        /// </summary>
        /// <remarks>
        /// All ships can transfer energy to other ships.
        /// When doing this, a corona will be created around the ship,
        /// which can be used by other ships to gain energy, even enemies.
        /// </remarks>
        /// <seealso cref="Energy" />
        public double EnergyTransfer => energyTransfer;

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
        /// This is a multiplier for the rate of ion gain from corona sections.
        /// </summary>
        public double IonCells => ionCells;

        /// <summary>
        /// Used by stations to produce ion.
        /// </summary>
        public double IonReactor => ionReactor;

        /// <summary>
        /// The rate at which ion can be transferred to other ships per tick.
        /// </summary>
        /// <remarks>
        /// All ships can transfer ion to other ships.
        /// When doing this, a corona will be created around the ship,
        /// which can be used by other ships to gain ion, even enemies.
        /// </remarks>
        public double IonTransfer => ionTransfer;

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
        /// The maximum forward thruster value of the ship.
        /// </summary>
        /// <seealso cref="Thruster" />
        public double ThrusterForwardMax => thrusterForwardMax;

        /// <summary>
        /// The maximum backward thruster value of the ship.
        /// </summary>
        /// <remarks>
        /// The negative value is usually limited to smaller value than the positive one.
        /// </remarks>
        /// <seealso cref="Thruster" />
        public double ThrusterBackwardMax => thrusterBackwardMax;
        
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
        /// The maximum nozzle value of the ship.
        /// As opposed to the thruster this is symmetrical
        /// </summary>
        /// <seealso cref="Nozzle" />
        public double NozzleMax => nozzleMax;

        /// <summary>
        /// The maximum speed of the ship.
        /// </summary>
        /// <remarks>
        /// Past this speed the velocity will be dampened by 10% per tick.
        /// You can still accelerate to go slightly faster, but at the cost of more energy.
        /// </remarks>
        /// <seealso cref="Movement" />
        public double SpeedMax => speedMax;

        /// <summary>
        /// The turnrate of the ship in degrees/tick.
        /// This is the current angular velocity of the ship.
        /// Can be positive or negative.
        /// </summary>
        public double Turnrate => turnrate;

        /// <summary>
        /// The maximum turnrate of the ship in degrees/tick.
        /// Past this value the turnrate will be dampened by 30% per tick.
        /// You can still accelerate to turn slightly faster, but at the cost of more energy.
        /// </summary>
        /// <seealso cref="Turnrate" />
        public double TurnrateMax => turnrateMax;

        /// <summary>
        /// This is used for upgrades of the ship.
        /// The cost of thes upgrades is defined in the map.
        /// </summary>
        public double CargoTungsten => cargoTungsten;

        /// <summary>
        /// This is used for ship repairs.
        /// The cost of thes repairs is defined ShipDesign and influenced by upgrades.
        /// </summary>
        public double CargoIron => cargoIron;

        /// <summary>
        /// This is used for ship upgrades.
        /// The cost of thes upgrades is defined in the map.
        /// </summary>
        public double CargoSilicon => cargoSilicon;

        /// <summary>
        /// This is used for ship upgrades.
        /// The cost of thes upgrades is defined in the map.
        /// </summary>
        public double CargoTritium => cargoTritium;

        /// <summary>
        /// The maximum cummulative cargo of the ship.
        /// </summary>
        /// <remarks>
        /// The materials factored in are Tungsten, Iron, Silicon and Tritium.
        /// </remarks>
        /// <seealso cref="CargoTungsten" />
        /// <seealso cref="CargoIron" />
        /// <seealso cref="CargoSilicon" />
        /// <seealso cref="CargoTritium" />
        public double CargoMax => cargoMax;

        /// <summary>
        /// The maximum amount of resources that can be extracted from resource sections per tick.
        /// </summary>
        /// <remarks>
        /// Resource sections are defined in the map.
        /// They are found around planets and moons.
        /// </remarks>
        public double ExtractorMax => extractorMax;

        /// <summary>
        /// The maximum speed of the weapon projectile.
        /// </summary>
        public double WeaponSpeedMax => weaponSpeed;

        /// <summary>
        /// The maximum fuse time of the weapon projectile.
        /// </summary>
        public ushort WeaponTimeMax => weaponTime;

        /// <summary>
        /// The damage radius of the weapon projectile.
        /// </summary>
        public double WeaponLoad => weaponLoad;

        /// <summary>
        /// The damage of the weapon projectile.
        /// </summary>
        public double WeaponDamage => weaponDamage;

        /// <summary>
        /// The current ammunition reserve of the weapon.
        /// </summary>
        public double WeaponAmmo => weaponAmmo;

        /// <summary>
        /// The maximum ammount of ammunition the ship can carry.
        /// </summary>
        public double WeaponAmmoMax => weaponAmmoMax;

        /// <summary>
        /// The rate at which the weapon ammunition is produced.
        /// </summary>
        public double WeaponAmmoProduction => weaponAmmoProduction;

        /// <summary>
        /// The position of the ship in the galaxy.
        /// These are absolute coordinates.
        /// </summary>
        public Vector Position => new Vector(position);

        /// <summary>
        /// The current velocity of the ship.
        /// </summary>
        public Vector Movement => new Vector(movement);
        
        /*/// <summary>
        /// Allows the ship to self destruct.
        /// </summary>
        /// <exception cref="GameException">
        /// Thrown when the ship is already dead.
        /// </exception>
        public async Task Kill()
        {
            if (hull == 0)
                throw new GameException(0x20);

            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x31;
            packet.Header.Id0 = Id;

            await session.SendWait(packet);
        }*/

        /// <summary>
        /// When the ship is dead or when entering a new galaxy, continue will spawn a new ship.
        /// </summary>
        /// <exception cref="GameException">
        /// Thrown when the ship is already alive.
        /// </exception>
        public async Task Continue()
        {
            if (hull > 0)
                throw new GameException(0x21);

            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x32;
            packet.Header.Id0 = Id;

            await session.SendWait(packet);
        }

        // public async Task Unregister()
        // {
        //     Session session = await galaxy.GetSession();
        //
        //     Packet packet = new Packet();
        //     packet.Header.Command = 0x33;
        //     packet.Header.Id0 = Id;
        //
        //     await session.SendWait(packet);
        // }

        /// <summary>
        /// Sets the thruster and nozzle of the ship at the same time. Please note, that you need to stay within the limits
        /// of your ships configuration. A positive thruster value will make your ship advance forward. A negative thruster
        /// value negative. Usually a ship is designed to be faster when flying forward.
        /// </summary>
        /// <param name="thruster">The thruster value or 0 if you want to disable the thruster.</param>
        /// <param name="nozzle">The nozzle value or 0, if you want to disable turning.</param>
        public async Task SetThrusterNozzle(double thruster, double nozzle)
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            if (!double.IsFinite(thruster) || !double.IsFinite(nozzle) || thruster < ThrusterBackwardMax * -1.05 ||
                thruster > ThrusterForwardMax * 1.05 || nozzle < NozzleMax * -1.05 || nozzle > NozzleMax * 1.05)
                throw new GameException(0x31);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x36;
            packet.Header.Id0 = Id;

            using (PacketWriter writer = packet.Write())
            {
                writer.Write(thruster);
                writer.Write(nozzle);
            }

            await session.SendWait(packet);
        }
        
        /// <summary>
        /// Sets the thruster of the ship. Please note, that you need to stay within the limits
        /// of your ships configuration. A positive thruster value will make your ship advance forward. A negative thruster
        /// value negative. Usually a ship is designed to be faster when flying forward.
        /// </summary>
        /// <param name="thruster">The thruster value or 0 if you want to disable the thruster.</param>
        public async Task SetThruster(double thruster)
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            if (!double.IsFinite(thruster) || thruster < ThrusterBackwardMax * -1.05 ||
                thruster > ThrusterForwardMax * 1.05)
                throw new GameException(0x31);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x34;
            packet.Header.Id0 = Id;

            using (PacketWriter writer = packet.Write())
                writer.Write(thruster);

            await session.SendWait(packet);
        }
        
        /// <summary>
        /// Sets the nozzle of the ship. Please note, that you need to stay within the limits
        /// of your ships configuration. A positive nozzle value will increase the Angle, a negative
        /// decrease.
        /// </summary>
        /// <param name="nozzle">The nozzle value or 0, if you want to disable turning.</param>
        public async Task SetNozzle(double nozzle)
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            if (!double.IsFinite(nozzle) || nozzle < NozzleMax * -1.05 || nozzle > NozzleMax * 1.05)
                throw new GameException(0x31);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x35;
            packet.Header.Id0 = Id;

            using (PacketWriter writer = packet.Write())
                writer.Write(nozzle);

            await session.SendWait(packet);
        }

        public async Task EnableScanner()
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x3A;
            packet.Header.Id0 = Id;
            packet.Header.Param0 = 1; // state enabled

            await session.SendWait(packet);
        }

        public async Task DisableScanner()
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x3A;
            packet.Header.Id0 = Id;
            packet.Header.Param0 = 0; // state disabled

            await session.SendWait(packet);
        }

        public async Task SetScanner(double range, double width, double direction)
        {
            if (!active)
                throw new GameException(0x22);
                
            if (hull == 0)
                throw new GameException(0x20);
            
            if (!double.IsFinite(direction) || direction < -360.0 || direction > 720.0)
                throw new GameException(0x31);

            direction = (direction + 720.0) % 360.0;
            
            if (!double.IsFinite(range) || range < 60.0 || range > ShipDesign.Config.ScannerRange * 1.05)
                throw new GameException(0x31);

            if (!double.IsFinite(width) || width < 10.0 || width > ShipDesign.Config.ScannerWidth * 1.05)
                throw new GameException(0x31);
            
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x3B;
            packet.Header.Id0 = Id;

            using (PacketWriter writer = packet.Write())
            {
                writer.Write(direction);
                writer.Write(range);
                writer.Write(width);
            }
            
            await session.SendWait(packet);
        }
    }
}
