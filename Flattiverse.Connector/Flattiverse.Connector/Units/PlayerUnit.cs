using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : MobileUnit
    {
        public int Account;
        public int Controllable;

        public double TurnRate;

        public Dictionary<PlayerUnitSystemKind, PlayerUnitSystem> Systems;
        
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


        }

        public override UnitKind Kind => UnitKind.PlayerUnit;        
    }
}