using Flattiverse.Connector.Units;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class PlayerUnitSystemUpgradepath
    {
        public readonly PlayerUnitSystemIdentifier? RequiredComponent;

        public readonly double Energy;
        public readonly double Particles;

        public readonly double Iron;
        public readonly double Carbon;
        public readonly double Silicon;
        public readonly double Platinum;
        public readonly double Gold;

        public readonly int Time;

        public readonly double Value0;
        public readonly double Value1;
        public readonly double Value2;

        public PlayerUnitSystemUpgradepath(JsonElement element)
        {
            Utils.Traverse(element, out Energy, "energy");
            Utils.Traverse(element, out Particles, "particles");
            Utils.Traverse(element, out Iron, "iron");
            Utils.Traverse(element, out Carbon, "carbon");
            Utils.Traverse(element, out Silicon, "silicon");
            Utils.Traverse(element, out Platinum, "platinum");
            Utils.Traverse(element, out Gold, "gold");
            Utils.Traverse(element, out Time, "time");
            Utils.Traverse(element, out Value0, "value0");
            Utils.Traverse(element, out Value1, "value1");
            Utils.Traverse(element, out Value2, "value2");

            if (Utils.Traverse(element, out string system, "requiredSystem"))
            {
                if (!Enum.TryParse(system, true, out PlayerUnitSystemKind kind))
                    throw new InvalidDataException($"Couldn't parse requiredSystem: \"{system}\".");
                Utils.Traverse(element, out int level, "requiredLevel");

                RequiredComponent = new PlayerUnitSystemIdentifier(kind, level);
            }
        }

        public PlayerUnitSystemUpgradepath(double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2)
        {
            Energy = energy;
            Particles = particles;
            Iron = iron;
            Carbon = carbon;
            Silicon = silicon;
            Platinum = platinum;
            Gold = gold;
            Time = time;
            Value0 = value0;
            Value1 = value1;
            Value2 = value2;
        }

        public PlayerUnitSystemUpgradepath(double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2, PlayerUnitSystemIdentifier? requiredComponent)
        {
            Energy = energy;
            Particles = particles;
            Iron = iron;
            Carbon = carbon;
            Silicon = silicon;
            Platinum = platinum;
            Gold = gold;
            Time = time;
            Value0 = value0;
            Value1 = value1;
            Value2 = value2;

            RequiredComponent = requiredComponent;
        }
    }
}