using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("buoy")]
    public class Buoy : SteadyUnit
    {
        public string Message;
        public List<Vector>? Hints;
        public MessageKind MessageKind;

        public Buoy(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Buoy(string name, Vector position) : base(name, position)
        {
        }

        public Buoy(string name) : base(name)
        {
        }

        public Buoy() : base()
        {
        }

        internal Buoy(UniverseGroup group, JsonElement element) : base(group, element)
        {
            IsSolid = false;

            Utils.Traverse(element, out Message, "message");

            if (Utils.Traverse(element, out JsonElement hints, "hints"))
            {
                Hints = new List<Vector>();
                foreach (JsonElement hint in hints.EnumerateArray())
                    Hints.Add(new Vector(hint));
            }

            if (!Utils.Traverse(element, out string messageKind, "messageKind") || !Enum.TryParse(messageKind, true, out MessageKind))
                throw new GameException(0xA1);
        }

        public override UnitKind Kind => UnitKind.Buoy;

        public override string ToString()
        {
            if (Hints is not null)
                return $"[{Kind}] {Name} at {Position} with a {MessageKind} message \"{Message}\" and {Hints.Count} hint vectors.";

            return $"[{Kind}] {Name} at {Position} with a {MessageKind} message \"{Message}\".";
        }
    }
}