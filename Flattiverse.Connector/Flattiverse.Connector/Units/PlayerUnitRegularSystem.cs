using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitRegularSystem : PlayerUnitSystem
    {
        public double MaxValue;

        public PlayerUnitRegularSystem(JsonElement element) : base(element)
        {
        }
    }
}