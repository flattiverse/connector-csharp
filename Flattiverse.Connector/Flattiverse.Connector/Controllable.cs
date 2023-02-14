using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System.Text.Json;

namespace Flattiverse.Connector
{
    // TOG: Schöne, ausführliche Kommentare schrieben und auch erwähnen welche Exceptions passieren können. Zudem noch remarks verwenden und erklären, was Leute bei den entsprechenden Kommandos zu erwarten haben.
    //      Diese Zeile stehen lassen, bis Sonntag Abend.
    /// <summary>
    /// A controllable that you control.
    /// </summary>
    public class Controllable
    {
        /// <summary>
        /// The universegroup your controllable is in.
        /// </summary>
        public readonly UniverseGroup Group;
        /// <summary>
        /// The name of your controllable.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The ID of your controllable.
        /// </summary>
        public readonly int ID;

        private double radius;
        private Vector position;
        private Vector movement;
        private double direction;
        private Team team;
        private double gravity;
        private double energyOutput;
        private double turnRate;
        private double requestedScanDirection;
        private double requestedScanWidth;
        private double requestedScanRange;
        private double scanDirection;
        private double scanWidth;
        private double scanRange;

        private PlayerUnitRegularSystem hull;
        private PlayerUnitRegularSystem cellsEnergy;
        private PlayerUnitRegularSystem batteryEnergy;
        private PlayerUnitEnergyConsumingSystem thruster;
        private PlayerUnitEnergyConsumingSystem nozzle;
        private PlayerUnitScannerSystem scanner;
        private PlayerUnitArmorSystem? armor;
        private PlayerUnitRegularSystem? shield;
        private PlayerUnitEnergyConsumingSystem? analyzer;
        private PlayerUnitRegularSystem? cellsParticles;
        private PlayerUnitRegularSystem? batteryParticles;
        private PlayerUnitRegularSystem? weaponLauncher;
        private PlayerUnitRegularSystem? weaponPayloadDamage;
        private PlayerUnitRegularSystem? weaponPayloadRadius;
        private PlayerUnitRegularSystem? weaponFactory;
        private PlayerUnitRegularSystem? weaponStorage;
        private PlayerUnitRegularSystem? cargoIron;
        private PlayerUnitRegularSystem? cargoCarbon;
        private PlayerUnitRegularSystem? cargoSilicon;
        private PlayerUnitRegularSystem? cargoPlatinum;
        private PlayerUnitRegularSystem? cargoGold;
        private PlayerUnitRegularSystem? cargoSpecial;
        private PlayerUnitEnergyConsumingSystem? extractorIron;
        private PlayerUnitEnergyConsumingSystem? extractorCarbon;
        private PlayerUnitEnergyConsumingSystem? extractorSilicon;
        private PlayerUnitEnergyConsumingSystem? extractorPlatinum;
        private PlayerUnitEnergyConsumingSystem? extractorGold;

        private bool active;

        /// <summary>
        /// The radius of your controllable.
        /// </summary>
        public double Radius => radius;

        /// <summary>
        /// The position of your controllable.
        /// </summary>
        public Vector Position => position;

        /// <summary>
        /// The movement of your controllable.
        /// </summary>
        public Vector Movement => movement;

        /// <summary>
        /// The direction of your controllable.
        /// </summary>
        public double Direction => direction;

        /// <summary>
        /// If you have joined a team, the team of your controllable.
        /// </summary>
        public Team Team => team;

        /// <summary>
        /// The gravity that your controllable is exercising on other mobile units.
        /// </summary>
        public double Gravity => gravity;

        /// <summary>
        /// The current energy output of your controllable.
        /// </summary>
        public double EnergyOutput => energyOutput;

        /// <summary>
        /// Whether your controllable is still alive.
        /// </summary>
        public bool IsAlive => hull.Value > 0.0;

        /// <summary>
        /// The rate at which your controllable is turning.
        /// </summary>
        public double TurnRate => turnRate;

        /// <summary>
        /// The hull of your controllable, keeping you away from the cold void of space.
        /// </summary>
        public PlayerUnitRegularSystem Hull => hull;

        /// <summary>
        /// The energy cell of your controllable, used for recharging your energy levels.
        /// </summary>
        public PlayerUnitRegularSystem CellsEnergy => cellsEnergy;

        /// <summary>
        /// The energy battery of your controllable, used for storing energy.
        /// </summary>
        public PlayerUnitRegularSystem BatteryEnergy => batteryEnergy;

        /// <summary>
        /// The thruster of your controllable, used to propel your controllabe through the universe.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem Thruster => thruster;

