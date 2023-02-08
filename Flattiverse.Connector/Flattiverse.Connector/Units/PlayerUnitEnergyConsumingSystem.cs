using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitEnergyConsumingSystem : PlayerUnitRegularSystem
    {
        public double EnergyUsage;
        public double ParticlesUsage;

        public PlayerUnitEnergyConsumingSystem(JsonElement element) : base(element)
        {
        }
    }
}