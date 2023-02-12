﻿using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector
{
    // TOG: Schöne, ausführliche Kommentare schrieben und auch erwähnen welche Exceptions passieren können. Zudem noch remarks verwenden und erklären,w as Leute bei den entsprechenden Kommandos zu erwarten haben.
    //      Diese Zeile stehen lassen, bis Sonntag Abend.
    public class Controllable
    {
        public readonly UniverseGroup Group;
        public readonly string Name;
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

        public double Radius => radius;

        public Vector Position => position;

        public Vector Movement => movement;

        public double Direction => direction;

        public Team? Team => team;

        public double Gravity => gravity;

        public double EnergyOutput => energyOutput;

        public bool IsAlive => isAlive;

        public double TurnRate => turnRate;

        public PlayerUnitSystem Hull => hull;

        public PlayerUnitSystem CellsEnergy => cellsEnergy;

        public PlayerUnitSystem BatteryEnergy => batteryEnergy;

        public PlayerUnitEnergyConsumingSystem Thruster => thruster;

        public PlayerUnitEnergyConsumingSystem Nozzle => nozzle;

        public PlayerUnitScannerSystem Scanner => scanner;

        public PlayerUnitArmorSystem? Armor => armor;

        public PlayerUnitSystem? Shield => shield;

        public PlayerUnitEnergyConsumingSystem? Analyzer => analyzer;

        public PlayerUnitSystem? CellsParticles => cellsParticles;

        public PlayerUnitSystem? BatteryParticles => batteryParticles;

        public PlayerUnitSystem? WeaponLauncher => weaponLauncher;

        public PlayerUnitSystem? WeaponPayloadDamage => weaponPayloadDamage;

        public PlayerUnitSystem? WeaponPayloadRadius => weaponPayloadRadius;

        public PlayerUnitSystem? WeaponFactory => weaponFactory;

        public PlayerUnitSystem? WeaponStorage => weaponStorage;

        public PlayerUnitSystem? CargoIron => cargoIron;

        public PlayerUnitSystem? CargoCarbon => cargoCarbon;

        public PlayerUnitSystem? CargoSilicon => cargoSilicon;

        public PlayerUnitSystem? CargoPlatinum => cargoPlatinum;

        public PlayerUnitSystem? CargoGold => cargoGold;

        public PlayerUnitSystem? CargoSpecial => cargoSpecial;

        public PlayerUnitEnergyConsumingSystem? ExtractorIron => extractorIron;

        public PlayerUnitEnergyConsumingSystem? ExtractorCarbon => extractorCarbon;

        public PlayerUnitEnergyConsumingSystem? ExtractorSilicon => extractorSilicon;

        public PlayerUnitEnergyConsumingSystem? ExtractorPlatinum => extractorPlatinum;

        public PlayerUnitEnergyConsumingSystem? ExtractorGold => extractorGold;

        internal Controllable(UniverseGroup group, string name, int id/*, double radius, Vector position, Vector movement, double direction, Team? team, double gravity, double energyOutput, double turnRate*/)
        {
            Group = group;
            Name = name;
            ID = id;

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

            if (value < -Nozzle.MaxValue || value > Nozzle.MaxValue)
                throw new GameException(0x23);

            if (value < -5.0)
                value = -5.0;

            if (value > 5.0)
                value = 5.0;

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

            // TOG: Hier bitte bei mir melden.
            if (direction < 0 || length > 300.1 || length < 59.9 || width < 19.9 || width > 60.1)
                throw new GameException(0x23);

            if (length > 300.0)
                length = 300.0;

            if (length < 60.0)
                length = 60.0;

            if (width < 20.0)
                width = 20.0;

            if (width > 60.0)
                width = 60.0;

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
