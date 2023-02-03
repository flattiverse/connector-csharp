using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        /// <summary>
        /// The name of the unit, representing his slot in the universegroup's player array.
        /// </summary>
        public readonly string Name;

        internal UnitEvent(JsonElement element) : base()
        {
            Utils.Traverse(element, out Name, "name");
        }
    }
}