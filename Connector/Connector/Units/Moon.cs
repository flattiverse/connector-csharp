using System.Text.Json;

namespace Flattiverse.Units
{
    public class Moon : Unit
    {
        public readonly double Corona;

        public Moon(JsonElement data) : base(data)
        {

        }
    }
}
