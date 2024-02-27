﻿using System.Diagnostics;
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
        private Galaxy galaxy;

        /// <summary>
        /// A local unique identifier for the controllable. This is not the same as the player id.
        /// Used to differantiate between multiple controllables of the same player.
        /// </summary>
        public readonly byte Id;

        /// <summary>
        /// The name of the controllable.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The player id of the controllable. 
        /// This is not the same as the controllable id.
        /// No two players within the same galaxy have the same id.
        /// </summary>
        /// <remarks>
        /// Ids will be reused when a player leaves the galaxy and a new player joins.
        /// </remarks>
        private readonly byte playerId;

        private readonly string name;

        /// <summary>
        /// The ship design of the controllable.
        /// Currently unimplemented.
        /// </summary>
        public readonly ShipDesign ShipDesign;

        private byte[] activeShipUpgrades;
        
        
        private double hull;
        private double hullMax;
        private double hullRepair;
        private double shields;
        private double shieldsMax;
        private double shieldsLoad;
        private double size;
        private double weight;
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
        private double thrusterMaxForward;
        private double thrusterMaxBackward;
        private double nozzle;
        private double nozzleMax;
        private double speedMax;
        private double turnrate;
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

        /// <summary>
        /// You will 
        /// </summary>
        public bool IsActive => active;

        /// <summary>
        /// this 
        ///
        public bool IsAlive => hull > 0;

        internal Controllable(Galaxy galaxy, byte id, PacketReader reader)
        {
            this.galaxy = galaxy;

            Id = id;

            name = reader.ReadString();

            ShipDesign = galaxy.ShipsDesigns[reader.ReadByte()];
            
            Debug.Assert(ShipDesign is not null, "Fail loading ship design.");

            size = reader.ReadDouble();
            weight = reader.ReadDouble();
            
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

            thrusterMaxForward = reader.ReadDouble();
            thrusterMaxBackward = reader.ReadDouble();
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
            size = reader.ReadDouble();
            weight = reader.ReadDouble();
            
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

            thrusterMaxForward = reader.ReadDouble();
            thrusterMaxBackward = reader.ReadDouble();
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

        public double Hull => hull;
        public double HullMax => hullMax;
        public double Direction => direction;
        public double HullRepair => hullRepair;
        public double Shields => shields;
        public double ShieldsMax => shieldsMax;
        public double ShieldsLoad => shieldsLoad;
        public double Size => size;
        public double Weight => weight;
        public double Energy => energy;
        public double EnergyMax => energyMax;
        public double EnergyCells => energyCells;
        public double EnergyReactor => energyReactor;
        public double EnergyTransfer => energyTransfer;
        public double Ion => ion;
        public double IonMax => ionMax;
        public double IonCells => ionCells;
        public double IonReactor => ionReactor;
        public double IonTransfer => ionTransfer;
        public double Thruster => thruster;
        public double ThrusterMaxForward => thrusterMaxForward;
        public double ThrusterMaxBackward => thrusterMaxBackward;
        public double Nozzle => nozzle;
        public double NozzleMax => nozzleMax;
        public double SpeedMax => speedMax;
        public double Turnrate => turnrate;
        public double CargoTungsten => cargoTungsten;
        public double CargoIron => cargoIron;
        public double CargoSilicon => cargoSilicon;
        public double CargoTritium => cargoTritium;
        public double CargoMax => cargoMax;
        public double ExtractorMax => extractorMax;
        public double WeaponSpeed => weaponSpeed;
        public ushort WeaponTime => weaponTime;
        public double WeaponLoad => weaponLoad;
        public double WeaponDamage => weaponDamage;
        public double WeaponAmmo => weaponAmmo;
        public double WeaponAmmoMax => weaponAmmoMax;
        public double WeaponAmmoProduction => weaponAmmoProduction;
        public Vector Position => new Vector(position);
        public Vector Movement => new Vector(movement);

        public bool Alive => hull > 0;
        
        public async Task Kill()
        {
            if (hull == 0)
                throw new GameException(0x20);

            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x31;
            packet.Header.Id0 = Id;

            await session.SendWait(packet);
        }

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
            
            if (!double.IsFinite(thruster) || !double.IsFinite(nozzle) || thruster < ThrusterMaxBackward * -1.05 ||
                thruster > ThrusterMaxForward * 1.05 || nozzle < NozzleMax * -1.05 || nozzle > NozzleMax * 1.05)
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
            
            if (!double.IsFinite(thruster) || thruster < ThrusterMaxBackward * -1.05 ||
                thruster > ThrusterMaxForward * 1.05)
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
    }
}
