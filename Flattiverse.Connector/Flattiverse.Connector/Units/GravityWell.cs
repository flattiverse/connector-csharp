using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class GravityWell
    {
        public double Radius;
        public double Force;

        public GravityWell(JsonElement element)
        {
            if (!Utils.Traverse(element, out Radius, "radius"))
                throw new GameException(0xA1);

            Utils.Traverse(element, out Force, "force");
        }

        internal void writeJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject("gravityWell");

            writer.WriteNumber("radius", Radius);
            writer.WriteNumber("force", Force);

            writer.WriteEndObject();
        }
    }
}