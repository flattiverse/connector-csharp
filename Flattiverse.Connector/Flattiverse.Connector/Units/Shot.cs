using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("shot")]
    public class Shot : SteadyUnit
    {
        public double ExplosionDamage;
        public double ExplosionRadius;
        public int LifeTime;

        public Shot(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Shot(string name, Vector position) : base(name, position)
        {
        }

        public Shot(string name) : base(name)
        {
        }

        public Shot() : base()
        {
        }

        internal Shot(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out ExplosionDamage, "explosionDamage");
            Utils.Traverse(element, out ExplosionRadius, "explosionRadius");
            Utils.Traverse(element, out LifeTime, "lifetime");
        }

        public override UnitKind Kind => UnitKind.Shot;
    }
}