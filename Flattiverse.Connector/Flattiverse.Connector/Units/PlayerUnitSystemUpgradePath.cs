using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitSystemUpgradepath
    {
        public readonly PlayerUnitSystemIdentifier? RequiredComponent;

        public readonly PlayerUnitSystemKind Kind;
        public readonly int Level;

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

        public readonly double AreaIncrease;
        public readonly double WeightIncrease;

        public PlayerUnitSystemUpgradepath(JsonElement element)
        {
            Utils.Traverse(element, out string system, "system");
            if (!Enum.TryParse(system, true, out Kind))
                throw new InvalidDataException($"Couldn't parse system: \"{system}\".");

            Utils.Traverse(element, out Level, "level");

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
            Utils.Traverse(element, out AreaIncrease, "areaIncrease");
            Utils.Traverse(element, out WeightIncrease, "weightIncrease");

            if (Utils.Traverse(element, out string requiredSystem, "requiredSystem"))
            {
                if (!Enum.TryParse(requiredSystem, true, out PlayerUnitSystemKind requiredKind))
                    throw new InvalidDataException($"Couldn't parse requiredSystem: \"{requiredSystem}\".");
                Utils.Traverse(element, out int level, "requiredLevel");

                RequiredComponent = new PlayerUnitSystemIdentifier(requiredKind, level);
            }
        }

        public PlayerUnitSystemUpgradepath(PlayerUnitSystemKind kind, int level, double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2, double areaIncrease, double weightIncrease)
        {
            Kind = kind;
            Level = level;
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
            AreaIncrease = areaIncrease;
            WeightIncrease = weightIncrease;
        }

        public PlayerUnitSystemUpgradepath(PlayerUnitSystemKind kind, int level, double energy, double particles, double iron, double carbon, double silicon, double platinum, double gold, int time, double value0, double value1, double value2, double areaIncrease, double weightIncrease, PlayerUnitSystemIdentifier? requiredComponent)
        {
            Kind = kind;
            Level = level;
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
            AreaIncrease = areaIncrease;
            WeightIncrease = weightIncrease;

            RequiredComponent = requiredComponent;
        }
    }
}