using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitArmorSystem : PlayerUnitRegularSystem
    {
        public double IronUsage;
        public double PlatinumUsage;

        public PlayerUnitArmorSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            IronUsage = system.Value1;
            PlatinumUsage = system.Value2;
        }
    }
}