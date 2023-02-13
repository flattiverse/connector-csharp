using System.Text.Json;
namespace Flattiverse.Connector.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        public readonly Universe Universe;

        internal UnitEvent(UniverseGroup group, JsonElement element) : base()
        {
            Utils.Traverse(element, out int universe, "universe");
            Universe = group.universesId[universe];
        }
    }
}