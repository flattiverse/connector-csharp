using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitSystem
    {
        public int Level;
        public double Value;

        public int MaxLevel;

        public PlayerUnitSystemKind Kind;

        public PlayerUnitSystem(JsonElement element)
        {
            if (!Utils.Traverse(element, out Level, "level"))
                throw new GameException(0xA1);

            if (!Utils.Traverse(element, out Value, "value"))
                throw new GameException(0xA1);
        }
    }
}
