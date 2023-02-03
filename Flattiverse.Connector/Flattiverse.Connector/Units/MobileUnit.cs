using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class MobileUnit : Unit
    {
        public MobileUnit(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public MobileUnit(string name, Vector position) : base(name, position)
        {
        }

        public MobileUnit(string name) : base(name)
        {
        }

        public MobileUnit() : base()
        {
        }

        internal MobileUnit(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Mobility = Mobility.Mobile;
        }

        public override bool MapEditable => false;
    }
}