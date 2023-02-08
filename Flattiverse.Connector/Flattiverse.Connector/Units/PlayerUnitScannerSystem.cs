using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitScannerSystem : PlayerUnitSystem
    {
        public double MaxRange;
        public double MaxAngle;
        public double EnergyUsagePerSurfaceUnit;

        public PlayerUnitScannerSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            MaxRange = system.Value0;
            MaxAngle = system.Value1;
            EnergyUsagePerSurfaceUnit = system.Value2;
        }
    }
}