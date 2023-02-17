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
        private bool scanActivated;

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
        private PlayerUnitEnergyConsumingSystem? weaponAmmunition;
        private PlayerUnitEnergyConsumingSystem? weaponLauncher;
        private PlayerUnitEnergyConsumingSystem? weaponPayloadDamage;
        private PlayerUnitEnergyConsumingSystem? weaponPayloadRadius;
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
        /// The requested scan direction.
        /// </summary>
        public double RequestedScanDirection => requestedScanDirection;

        /// <summary>
        /// The requested scan width.
        /// </summary>
        public double RequestedScanWidth => requestedScanWidth;

        /// <summary>
        /// The requested scan range.
        /// </summary>
        public double RequestedScanRange => requestedScanRange;

        /// <summary>
        /// The current scan direction.
        /// </summary>
        public double ScanDirection => scanDirection;

        /// <summary>
        /// The current scan width.
        /// </summary>
        public double ScanWidth => scanWidth;

        /// <summary>
        /// The current scan range.
        /// </summary>
        public double ScanRange => scanRange;

        /// <summary>
        /// Whether the scan is activated.
        /// </summary>
        public bool ScanActivated => scanActivated;

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
        /// The shot lifetime of your controllable's weapons.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? WeaponAmmunition => weaponAmmunition;

        /// <summary>
        /// The shot speed of your controllable's weapons.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? WeaponLauncher => weaponLauncher;

        /// <summary>
        /// The damage of your controllable's weapons.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? WeaponPayloadDamage => weaponPayloadDamage;

        /// <summary>
        /// The radius of your controllable's weapons' explosions.
        /// </summary>
        public PlayerUnitEnergyConsumingSystem? WeaponPayloadRadius => weaponPayloadRadius;

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
            if (Utils.Traverse(element, out int teamID, "team") && teamID >= 0 && teamID < 16)
                team = group.teamsId[teamID];

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
            Utils.Traverse(element, out scanActivated, "scanActivated");

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
                    case PlayerUnitSystemKind.WeaponAmmunition:
                        weaponAmmunition = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponLauncher:
                        weaponLauncher = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadDamage:
                        weaponPayloadDamage = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadRadius:
                        weaponPayloadRadius = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
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

        internal Controllable(UniverseGroup group, string name, int id)
        {
            Group = group;
            Name = name;
            ID = id;
            team = group.Player.Team;

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
                        case PlayerUnitSystemKind.WeaponAmmunition:
                            weaponAmmunition = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponLauncher:
                            weaponLauncher = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadDamage:
                            weaponPayloadDamage = (PlayerUnitEnergyConsumingSystem)system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadRadius:
                            weaponPayloadRadius = (PlayerUnitEnergyConsumingSystem)system;
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

        /// <summary>
        /// <para>Shoots a shot. It can only handle one shot per tick per ship and has a buffer of one additional shot. Units generally can shoot only one shot per tick, so
        /// specifying 3 Shots in one tick will result in an error at the 3rd shot requested. The shot will be generated with the next tick. The server tries to anticipate,
        /// if you are able to shoot. However, this may not be possible. So the call to this method may be successfull but the shot may not be created, if you are out of
        /// energy, etc. Please observe Events like <c>ResourceDepletedEvent</c> to determine such situations.</para>
        /// <list type="number"><listheader><term>The process.</term><description>The process is as described in the following steps.</description></listheader>
        /// <item><term>You call <c>.Shoot()</c> with time 1</term></item><item><term>The shot will be placed when the next tick is processed with time 1.</term></item>
        /// <item><term>The next tick will change time to 0.</term></item><item><term>The next tick will delete the shot and create the explosion.</term></item>
        /// <item><term>In the next tick the explosion is removed and deals the damage.</term></item></list>
        /// <para>This described process has been implemented for "a better gameflow". Please also note the corresponding minimum and maximum values for each of the parameters.</para>
        /// </summary>
        /// <param name="direction">The direction in which you want to shoot. Calculated energy costs due to what the corresponding systems say are true for a exact forward shot. Shooting backwards the shot will cost 7 times the energy. Shooting 90 degrees sideways will cost 4 times the energy and so on. The length of this vector should be longer than 0.1.</param>
        /// <param name="load">The radius of the resulting explosion. Minimum value is 2.5.</param>
        /// <param name="damage">The damage dealt of the explosion. Minimum value is 0.001.</param>
        /// <param name="time">The amount of ticks the shot will live, before exploding. Minimum value is 0.</param>
        /// <exception cref="GameException">Wrong parameters may result in an exception, also if you don't have enough shots to shoot or if oyu don't have all of the required systems.</exception>
        /// <remarks>Please query the status of your weapon systems for the corresponding maximums (<c>MaxValue</c>) and energy costs: <c>direction.Length</c> is <c>WeaponLauncher</c>, <c>load</c> is <c>WeaponPayloadRadius</c>, <c>damage</c> is <c>WeaponPayloadDamage</c> and <c>time</c> is <c>WeaponAmmunition</c>.</remarks>
        public async Task Shoot(Vector direction, double load, double damage, int time)
        {
            if (hull.Value <= 0.0)
                throw new GameException(0x22);

            if (direction is null)
                throw new GameException(0xB0);

            if (direction.IsDamaged || double.IsNaN(load) || double.IsInfinity(load) || double.IsNaN(damage) || double.IsInfinity(damage))
                throw new GameException(0xB6);

            if (WeaponAmmunition is null || WeaponFactory is null || WeaponLauncher is null || WeaponPayloadDamage is null || WeaponPayloadRadius is null || WeaponStorage is null)
                throw new GameException(0x24);

            if (direction < 0.075 || direction > WeaponLauncher.MaxValue * 1.05 || load < 2.25 || load > WeaponPayloadRadius.MaxValue * 1.05 || damage < 0.00075 || damage > WeaponPayloadDamage.MaxValue * 1.05 || time < 0 || time > (int)(WeaponAmmunition.MaxValue + 0.5))
                throw new GameException(0x23);

            if (direction < 0.1)
                direction.Length = 0.1;
            else if (direction > WeaponLauncher.MaxValue)
                direction.Length = WeaponLauncher.MaxValue;

            if (load < 2.5)
                load = 2.5;

            if (load > WeaponPayloadRadius.MaxValue)
                load = WeaponPayloadRadius.MaxValue;

            if (damage < 0.001)
                damage = 0.001;

            if (damage > WeaponPayloadDamage.MaxValue)
                damage = WeaponPayloadDamage.MaxValue;

            using (Query query = Group.connection.Query("controllableShoot"))
            {
                query.Write("controllable", ID);

                query.Write("direction", direction);
                query.Write("load", load);
                query.Write("damage", damage);
                query.Write("time", time);

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