        /// <summary>
        /// The nozzle of your controllable, used to make you spin all around.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem Nozzle => nozzle;

        /// <summary>
        /// The scanner of your controllable, used to detect objects in the vicinity.
        /// </summary>
        public PlayerUnitScannerSystem Scanner => scanner;

        /// <summary>
        /// The armor of your controllable, used to reduce damage from malicious influences.
        /// </summary>
        public PlayerUnitArmorSystem? Armor => armor;

        /// <summary>
        /// The shield of your controllable, used to avoid damage altogether.
        /// </summary>
        public PlayerUnitRegularSystem? Shield => shield;

        /// <summary>
        /// The analyzer of your controllable, used to identify objects.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? Analyzer => analyzer;

        /// <summary>
        /// The particle cells of your controllable, used for recharging your particle levels.
        /// </summary>
        public PlayerUnitRegularSystem? CellsParticles => cellsParticles;

        /// <summary>
        /// The particle battery of your controllable, used for storing particles.
        /// </summary>
        public PlayerUnitRegularSystem? BatteryParticles => batteryParticles;

        /// <summary>
        /// The weapon launcher of your controllable, used to do the pew pew.
        /// </summary>
        public PlayerUnitRegularSystem? WeaponLauncher => weaponLauncher;

        /// <summary>
        /// The damage of your controllable's weapons.
        /// </summary>
        public PlayerUnitRegularSystem? WeaponPayloadDamage => weaponPayloadDamage;

        /// <summary>
        /// The radius of your controllable's weapons' explosions.
        /// </summary>
        public PlayerUnitRegularSystem? WeaponPayloadRadius => weaponPayloadRadius;

        /// <summary>
        /// The weapon factory of your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? WeaponFactory => weaponFactory;

        /// <summary>
        /// The storage capacity of your controllable for weapons.
        /// </summary>
        public PlayerUnitRegularSystem? WeaponStorage => weaponStorage;

        /// <summary>
        /// The storage capacity of iron in your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoIron => cargoIron;

        /// <summary>
        /// The storage capacity of carbon in your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoCarbon => cargoCarbon;

        /// <summary>
        /// The storage capacity of silicon in your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoSilicon => cargoSilicon;

        /// <summary>
        /// The storage capacity of platinum in your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoPlatinum => cargoPlatinum;

        /// <summary>
        /// The capacity for gold in your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoGold => cargoGold;

        /// <summary>
        /// The special storage capacity of your controllable.
        /// </summary>
        public PlayerUnitRegularSystem? CargoSpecial => cargoSpecial;

        /// <summary>
        /// The extraction capabilities of your controllable for iron.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorIron => extractorIron;

        /// <summary>
        /// The extraction capabilities of your controllable for carbon.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorCarbon => extractorCarbon;

        /// <summary>
        /// The extraction capabilities of your controllable for silicon.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorSilicon => extractorSilicon;

        /// <summary>
        /// The extraction capabilities of your controllable for platinum.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorPlatinum => extractorPlatinum;

        /// <summary>
        /// The extraction capabilities of your controllable for gold.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorGold => extractorGold;

        /// <summary>
        /// Indictaes, if the controllable is active (registered to the game) or not. If this value is false, you already triggered
        /// the deregistration of the controllable or the server forced the deregistration due to a tournament start, server shutdown
        /// or due to disconnection of the player.
        /// </summary>
        public bool Active => active;

