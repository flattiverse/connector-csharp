using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flattiverse.Connector
{
    // TOG: Schöne, ausführliche Kommentare schrieben und auch erwähnen welche Exceptions passieren können. Zudem noch remarks verwenden und erklären,w as Leute bei den entsprechenden Kommandos zu erwarten haben.
    //      Diese Zeile stehen lassen, bis Sonntag Abend.
    public class Controllable
    {
        public readonly UniverseGroup Group;
        public readonly string Name;
        public readonly int ID;

        public readonly double Radius;
        public readonly Vector Position;
        public readonly Vector Movement;
        public readonly double Direction;
        public readonly Team? Team;
        public readonly double Gravity;
        public readonly double EnergyOutput;
        public readonly bool IsAlive;
        public readonly double TurnRate;

        public readonly PlayerUnitSystem Hull;
        public readonly PlayerUnitSystem CellsEnergy;
        public readonly PlayerUnitSystem BatteryEnergy;
        public readonly PlayerUnitSystem Thruster;
        public readonly PlayerUnitSystem Nozzle;
        public readonly PlayerUnitSystem Scanner;
        public readonly PlayerUnitSystem? Armor;
        public readonly PlayerUnitSystem? Shield;
        public readonly PlayerUnitSystem? Analyzer;
        public readonly PlayerUnitSystem? CellsParticles;
        public readonly PlayerUnitSystem? BatteryParticles;
        public readonly PlayerUnitSystem? WeaponLauncher;
        public readonly PlayerUnitSystem? WeaponPayloadDamage;
        public readonly PlayerUnitSystem? WeaponPayloadRadius;
        public readonly PlayerUnitSystem? WeaponFactory;
        public readonly PlayerUnitSystem? WeaponStorage;
        public readonly PlayerUnitSystem? CargoIron;
        public readonly PlayerUnitSystem? CargoCarbon;
        public readonly PlayerUnitSystem? CargoSilicon;
        public readonly PlayerUnitSystem? CargoPlatinum;
        public readonly PlayerUnitSystem? CargoGold;
        public readonly PlayerUnitSystem? CargoSpecial;
        public readonly PlayerUnitSystem? ExtractorIron;
        public readonly PlayerUnitSystem? ExtractorCarbon;
        public readonly PlayerUnitSystem? ExtractorSilicon;
        public readonly PlayerUnitSystem? ExtractorPlatinum;
        public readonly PlayerUnitSystem? ExtractorGold;

        internal Controllable(UniverseGroup group, string name, int id/*, double radius, Vector position, Vector movement, double direction, Team? team, double gravity, */)
        {
            Group = group;
            Name = name;
            ID = id;

            foreach (PlayerUnitSystemKind kind in Enum.GetValues(typeof(PlayerUnitSystemKind)))
                if (PlayerUnitSystem.TryGetStartSystem(group, kind, out PlayerUnitSystem? system))
                {
                    switch (kind)
                    {
                        case PlayerUnitSystemKind.Hull:
                            Hull = system;
                            break;
                        case PlayerUnitSystemKind.Shield:
                            Shield = system;
                            break;
                        case PlayerUnitSystemKind.Armor:
                            Armor = system;
                            break;
                        case PlayerUnitSystemKind.Thruster:
                            Thruster = system;
                            break;
                        case PlayerUnitSystemKind.Nozzle:
                            Nozzle = system;
                            break;
                        case PlayerUnitSystemKind.Scanner:
                            Scanner = system;
                            break;
                        case PlayerUnitSystemKind.Analyzer:
                            Analyzer = system;
                            break;
                        case PlayerUnitSystemKind.CellsEnergy:
                            CellsEnergy = system;
                            break;
                        case PlayerUnitSystemKind.CellsParticles:
                            CellsParticles = system;
                            break;
                        case PlayerUnitSystemKind.BatteryEnergy:
                            BatteryEnergy = system;
                            break;
                        case PlayerUnitSystemKind.BatteryParticles:
                            BatteryParticles = system;
                            break;
                        case PlayerUnitSystemKind.WeaponLauncher:
                            WeaponLauncher = system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadDamage:
                            WeaponPayloadDamage = system;
                            break;
                        case PlayerUnitSystemKind.WeaponPayloadRadius:
                            WeaponPayloadRadius = system;
                            break;
                        case PlayerUnitSystemKind.WeaponFactory:
                            WeaponFactory = system;
                            break;
                        case PlayerUnitSystemKind.WeaponStorage:
                            WeaponStorage = system;
                            break;
                        case PlayerUnitSystemKind.CargoIron:
                            CargoIron = system;
                            break;
                        case PlayerUnitSystemKind.CargoCarbon:
                            CargoCarbon = system;
                            break;
                        case PlayerUnitSystemKind.CargoSilicon:
                            CargoSilicon = system;
                            break;
                        case PlayerUnitSystemKind.CargoPlatinum:
                            CargoPlatinum = system;
                            break;
                        case PlayerUnitSystemKind.CargoGold:
                            CargoGold = system;
                            break;
                        case PlayerUnitSystemKind.CargoSpecial:
                            CargoSpecial = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorIron:
                            ExtractorIron = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorCarbon:
                            ExtractorCarbon = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorSilicon:
                            ExtractorSilicon = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorPlatinum:
                            ExtractorPlatinum = system;
                            break;
                        case PlayerUnitSystemKind.ExtractorGold:
                            ExtractorGold = system;
                            break;
                        default:
                            group.connection.PushFailureEvent($"PlayerUnitSystemKind {kind} is not implemented.");
                            break;
                    }
                }
        }

        public async Task Continue()
        {
            if (Hull.Value > 0.0)
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
            if (Hull.Value <= 0.0)
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
