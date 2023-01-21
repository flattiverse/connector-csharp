using System.Text.Json;

namespace Flattiverse.Units
{
    public class PlayerShip : Unit
    {
        public readonly string UserName;

        public PlayerShip(JsonElement data) : base(data)
        {
            if (!Utils.Traverse(data, out UserName, false, "player"))
                throw new InvalidDataException($"Unit doesn't contain a valid playername property.");
        }
    }
}
