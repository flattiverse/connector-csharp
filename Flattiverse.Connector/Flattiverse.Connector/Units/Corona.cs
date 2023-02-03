using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class Corona
    {
        public double Radius;
        public double Energy;
        public double Particles;

        public Corona(JsonElement element)
        {
            if (!Utils.Traverse(element, out Radius, "radius"))
                throw new GameException(0xA1);

            Utils.Traverse(element, out Energy, "energy");
            Utils.Traverse(element, out Particles, "particles");
        }

        internal void writeJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject("corona");

            writer.WriteNumber("radius", Radius);
            writer.WriteNumber("energy", Energy);
            writer.WriteNumber("particles", Particles);

            writer.WriteEndObject();
        }
    }
}