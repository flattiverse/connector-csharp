using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitEnergyConsumingSystem : PlayerUnitRegularSystem
    {
        public double EnergyUsage;
        public double ParticlesUsage;

        public PlayerUnitEnergyConsumingSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            EnergyUsage = system.Value1;
            ParticlesUsage = system.Value2;
        }
    }
}