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

            size = reader.Read3U(1000);
            weight = reader.Read2S(10000);
            
            activeShipUpgrades = reader.ReadBytes(32);
            
            hullMax = reader.Read3U(10000);
            hullRepair = reader.Read3U(10000);
            shieldsMax = reader.Read3U(10000);
            shieldsLoad = reader.Read3U(10000);

            active = true;
        }

        internal void Update(PacketReader reader)
        {
            energyMax = reader.Read4U(1000);
            energyCells = reader.Read4U(1000);
            energyReactor = reader.Read4U(1000);
            energyTransfer = reader.Read4U(1000);
            ionMax = reader.Read4U(1000);
            ionCells = reader.Read4U(1000);
            ionReactor = reader.Read4U(1000);
            ionTransfer = reader.Read4U(1000);
            thrusterMaxForward = reader.Read2U(10000);
            thrusterMaxBackward = reader.Read2U(10000);
            nozzleMax = reader.Read2S(100);
            speedMax = reader.Read2U(1000);
            cargoMax = reader.Read4U(1000);
            extractorMax = reader.Read4U(1000);
            weaponSpeed = reader.Read2U(1000);
            weaponTime = reader.ReadUInt16();
            weaponLoad = reader.Read3U(1000);
            weaponDamage = reader.Read3U(10000);
            weaponAmmoMax = reader.ReadUInt16();
            weaponAmmoProduction = reader.Read4U(1000);
        }

        internal void DynamicUpdate(PacketReader reader)
        {
            hull = reader.Read3U(10000);
            shields = reader.Read3U(10000);
            energy = reader.Read4U(1000);
            ion = reader.Read4U(1000);
            thruster = reader.Read2S(10000);
            nozzle = reader.Read2S(100);
            turnrate = reader.Read2S(100);

            cargoTungsten = reader.Read4U(1000);
            cargoIron = reader.Read4U(1000);
            cargoSilicon = reader.Read4U(1000);
            cargoTritium = reader.Read4U(1000);
            weaponAmmo = reader.Read4U(1000);
            position = new Vector(reader);
            movement = new Vector(reader);
            direction = reader.Read2U(100);
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
                throw new GameException(0xF5);

            Session session = await galaxy.GetSession();

            Packet packet = new Packet();
            packet.Header.Command = 0x31;
            packet.Header.Id0 = Id;

            await session.SendWait(packet);
        }

        public async Task Continue()
        {
            if (hull > 0)
                throw new GameException(0xF6);

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
