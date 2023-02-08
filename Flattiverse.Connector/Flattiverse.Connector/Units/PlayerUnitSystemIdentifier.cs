using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    public struct PlayerUnitSystemIdentifier
    {
        public readonly PlayerUnitSystemKind Kind;
        public readonly int Level;

        internal PlayerUnitSystemIdentifier(JsonElement element)
        {
            Utils.Traverse(element, out string system, "system");
            if (!Enum.TryParse(system, true, out Kind))
                throw new InvalidDataException($"Couldn't parse system: \"{system}\".");
            Utils.Traverse(element, out Level, "level");
        }

        public PlayerUnitSystemIdentifier(PlayerUnitSystemKind kind, int level)
        {
            Kind = kind;
            Level = level;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null || obj is not PlayerUnitSystemIdentifier)
                return false;

            PlayerUnitSystemIdentifier identifier = (PlayerUnitSystemIdentifier)obj;

            return Kind == identifier.Kind && Level == identifier.Level;
        }

        public override int GetHashCode()
        {
            return (int)Kind << 23 | Level;
        }
    }
}