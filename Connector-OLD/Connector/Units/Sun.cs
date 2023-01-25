using System.Text.Json;

namespace Flattiverse.Units
{
    public class Sun : Unit
    {
        public readonly double Corona;

        public Sun(JsonElement data) : base(data)
        {
            if(!Utils.Traverse(data, out Corona, "corona"))
                throw new InvalidDataException($"Unit doesn't contain a valid corona property.");
        }
    }
}
