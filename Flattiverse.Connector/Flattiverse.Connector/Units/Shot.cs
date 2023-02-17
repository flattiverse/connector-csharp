using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("shot")]
    public class Shot : MobileUnit
    {
        public double ExplosionDamage;
        public double ExplosionRadius;
        public int Lifetime;

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
            IsSolid = false;

            Utils.Traverse(element, out ExplosionDamage, "explosionDamage");
            Utils.Traverse(element, out ExplosionRadius, "explosionRadius");
            Utils.Traverse(element, out Lifetime, "lifetime");
        }

        public override UnitKind Kind => UnitKind.Shot;

        public override string ToString()
        {
            return $"[{Kind}] at {Position} with explosion damage {ExplosionDamage}, explosion radius {ExplosionRadius} and remaining lifetime {Lifetime}.";
        }
    }
}