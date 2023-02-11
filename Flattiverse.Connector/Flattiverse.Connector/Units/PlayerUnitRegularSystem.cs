using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitRegularSystem : PlayerUnitSystem
    {
        public double MaxValue;

        public PlayerUnitRegularSystem(UniverseGroup universeGroup, PlayerUnitSystemUpgradepath path) : base(universeGroup, path)
        {
            MaxValue = path.Value0;
        }

        public PlayerUnitRegularSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            MaxValue = system.Value0;
        }

        //internal override void Update(PlayerUnitSystem system)
        //{
        //    MaxValue = ((PlayerUnitRegularSystem)system).MaxValue;
        //    base.Update(system);
        //}
    }
}