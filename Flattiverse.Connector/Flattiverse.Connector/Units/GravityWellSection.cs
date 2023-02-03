using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class GravityWellSection
    {
        public double AngleStart;
        public double AngleEnd;
        public double DistanceStart;
        public double DistanceEnd;
        public double Force;

        public Activation Activation;

        public ActivationState State;
        public int Frame;

        public GravityWellSection(JsonElement element)
        {
            if (!Utils.Traverse(element, out AngleStart, "angleStart") ||
                !Utils.Traverse(element, out AngleEnd, "angleEnd") ||
                !Utils.Traverse(element, out DistanceStart, "distanceStart") ||
                !Utils.Traverse(element, out DistanceEnd, "distanceEnd"))
                throw new GameException(0xA1);

            Utils.Traverse(element, out Force, "force");

            if (Utils.Traverse(element, out JsonElement activationElement, "activation"))
                Activation = new Activation(activationElement);
        }

        internal void writeJson(Utf8JsonWriter writer, bool mapEdit)
        {
            writer.WriteStartObject();

            writer.WriteNumber("angleStart", AngleStart);
            writer.WriteNumber("angleEnd", AngleEnd);
            writer.WriteNumber("distanceStart", DistanceStart);
            writer.WriteNumber("distanceEnd", DistanceEnd);
            writer.WriteNumber("force", Force);
            
            if (Activation != null)
                Activation.writeJson(writer);

            if (!mapEdit)
            {
                writer.WriteStartObject("activationState");
                writer.WriteString("state", State.ToString().ToLower());
                writer.WriteNumber("frame", Frame);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}