using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitArmorSystem : PlayerUnitRegularSystem
    {
        public double IronUsage;
        public double PlatinumUsage;

        public PlayerUnitArmorSystem(JsonElement element) : base(element)
        {
        }
    }
}