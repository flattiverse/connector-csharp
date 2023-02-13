using Flattiverse.Connector.Accounts;
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
        private Team? team;
        private double gravity;
        private double energyOutput;
        private bool isAlive;
        private double turnRate;
        private double requestedScanDirection;
        private double requestedScanWidth;
        private double requestedScanRange;
        private double scanDirection;
        private double scanWidth;
        private double scanRange;

        private PlayerUnitSystem hull;
        private PlayerUnitSystem cellsEnergy;
        private PlayerUnitSystem batteryEnergy;
        private PlayerUnitEnergyConsumingSystem thruster;
        private PlayerUnitEnergyConsumingSystem nozzle;
        private PlayerUnitScannerSystem scanner;
        private PlayerUnitArmorSystem? armor;
        private PlayerUnitSystem? shield;
        private PlayerUnitEnergyConsumingSystem? analyzer;
        private PlayerUnitSystem? cellsParticles;
        private PlayerUnitSystem? batteryParticles;
        private PlayerUnitSystem? weaponLauncher;
        private PlayerUnitSystem? weaponPayloadDamage;
        private PlayerUnitSystem? weaponPayloadRadius;
        private PlayerUnitSystem? weaponFactory;
        private PlayerUnitSystem? weaponStorage;
        private PlayerUnitSystem? cargoIron;
        private PlayerUnitSystem? cargoCarbon;
        private PlayerUnitSystem? cargoSilicon;
        private PlayerUnitSystem? cargoPlatinum;
        private PlayerUnitSystem? cargoGold;
        private PlayerUnitSystem? cargoSpecial;
        private PlayerUnitEnergyConsumingSystem? extractorIron;
        private PlayerUnitEnergyConsumingSystem? extractorCarbon;
        private PlayerUnitEnergyConsumingSystem? extractorSilicon;
        private PlayerUnitEnergyConsumingSystem? extractorPlatinum;
        private PlayerUnitEnergyConsumingSystem? extractorGold;

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
        public Team? Team => team;

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
        public bool IsAlive => isAlive;

        /// <summary>
        /// The rate at which your controllable is turning.
        /// </summary>
        public double TurnRate => turnRate;

        /// <summary>
        /// The hull of your controllable, keeping you away from the cold void of space.
        /// </summary>
        public PlayerUnitSystem Hull => hull;

        /// <summary>
        /// The energy cell of your controllable, used for recharging your energy levels.
        /// </summary>
        public PlayerUnitSystem CellsEnergy => cellsEnergy;

        /// <summary>
        /// The energy battery your controllable, used for storing energy.
        /// </summary>
        public PlayerUnitSystem BatteryEnergy => batteryEnergy;

        /// <summary>
        /// The thruster of your controllable, used to propel your controllabe through the universe.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem Thruster => thruster;

        /// <summary>
        /// The nozzle of your controllable, used make you spin all around.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem Nozzle => nozzle;

        /// <summary>
        /// The scanner of your controllable, used to detect objects in the vincinity.
        /// </summary>
        public PlayerUnitScannerSystem Scanner => scanner;

        /// <summary>
        /// The armor of your controllable, used to reduce damage from malicious influences.
        /// </summary>
        public PlayerUnitArmorSystem? Armor => armor;

        /// <summary>
        /// The shield of your controllable, used to avoid damage alltogether.
        /// </summary>
        public PlayerUnitSystem? Shield => shield;

        /// <summary>
        /// The analayzer of your controllable, used to identify objects.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? Analyzer => analyzer;

        /// <summary>
        /// The particle cells, used for recharging your particle levels.
        /// </summary>
        public PlayerUnitSystem? CellsParticles => cellsParticles;

        /// <summary>
        /// The particle battery, used for storing particles.
        /// </summary>
        public PlayerUnitSystem? BatteryParticles => batteryParticles;

        /// <summary>
        /// The weapon launcher.
        /// </summary>
        public PlayerUnitSystem? WeaponLauncher => weaponLauncher;

        /// <summary>
        /// The damage of your controllable's weapons.
        /// </summary>
        public PlayerUnitSystem? WeaponPayloadDamage => weaponPayloadDamage;

        /// <summary>
        /// The radius of your controllable's weapons' explosions.
        /// </summary>
        public PlayerUnitSystem? WeaponPayloadRadius => weaponPayloadRadius;

        /// <summary>
        /// The weapon factory of your controllable.
        /// </summary>
        public PlayerUnitSystem? WeaponFactory => weaponFactory;

        /// <summary>
        /// The storage ability of your controllable for weapons.
        /// </summary>
        public PlayerUnitSystem? WeaponStorage => weaponStorage;

        /// <summary>
        /// The capacity for iron in your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoIron => cargoIron;

        /// <summary>
        /// The capacity for carbon in your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoCarbon => cargoCarbon;

        /// <summary>
        /// The capacity for silicon in your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoSilicon => cargoSilicon;

        /// <summary>
        /// The capacity for platinum in your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoPlatinum => cargoPlatinum;

        /// <summary>
        /// The capacity for gold in your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoGold => cargoGold;

        /// <summary>
        /// The special storage capability of your controllable.
        /// </summary>
        public PlayerUnitSystem? CargoSpecial => cargoSpecial;

        /// <summary>
        /// The extracting capabilities of your controllable for iron.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorIron => extractorIron;

        /// <summary>
        /// The extracting capabilities of your controllable for carbon.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorCarbon => extractorCarbon;

        /// <summary>
        /// The extracting capabilities of your controllable for silicon.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorSilicon => extractorSilicon;

        /// <summary>
        /// The extracting capabilities of your controllable for platinum.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorPlatinum => extractorPlatinum;

        /// <summary>
        /// The extracting capabilities of your controllable for gold.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? ExtractorGold => extractorGold;

        internal void update(UniverseGroup group, JsonElement element)
        {
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
                            hull = system;
                            break;
                        case PlayerUnitSystemKind.Shield:
                            shield = system;
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
                            cellsEnergy = system;
                            break;
                        case PlayerUnitSystemKind.CellsParticles:
                            cellsParticles = system;
                            break;
                        case PlayerUnitSystemKind.BatteryEnergy:
                            batteryEnergy = system;
                            break;
                        case PlayerUnitSystemKind.BatteryParticles:
                            batteryParticles = system;
                            break;
                        case PlayerUnitSystemKind.WeaponLauncher:
                            weaponLauncher = system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadDamage:
                            weaponPayloadDamage = system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadRadius:
                            weaponPayloadRadius = system;
                            break;
                        case PlayerUnitSystemKind.WeaponFactory:
                            weaponFactory = system;
                            break;
                        case PlayerUnitSystemKind.WeaponStorage:
                            weaponStorage = system;
                            break;
                        case PlayerUnitSystemKind.CargoIron:
                            cargoIron = system;
                            break;
                        case PlayerUnitSystemKind.CargoCarbon:
                            cargoCarbon = system;
                            break;
                        case PlayerUnitSystemKind.CargoSilicon:
                            cargoSilicon = system;
                            break;
                        case PlayerUnitSystemKind.CargoPlatinum:
                            cargoPlatinum = system;
                            break;
                        case PlayerUnitSystemKind.CargoGold:
                            cargoGold = system;
                            break;
                        case PlayerUnitSystemKind.CargoSpecial:
                            cargoSpecial = system;
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
    }
}
