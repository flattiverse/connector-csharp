using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnit : MobileUnit
    {

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
            
        }

        public override UnitKind Kind => UnitKind.PlayerUnit;
    }
}