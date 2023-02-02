using System.Text.Json;

namespace Flattiverse.Connector
{
    /// <summary>
    /// Represents an universe within your universe group.
    /// </summary>
    public class Universe
    {
        /// <summary>
        /// The ID of the universe.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// The name of the universe.
        /// </summary>
        public readonly string Name;

        public Universe(JsonElement element)
        {
            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");
        }
    }
}