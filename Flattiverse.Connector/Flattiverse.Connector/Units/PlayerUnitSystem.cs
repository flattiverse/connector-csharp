using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitSystem
    {
        public int Level;
        public double Value;

        public int MaxLevel;

        public PlayerUnitSystemKind Kind;

        internal readonly PlayerUnitSystemUpgradepath system;

        public PlayerUnitSystem(UniverseGroup group, PlayerUnitSystemKind kind, JsonElement element)
        {
            if (!Utils.Traverse(element, out Level, "level"))
                group.connection.PushFailureEvent($"Couldn't read level in PlayerUnit for system {kind}."); //Tog überall gescheite exceptions

            Utils.Traverse(element, out Value, "value");

            group.TryGetSystem(kind, Level, out system);
        }
    }
}
