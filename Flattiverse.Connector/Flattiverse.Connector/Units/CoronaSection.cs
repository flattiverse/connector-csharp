using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class CoronaSection
    {
        public double AngleStart;
        public double AngleEnd;
        public double DistanceStart;
        public double DistanceEnd;
        public double Energy;
        public double Particles;

        public Activation Activation;

        public ActivationState State;
        public int Frame;

        public CoronaSection(double angleStart, double angleEnd, double distanceStart, double distanceEnd, double energy, double particles, Activation activation, ActivationState state, int frame)
        {
            AngleStart = angleStart;
            AngleEnd = angleEnd;
            DistanceStart = distanceStart;
            DistanceEnd = distanceEnd;
            Energy = energy;
            Particles = particles;
            Activation = activation;
            State = state;
            Frame = frame;
        }

        public CoronaSection(JsonElement element)
        {
            if (!Utils.Traverse(element, out AngleStart, "angleStart") ||
                !Utils.Traverse(element, out AngleEnd, "angleEnd") ||
                !Utils.Traverse(element, out DistanceStart, "distanceStart") ||
                !Utils.Traverse(element, out DistanceEnd, "distanceEnd"))
                throw new GameException(0xA1);

            Utils.Traverse(element, out Energy, "energy");
            Utils.Traverse(element, out Particles, "particles");

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
            writer.WriteNumber("energy", Energy);
            writer.WriteNumber("particles", Particles);

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