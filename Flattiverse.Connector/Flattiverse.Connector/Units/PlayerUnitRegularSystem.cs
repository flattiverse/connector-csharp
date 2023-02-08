using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitRegularSystem : PlayerUnitSystem
    {
        public double MaxValue;

        public PlayerUnitRegularSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            MaxValue = system.Value0;
        }
    }
}