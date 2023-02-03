using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class Activation
    {
        public readonly double Probability;
        public readonly int Foreshadowing;
        public readonly int Upramp;
        public readonly int Time;
        public readonly int Fade;

        public Activation(JsonElement element)
        {
            Utils.Traverse(element, out Probability, "probability");
            Utils.Traverse(element, out Time, "time");
            Utils.Traverse(element, out Foreshadowing, "foreshadowing");
            Utils.Traverse(element, out Upramp, "upramp");
            Utils.Traverse(element, out Fade, "fade");
        }

        internal void writeJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject("activation");
            writer.WriteNumber("probability", Probability);
            writer.WriteNumber("foreshadowing", Foreshadowing);
            writer.WriteNumber("upramp", Upramp);
            writer.WriteNumber("time", Time);
            writer.WriteNumber("fade", Fade);
            writer.WriteEndObject();
        }
    }
}
