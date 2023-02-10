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
        private PlayerUnitSystem thruster;
        private PlayerUnitSystem nozzle;
        private PlayerUnitSystem scanner;
        private PlayerUnitSystem? armor;
        private PlayerUnitSystem? shield;
        private PlayerUnitSystem? analyzer;
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
        private PlayerUnitSystem? extractorIron;
        private PlayerUnitSystem? extractorCarbon;
        private PlayerUnitSystem? extractorSilicon;
        private PlayerUnitSystem? extractorPlatinum;
        private PlayerUnitSystem? extractorGold;

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

        public PlayerUnitSystem Thruster => thruster;

        public PlayerUnitSystem Nozzle => nozzle;

        public PlayerUnitSystem Scanner => scanner;

        public PlayerUnitSystem? Armor => armor;

        public PlayerUnitSystem? Shield => shield;

        public PlayerUnitSystem? Analyzer => analyzer;

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

        public PlayerUnitSystem? ExtractorIron => extractorIron;

        public PlayerUnitSystem? ExtractorCarbon => extractorCarbon;

        public PlayerUnitSystem? ExtractorSilicon => extractorSilicon;

        public PlayerUnitSystem? ExtractorPlatinum => extractorPlatinum;

        public PlayerUnitSystem? ExtractorGold => extractorGold;

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
                            armor = system;
                            break;
                        case PlayerUnitSystemKind.Thruster:
                            thruster = system;
                            break;
                        case PlayerUnitSystemKind.Nozzle:
                            nozzle = system;
                            break;
                        case PlayerUnitSystemKind.Scanner:
                            scanner = system;
                            break;
                        case PlayerUnitSystemKind.Analyzer:
                            analyzer = system;
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
                            extractorIron = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorCarbon:
                            extractorCarbon = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorSilicon:
                            extractorSilicon = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorPlatinum:
                            extractorPlatinum = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorGold:
                            extractorGold = system;
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
            hull.Value = playerUnit.Hull.Value;

            // TOG: Hier müssen im Prinzip alle Werte übernommen werden, wann immer ein Update kommt.
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
    }
}
