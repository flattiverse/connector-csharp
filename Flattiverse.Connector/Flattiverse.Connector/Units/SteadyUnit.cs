using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class SteadyUnit : Unit
    {
        public IReadOnlyCollection<Orbit>? Orbits => orbits;

        internal readonly List<Orbit>? orbits;

        public SteadyUnit(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public SteadyUnit(string name, Vector position) : base(name, position)
        {
        }

        public SteadyUnit(string name) : base(name)
        {
        }

        public SteadyUnit() : base()
        {
        }

        internal SteadyUnit(JsonElement element) : base(element)
        {
            if (Utils.Traverse(element, out JsonElement orbiting, "orbiting"))
            {
                if (orbiting.ValueKind != JsonValueKind.Array)
                    throw new GameException(0xA2);

                foreach (JsonElement orbit in orbiting.EnumerateArray())
                {
                    if (orbit.ValueKind != JsonValueKind.Object)
                        throw new GameException(0xA2);

                    if (orbits == null)
                        orbits = new List<Orbit>();

                    orbits.Add(new Orbit(orbit));
                }
            }

            Mobility = orbits == null ? Mobility.Still : Mobility.Steady;
        }

        public override bool MapEditable => true;
    }
}