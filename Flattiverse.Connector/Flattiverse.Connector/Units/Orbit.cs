using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class Orbit
    {
        public double Distance;
        public double Angle;
        public int Interval;

        public Orbit(JsonElement element)
        {
            if (!Utils.Traverse(element, out Distance, "distance") ||
                !Utils.Traverse(element, out Angle, "angle") ||
                !Utils.Traverse(element, out Interval, "interval"))
                throw new GameException(0xA1);
        }

        internal void writeJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WriteNumber("distance", Distance);
            writer.WriteNumber("angle", Angle);
            writer.WriteNumber("interval", Interval);

            writer.WriteEndObject();
        }
    }
}