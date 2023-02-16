using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("sun")]
    public class Sun : SteadyUnit
    {
        public Corona? Corona;

        public List<CoronaSection>? Sections;

        public Sun(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Sun(string name, Vector position) : base(name, position)
        {
        }

        public Sun(string name) : base(name)
        {
        }

        public Sun() : base()
        {
        }

        internal Sun(UniverseGroup group, JsonElement element) : base(group, element)
        {
            if (Utils.Traverse(element, out JsonElement corona, "corona"))
            {
                if (corona.ValueKind != JsonValueKind.Object)
                    throw new GameException(0xA3);

                Corona = new Corona(corona);
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
                        Sections = new List<CoronaSection>();

                    Sections.Add(new CoronaSection(section));
                }
            }
        }

        public override UnitKind Kind => UnitKind.Sun;

        public override string ToString()
        {
            if (Corona is null && Sections is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} but no corona or corona section.";

            if (Corona is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} and {Sections!.Count} corona sections.";

            if (Sections is null)
                return $"[{Kind}] {Name} at {Position} with radius {Radius} and a corona with radius {Corona!.Radius}.";


            return $"[{Kind}] {Name} at {Position} with radius {Radius} and a corona with radius {Corona.Radius} and {Sections.Count} corona sections.";
        }
    }
}