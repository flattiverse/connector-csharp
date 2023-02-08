using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitEnergyConsumingSystem : PlayerUnitRegularSystem
    {
        public double EnergyUsage;
        public double ParticlesUsage;

        public PlayerUnitEnergyConsumingSystem(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out EnergyUsage, "energyUsage"))
                throw new GameException(0xA1);

            if (!Utils.Traverse(element, out ParticlesUsage, "particlesUsage"))
                throw new GameException(0xA1);
        }
    }
}