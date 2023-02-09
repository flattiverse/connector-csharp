using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : MobileUnit
    {
        public int Player;
        public int Controllable;

        public double TurnRate;

        public PlayerUnitSystem Hull;
        public PlayerUnitSystem CellsEnergy;
        public PlayerUnitSystem BatteryEnergy;
        public PlayerUnitSystem Thruster;
        public PlayerUnitSystem Nozzle;
        public PlayerUnitSystem Scanner;
        public PlayerUnitSystem? Armor;
        public PlayerUnitSystem? Shield;
        public PlayerUnitSystem? Analyzer;
        public PlayerUnitSystem? CellsParticles;
        public PlayerUnitSystem? BatteryParticles;
        public PlayerUnitSystem? WeaponLauncher;
        public PlayerUnitSystem? WeaponPayloadDamage;
        public PlayerUnitSystem? WeaponPayloadRadius;
        public PlayerUnitSystem? WeaponFactory;
        public PlayerUnitSystem? WeaponStorage;
        public PlayerUnitSystem? CargoIron;
        public PlayerUnitSystem? CargoCarbon;
        public PlayerUnitSystem? CargoSilicon;
        public PlayerUnitSystem? CargoPlatinum;
        public PlayerUnitSystem? CargoGold;
        public PlayerUnitSystem? CargoSpecial;
        public PlayerUnitSystem? ExtractorIron;
        public PlayerUnitSystem? ExtractorCarbon;
        public PlayerUnitSystem? ExtractorSilicon;
        public PlayerUnitSystem? ExtractorPlatinum;
        public PlayerUnitSystem? ExtractorGold;

        public PlayerUnit()
        {
        }

        public PlayerUnit(string name) : base(name)
        {
        }

        public PlayerUnit(string name, Vector position) : base(name, position)
        {
        }

        public PlayerUnit(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        internal PlayerUnit(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out Player, "player");
            Utils.Traverse(element, out Controllable, "controllable");
            Utils.Traverse(element, out TurnRate, "turnRate");

            Utils.Traverse(element, out JsonElement systems, "systems");
            foreach (JsonProperty system in systems.EnumerateObject())
            {
                if (!Enum.TryParse(system.Name, true, out PlayerUnitSystemKind kind))
                    group.connection.PushFailureEvent($"Couldn't parse kind in PlayerUnit for system {system.Name}.");

                switch (kind)
                {
                    case PlayerUnitSystemKind.Hull:
                        Hull = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Shield:
                        Shield = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Armor:
                        Armor = new PlayerUnitArmorSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Thruster:
                        Thruster = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Nozzle:
                        Nozzle = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Scanner:
                        Scanner = new PlayerUnitScannerSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.Analyzer:
                        Analyzer = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CellsEnergy:
                        CellsEnergy = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CellsParticles:
                        CellsParticles = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.BatteryEnergy:
                        BatteryEnergy = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.BatteryParticles:
                        BatteryParticles = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.WeaponLauncher:
                        WeaponLauncher = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadDamage:
                        WeaponPayloadDamage = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadRadius:
                        WeaponPayloadRadius = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.WeaponFactory:
                        WeaponFactory = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.WeaponStorage:
                        WeaponStorage = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoIron:
                        CargoIron = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoCarbon:
                        CargoCarbon = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoSilicon:
                        CargoSilicon = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoPlatinum:
                        CargoPlatinum = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoGold:
                        CargoGold = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.CargoSpecial:
                        CargoSpecial = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorIron:
                        ExtractorIron = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorCarbon:
                        ExtractorCarbon = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorSilicon:
                        ExtractorSilicon = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorPlatinum:
                        ExtractorPlatinum = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorGold:
                        ExtractorGold = new PlayerUnitEnergyConsumingSystem(group, kind, element);
                        break;
                    default:
                        group.connection.PushFailureEvent($"PlayerUnitSystemKind {system.Name} is not implemented.");
                        break;
                }
            }
        }

        public override UnitKind Kind => UnitKind.PlayerUnit;
    }
}