using System.Numerics;
using System.Text.Json;

namespace Flattiverse.Units
{
    public class Planet : Unit
    {
        public readonly double Corona;

        public Planet(JsonElement data) : base(data)
        {

        }
    }
}
