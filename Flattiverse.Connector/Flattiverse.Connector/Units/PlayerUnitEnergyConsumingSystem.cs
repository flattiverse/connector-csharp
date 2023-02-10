using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitEnergyConsumingSystem : PlayerUnitRegularSystem
    {
        public double EnergyUsage;
        public double ParticlesUsage;

        public PlayerUnitEnergyConsumingSystem(UniverseGroup universeGroup, PlayerUnitSystemUpgradepath path) : base(universeGroup, path)
        {
            EnergyUsage = path.Value1;
            ParticlesUsage = path.Value2;
        }

        public PlayerUnitEnergyConsumingSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            EnergyUsage = system.Value1;
            ParticlesUsage = system.Value2;
        }
    }
}