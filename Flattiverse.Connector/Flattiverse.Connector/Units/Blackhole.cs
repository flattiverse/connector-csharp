using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("blackhole")]
    public class Blackhole : SteadyUnit
    {
        public GravityWell? GravityWell;

        public List<GravityWellSection>? Sections;

        public Blackhole(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Blackhole(string name, Vector position) : base(name, position)
        {
        }

        public Blackhole(string name) : base(name)
        {
        }

        public Blackhole() : base()
        {
        }

        internal Blackhole(UniverseGroup group, JsonElement element) : base(group, element)
        {
            if (Utils.Traverse(element, out JsonElement gravityWell, "gravityWell"))
            {
                if (gravityWell.ValueKind != JsonValueKind.Object)
                    throw new GameException(0xA3);

                GravityWell = new GravityWell(gravityWell);
            }

            if (Utils.Traverse(element, out JsonElement sections, "sections"))
            {
                if (sections.ValueKind != JsonValueKind.Array)
                    throw new GameException(0xA3);

                foreach (JsonElement section in sections.EnumerateArray())
                {
                    if (section.ValueKind != JsonValueKind.Object)
                        throw new GameException(0xA3);

                    if (Sections == null)
                        Sections = new List<GravityWellSection>();

                    Sections.Add(new GravityWellSection(section));
                }
            }
        }

        public override UnitKind Kind => UnitKind.BlackHole;

        public override string ToString()
        {
            if (GravityWell is null && Sections is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} but no gravitywell or gravitywell section.";

            if (GravityWell is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} and {Sections!.Count} gravitywell sections.";

            if (Sections is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} and a gravitywell with radius {GravityWell!.Radius}.";


            return $"[{Kind}] {Name} at {Position} with radius {Radius} and a gravitywell with radius {GravityWell.Radius} and {Sections.Count} gravitywell sections.";
        }
    }
}