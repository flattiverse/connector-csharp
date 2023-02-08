using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitScannerSystem : PlayerUnitRegularSystem
    {
        public double MaxRange;
        public double MaxAngle;
        public double EnergyUsagePerSurfaceUnit;

        public PlayerUnitScannerSystem(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out MaxRange, "maxRange"))
                throw new GameException(0xA1);

            if (!Utils.Traverse(element, out MaxAngle, "maxAngle"))
                throw new GameException(0xA1);

            if (!Utils.Traverse(element, out EnergyUsagePerSurfaceUnit, "energyUsagePerSurfaceUnit"))
                throw new GameException(0xA1);
        }
    }
}