using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    // TOG: Können wir in das Scanner-System bitte auch Values und ein Bool für Activated einabuen? Oder siehst Du da ein Problem?
    // Also, dass wir später hier Werte wie scanDirection und requestedScanDirection übertragen?
    // Auf lange Sicht hätte ich sowieso gerne alle Werte des Schiffs einfach in Systeme verpackt.
    public class PlayerUnitScannerSystem : PlayerUnitSystem
    {
        public double MaxRange;
        public double MaxAngle;
        public double EnergyUsagePerSurfaceUnit;

        public PlayerUnitScannerSystem(UniverseGroup universeGroup, PlayerUnitSystemUpgradepath path) : base(universeGroup, path)
        {
            MaxRange = path.Value0;
            MaxAngle = path.Value1;
            EnergyUsagePerSurfaceUnit = path.Value2;
        }

        public PlayerUnitScannerSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            MaxRange = system.Value0;
            MaxAngle = system.Value1;
            EnergyUsagePerSurfaceUnit = system.Value2;
        }

        //internal override void Update(PlayerUnitSystem system)
        //{
        //    MaxRange = ((PlayerUnitScannerSystem)system).MaxRange;
        //    MaxAngle = ((PlayerUnitScannerSystem)system).MaxAngle;
        //    EnergyUsagePerSurfaceUnit = ((PlayerUnitScannerSystem)system).EnergyUsagePerSurfaceUnit;
        //    base.Update(system);
        //}
    }
}