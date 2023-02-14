using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("playerUnit")]
    public class PlayerUnit : MobileUnit
    {
        public Player Player;
        public int Controllable;

        public double TurnRate;
        public double RequestedScanDirection;
        public double RequestedScanWidth;
        public double RequestedScanRange;
        public double ScanDirection;
        public double ScanWidth;
        public double ScanRange;

        public PlayerUnitRegularSystem Hull;
        public PlayerUnitRegularSystem CellsEnergy;
        public PlayerUnitRegularSystem BatteryEnergy;
        public PlayerUnitEnergyConsumingSystem Thruster;
        public PlayerUnitEnergyConsumingSystem Nozzle;
        public PlayerUnitScannerSystem Scanner;
        public PlayerUnitArmorSystem? Armor;
        public PlayerUnitRegularSystem? Shield;
        public PlayerUnitEnergyConsumingSystem? Analyzer;
        public PlayerUnitRegularSystem? CellsParticles;
        public PlayerUnitRegularSystem? BatteryParticles;
        public PlayerUnitRegularSystem? WeaponLauncher;
        public PlayerUnitRegularSystem? WeaponPayloadDamage;
        public PlayerUnitRegularSystem? WeaponPayloadRadius;
        public PlayerUnitRegularSystem? WeaponFactory;
        public PlayerUnitRegularSystem? WeaponStorage;
        public PlayerUnitRegularSystem? CargoIron;
        public PlayerUnitRegularSystem? CargoCarbon;
        public PlayerUnitRegularSystem? CargoSilicon;
        public PlayerUnitRegularSystem? CargoPlatinum;
        public PlayerUnitRegularSystem? CargoGold;
        public PlayerUnitRegularSystem? CargoSpecial;
        public PlayerUnitEnergyConsumingSystem? ExtractorIron;
        public PlayerUnitEnergyConsumingSystem? ExtractorCarbon;
        public PlayerUnitEnergyConsumingSystem? ExtractorSilicon;
        public PlayerUnitEnergyConsumingSystem? ExtractorPlatinum;
        public PlayerUnitEnergyConsumingSystem? ExtractorGold;

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
            Utils.Traverse(element, out int playerId, "player");
            if (group.playersId[playerId] is null)
                group.connection.PushFailureEvent($"Tried to instantiate a PlayerUnit for a player that does not exist in the universe.");
            else
                Player = group.playersId[playerId];

            Utils.Traverse(element, out Direction, "direction");
            Utils.Traverse(element, out Controllable, "controllable");
            Utils.Traverse(element, out TurnRate, "turnRate");
            Utils.Traverse(element, out RequestedScanDirection, "requestedScanDirection"); 
            Utils.Traverse(element, out RequestedScanWidth, "requestedScanWidth"); 
            Utils.Traverse(element, out RequestedScanRange, "requestedScanRange"); 
            Utils.Traverse(element, out ScanDirection, "scanDirection"); 
            Utils.Traverse(element, out ScanWidth, "scanWidth"); 
            Utils.Traverse(element, out ScanRange, "scanRange"); 

            Utils.Traverse(element, out JsonElement systems, "systems");
            foreach (JsonProperty system in systems.EnumerateObject())
            {
                if (!Enum.TryParse(system.Name, true, out PlayerUnitSystemKind kind))
                    group.connection.PushFailureEvent($"Couldn't parse kind in PlayerUnit for system {system.Name}.");

                switch (kind)
                {
                    case PlayerUnitSystemKind.Hull:
                        Hull = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Shield:
                        Shield = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Armor:
                        Armor = new PlayerUnitArmorSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Thruster:
                        Thruster = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Nozzle:
                        Nozzle = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Scanner:
                        Scanner = new PlayerUnitScannerSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.Analyzer:
                        Analyzer = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CellsEnergy:
                        CellsEnergy = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CellsParticles:
                        CellsParticles = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.BatteryEnergy:
                        BatteryEnergy = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.BatteryParticles:
                        BatteryParticles = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponLauncher:
                        WeaponLauncher = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadDamage:
                        WeaponPayloadDamage = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponPayloadRadius:
                        WeaponPayloadRadius = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponFactory:
                        WeaponFactory = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.WeaponStorage:
                        WeaponStorage = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoIron:
                        CargoIron = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoCarbon:
                        CargoCarbon = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoSilicon:
                        CargoSilicon = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoPlatinum:
                        CargoPlatinum = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoGold:
                        CargoGold = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.CargoSpecial:
                        CargoSpecial = new PlayerUnitRegularSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorIron:
                        ExtractorIron = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorCarbon:
                        ExtractorCarbon = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorSilicon:
                        ExtractorSilicon = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorPlatinum:
                        ExtractorPlatinum = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
                        break;
                    case PlayerUnitSystemKind.ExtractorGold:
                        ExtractorGold = new PlayerUnitEnergyConsumingSystem(group, kind, system.Value);
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