        internal void update(UniverseGroup group, JsonElement element)
        {
            Utils.Traverse(element, out movement, "movement");
            Utils.Traverse(element, out position, "position");

            Utils.Traverse(element, out radius, "radius");
            Utils.Traverse(element, out gravity, "gravity");
            Utils.Traverse(element, out energyOutput, "energyOutput");

            Utils.Traverse(element, out direction, "direction");
            Utils.Traverse(element, out turnRate, "turnRate");
            Utils.Traverse(element, out requestedScanDirection, "requestedScanDirection");
            Utils.Traverse(element, out requestedScanWidth, "requestedScanWidth");
            Utils.Traverse(element, out requestedScanRange, "requestedScanRange");
            Utils.Traverse(element, out scanDirection, "scanDirection");
            Utils.Traverse(element, out scanWidth, "scanWidth");
            Utils.Traverse(element, out scanRange, "scanRange");

            Utils.Traverse(element, out JsonElement systems, "systems");
            foreach (JsonProperty system in systems.EnumerateObject())
            {
                if (!Enum.TryParse(system.Name, true, out PlayerUnitSystemKind kind))
                    group.connection.PushFailureEvent($"Couldn't parse kind in PlayerUnit for system {system.Name}.");

                switch (kind)
                {
                    case PlayerUnitSystemKind.Hull:
                        hull = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Shield:
                        shield = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Armor:
                        armor = new PlayerUnitArmorSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Thruster:
                        thruster = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Nozzle:
                        nozzle = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Scanner:
                        scanner = new PlayerUnitScannerSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Analyzer:
                        analyzer = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CellsEnergy:
                        cellsEnergy = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CellsParticles:
                        cellsParticles = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.BatteryEnergy:
                        batteryEnergy = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.BatteryParticles:
                        batteryParticles = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponLauncher:
                        weaponLauncher = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadDamage:
                        weaponPayloadDamage = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadRadius:
                        weaponPayloadRadius = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponFactory:
                        weaponFactory = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponStorage:
                        weaponStorage = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoIron:
                        cargoIron = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoCarbon:
                        cargoCarbon = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoSilicon:
                        cargoSilicon = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoPlatinum:
                        cargoPlatinum = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoGold:
                        cargoGold = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoSpecial:
                        cargoSpecial = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorIron:
                        extractorIron = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorCarbon:
                        extractorCarbon = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorSilicon:
                        extractorSilicon = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorPlatinum:
                        extractorPlatinum = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorGold:
                        extractorGold = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    default:
                        group.connection.PushFailureEvent($"PlayerUnitSystemKind {system.Name} is not implemented.");
                        break;
                }
            }
        }

        internal Controllable(UniverseGroup group, string name, int id/*, double radius, Vector position, Vector movement, double direction, Team? team, double gravity, double energyOutput, double turnRate*/)
        {
            Group = group;
            Name = name;
            ID = id;

            //Maluk: Uncomment if needed

            //this.radius = radius;
            //this.position = position;
            //this.movement = movement;
            //this.direction = direction;
            //this.team = team;
            //this.gravity = gravity;
            //this.energyOutput = energyOutput;
            //this.turnRate = turnRate;

            foreach (PlayerUnitSystemKind kind in Enum.GetValues(typeof(PlayerUnitSystemKind)))
                if (PlayerUnitSystem.TryGetStartSystem(group, kind, out PlayerUnitSystem? system))
                {
                    switch (kind)
                    {
                        case PlayerUnitSystemKind.Hull:
                            hull = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.Shield:
                            shield = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.Armor:
                            armor = (PlayerUnitArmorSystem)system;
                            break;
                        case PlayerUnitSystemKind.Thruster:
                            thruster = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.Nozzle:
                            nozzle = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.Scanner:
                            scanner = (PlayerUnitScannerSystem)system;
                            break;
                        case PlayerUnitSystemKind.Analyzer:
                            analyzer = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.CellsEnergy:
                            cellsEnergy = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CellsParticles:
                            cellsParticles = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.BatteryEnergy:
                            batteryEnergy = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.BatteryParticles:
                            batteryParticles = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponLauncher:
                            weaponLauncher = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadDamage:
                            weaponPayloadDamage = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadRadius:
                            weaponPayloadRadius = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponFactory:
                            weaponFactory = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponStorage:
                            weaponStorage = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoIron:
                            cargoIron = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoCarbon:
                            cargoCarbon = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoSilicon:
                            cargoSilicon = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoPlatinum:
                            cargoPlatinum = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoGold:
                            cargoGold = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.CargoSpecial:
                            cargoSpecial = (PlayerUnitRegularSystem)system;
                            break;
                        case PlayerUnitSystemKind.ExtractorIron:
                            extractorIron = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.ExtractorCarbon:
                            extractorCarbon = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.ExtractorSilicon:
                            extractorSilicon = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.ExtractorPlatinum:
                            extractorPlatinum = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.ExtractorGold:
                            extractorGold = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        default:
                            group.connection.PushFailureEvent($"PlayerUnitSystemKind {kind} is not implemented.");
                            break;
                    }
                }
        }

        internal void update()
        {
            hull.Value = 0.0;
        }

