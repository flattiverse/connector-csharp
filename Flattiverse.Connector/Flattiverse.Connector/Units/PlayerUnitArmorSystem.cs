using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitArmorSystem : PlayerUnitRegularSystem
    {
        public double IronUsage;
        public double PlatinumUsage;

        public PlayerUnitArmorSystem(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out IronUsage, "ironUsage"))
                throw new GameException(0xA1);

            if (!Utils.Traverse(element, out PlatinumUsage, "platinumUsage"))
                throw new GameException(0xA1);
        }
    }
}