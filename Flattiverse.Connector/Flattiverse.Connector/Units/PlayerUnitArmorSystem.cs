using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitArmorSystem : PlayerUnitRegularSystem
    {
        public double IronUsage;
        public double PlatinumUsage;

        public PlayerUnitArmorSystem(UniverseGroup universeGroup, PlayerUnitSystemUpgradepath path) : base(universeGroup, path)
        {
            IronUsage = path.Value1;
            PlatinumUsage = path.Value2;
        }

        public PlayerUnitArmorSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element) : base(group, kind, element)
        {
            IronUsage = system.Value1;
            PlatinumUsage = system.Value2;
        }

        //internal override void Update(PlayerUnitSystem system)
        //{
        //    IronUsage = ((PlayerUnitArmorSystem)system).IronUsage;
        //    PlatinumUsage = ((PlayerUnitArmorSystem)system).PlatinumUsage;
        //    base.Update(system);
        //}
    }
}