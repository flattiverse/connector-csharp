using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("explosion")]
    public class Explosion : SteadyUnit
    {
        public double Damage;

        public Explosion(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Explosion(string name, Vector position) : base(name, position)
        {
        }

        public Explosion(string name) : base(name)
        {
        }

        public Explosion() : base()
        {
        }

        internal Explosion(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out Damage, "damage");
        }

        public override UnitKind Kind => UnitKind.Explosion;
    }
}