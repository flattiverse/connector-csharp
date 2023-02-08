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
        }
    }
}