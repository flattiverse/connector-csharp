using System.Reflection.Emit;
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

            if (Utils.Traverse(element, out JsonElement dependency, "dependency"))
            {
                if (dependency.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException($"Couldn't parse dependency: \"{dependency}\".");

                RequiredComponent = new PlayerUnitSystemIdentifier(dependency);
            }
        }
    }
}