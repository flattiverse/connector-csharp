using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitRegularSystem : PlayerUnitSystem
    {
        public double MaxValue;

        public PlayerUnitRegularSystem(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out MaxValue, "maxValue"))
                throw new GameException(0xA1);
        }
    }
}