        internal void update(Universe universe, PlayerUnit playerUnit)
        {
            hull = playerUnit.Hull;
            cellsEnergy = playerUnit.CellsEnergy;
            batteryEnergy = playerUnit.BatteryEnergy;
            thruster = playerUnit.Thruster;
            nozzle = playerUnit.Nozzle;
            scanner = playerUnit.Scanner;
            armor = playerUnit.Armor;
            shield = playerUnit.Shield;
            analyzer = playerUnit.Analyzer;
            cellsParticles = playerUnit.CellsParticles;
            batteryParticles = playerUnit.BatteryParticles;
            weaponLauncher = playerUnit.WeaponLauncher;
            weaponPayloadDamage = playerUnit.WeaponPayloadDamage;
            weaponPayloadRadius = playerUnit.WeaponPayloadRadius;
            weaponFactory = playerUnit.WeaponFactory;
            weaponStorage = playerUnit.WeaponStorage;
            cargoIron = playerUnit.CargoIron;
            cargoCarbon = playerUnit.CargoCarbon;
            cargoSilicon = playerUnit.CargoSilicon;
            cargoPlatinum = playerUnit.CargoPlatinum;
            cargoGold = playerUnit.CargoGold;
            cargoSpecial = playerUnit.CargoSpecial;
            extractorIron = playerUnit.ExtractorIron;
            extractorCarbon = playerUnit.ExtractorCarbon;
            extractorSilicon = playerUnit.ExtractorSilicon;
            extractorPlatinum = playerUnit.ExtractorPlatinum;
            extractorGold = playerUnit.ExtractorGold;

            //Evtl. muss es so gemacht werden...

            //if (playerUnit.Hull is not null) hull.Update(playerUnit.Hull);
            //if (playerUnit.CellsEnergy is not null) cellsEnergy.Update(playerUnit.CellsEnergy);
            //if (playerUnit.BatteryEnergy is not null) batteryEnergy.Update(playerUnit.BatteryEnergy);
            //if (playerUnit.Thruster is not null) thruster.Update(playerUnit.Thruster);
            //if (playerUnit.Nozzle is not null) nozzle.Update(playerUnit.Nozzle);
            //if (playerUnit.Scanner is not null) scanner.Update(playerUnit.Scanner);

            //if (playerUnit.Armor is not null)
            //    if (armor is not null)
            //        armor.Update(playerUnit.Armor);
            //    else
            //        armor = new PlayerUnitArmorSystem(playerUnit.Armor);

            //etc...
        }

        public async Task Continue()
        {
            if (hull.Value > 0.0)
                throw new GameException(0x20);

            using (Query query = Group.connection.Query("controllableContinue"))
            {
                query.Write("controllable", ID);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        public async Task Kill()
        {
            if (hull.Value <= 0.0)
                throw new GameException(0x22);

            using (Query query = Group.connection.Query("controllableKill"))
            {
                query.Write("controllable", ID);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        public async Task SetNozzle(double value)
        {
            if (hull.Value <= 0.0)
                throw new GameException(0x22);

            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new GameException(0xB6);

            if (value < Nozzle.MaxValue * -1.05 || value > Nozzle.MaxValue * 1.05)
                throw new GameException(0x23);

            if (value < -Nozzle.MaxValue)
                value = -Nozzle.MaxValue;

            if (value > Nozzle.MaxValue)
                value = Nozzle.MaxValue;

            using (Query query = Group.connection.Query("controllableNozzle"))
            {
                query.Write("controllable", ID);

                query.Write("nozzle", value);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        public async Task SetThruster(double value)
        {
            if (hull.Value <= 0.0)
                throw new GameException(0x22);

            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new GameException(0xB6);

            if (value < 0.0 || value > Thruster.MaxValue * 1.05)
                throw new GameException(0x23);

            if (value > Thruster.MaxValue)
                value = Thruster.MaxValue;

            using (Query query = Group.connection.Query("controllableThruster"))
            {
                query.Write("controllable", ID);

                query.Write("thrust", value);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        public async Task SetScanner(double direction, double length, double width, bool enabled = true)
        {
            if (hull.Value <= 0.0)
                throw new GameException(0x22);

            if (double.IsNaN(direction) || double.IsInfinity(direction) || double.IsNaN(length) || double.IsInfinity(length) || double.IsNaN(width) || double.IsInfinity(width))
                throw new GameException(0xB6);

            direction = (direction + 3600.0) % 360.0;

            if (direction < 0 || length > 360.1 || length < 59.9 || length > scanner.MaxRange * 1.05 || width < 19.9 || width > scanner.MaxAngle * 1.05)
                throw new GameException(0x23);

            if (direction > 360.0)
                direction = 360.0;

            if (length < 60.0)
                length = 60.0;

            if (length > scanner.MaxRange)
                length = scanner.MaxRange;

            if (width < 20.0)
                width = 20.0;

            if (width > scanner.MaxAngle)
                width = scanner.MaxAngle;

            using (Query query = Group.connection.Query("controllableScanner"))
            {
                query.Write("controllable", ID);

                query.Write("direction", direction);
                query.Write("length", length);
                query.Write("width", width);
                query.Write("enabled", enabled);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }

        internal void updateUnregistered()
        {
            active = false;

            hull.Value = 0;
        }
    }
}
