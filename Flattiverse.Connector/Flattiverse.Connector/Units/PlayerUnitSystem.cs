using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
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

        internal static bool TryGetStartSystem(UniverseGroup universeGroup, PlayerUnitSystemKind kind, [NotNullWhen(returnValue: true)] out PlayerUnitSystem? system)
        {
            PlayerUnitSystemUpgradepath? systemUpgradepath;

            if (!universeGroup.Systems.TryGetValue(new PlayerUnitSystemIdentifier(kind, 0), out systemUpgradepath))
            {
                system = null;
                return false;
            }

            switch (kind)
            {
                default:
                    system = new PlayerUnitRegularSystem(universeGroup, systemUpgradepath);
                    break;
                case PlayerUnitSystemKind.ExtractorCarbon:
                case PlayerUnitSystemKind.ExtractorIron:
                case PlayerUnitSystemKind.ExtractorSilicon:
                case PlayerUnitSystemKind.ExtractorPlatinum:
                case PlayerUnitSystemKind.ExtractorGold:
                case PlayerUnitSystemKind.Thruster:
                case PlayerUnitSystemKind.Analyzer:
                case PlayerUnitSystemKind.Nozzle:
                    system = new PlayerUnitEnergyConsumingSystem(universeGroup, systemUpgradepath);
                    break;
                case PlayerUnitSystemKind.Scanner:
                    system = new PlayerUnitScannerSystem(universeGroup, systemUpgradepath);
                    break;
                case PlayerUnitSystemKind.Armor:
                    system = new PlayerUnitArmorSystem(universeGroup, systemUpgradepath);
                    break;
            }

            return true;
        }

        internal PlayerUnitSystem(UniverseGroup universeGroup, PlayerUnitSystemUpgradepath path)
        {
            for (int level = 1; level < 32; level++)
                if (!universeGroup.Systems.TryGetValue(new PlayerUnitSystemIdentifier(path.Kind, level), out _))
                    MaxLevel = level - 1;

            Kind = path.Kind;
            Level = path.Level;
        }
    }
}
