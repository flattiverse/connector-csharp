using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : MobileUnit
    {
        public int Account;
        public int Controllable;

        public double TurnRate;

        //TOG: Pflicht: Hull, EnergyCell, EnergyBattery, Thruster, Nozzle, Scanner, Rest opt.
        public PlayerUnitSystem Hull;
        public PlayerUnitSystem? Armor;
        public PlayerUnitSystem? Shield;

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
            Utils.Traverse(element, out Account, "account");
            Utils.Traverse(element, out Controllable, "controllable");
            Utils.Traverse(element, out TurnRate, "turnRate");

            Utils.Traverse(element, out JsonElement systems, "systems");
            foreach (JsonProperty system in systems.EnumerateObject())
            {
                if (!Enum.TryParse(system.Name, true, out PlayerUnitSystemKind kind))
                    group.connection.PushFailureEvent($"Couldn't parse kind in PlayerUnit for system {system.Name}.");

                //system.Name -> kind
                //system.Value -> object
                //switch by kind für konstruktor

                switch (kind)
                {
                    case PlayerUnitSystemKind.Hull:
                        Hull = new PlayerUnitRegularSystem(group, kind, element);
                        break;
                    case PlayerUnitSystemKind.ExtractorCarbon:
                    case PlayerUnitSystemKind.ExtractorIron:
                    case PlayerUnitSystemKind.ExtractorSilicon:
                    case PlayerUnitSystemKind.ExtractorPlatinum:
                    case PlayerUnitSystemKind.ExtractorGold:
                    case PlayerUnitSystemKind.Thruster:
                    case PlayerUnitSystemKind.Analyzer:
                    //case PlayerUnitSystemKind.Nozzle:
                    //    Systems.Add(kind, new PlayerUnitRegularSystem(group, kind, element));
                    //    system = new PlayerUnitEnergyConsumingSystem(systemUpgradepath);
                    //    break;
                    //case PlayerUnitSystemKind.Scanner:
                    //    Systems.Add(kind, new PlayerUnitRegularSystem(group, kind, element));
                    //    system = new PlayerUnitScannerSystem(systemUpgradepath);
                    //    break;
                    //case PlayerUnitSystemKind.Armor:
                    //    Systems.Add(kind, new PlayerUnitRegularSystem(group, kind, element));
                    //    system = new PlayerUnitArmorSystem(systemUpgradepath);
                    //    break;
                    default:
                        break;
                        //TOG: Ausformulieren.
                }
            }


        }

        public override UnitKind Kind => UnitKind.PlayerUnit;
    }
}