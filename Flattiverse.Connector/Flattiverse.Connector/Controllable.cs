using System.Diagnostics;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Flattiverse.Connector
{
    public class Controllable : INamedUnit
    {
        private Galaxy galaxy;

        public readonly byte Id;

        public string Name => name;

        private readonly byte playerId;

        private readonly string name;

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

        public bool IsActive => active;

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

            active = true;
        }

        internal void Update(PacketReader reader)
        {
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
            weaponAmmoMax = reader.ReadUInt16();
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
        public Vector Position => position;
        public Vector Movement => movement;
        
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

        public async Task Unregister()
        {
            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x33;
            packet.Header.Id0 = Id;

            await session.SendWait(packet);
        }
    }
}
