using System.Collections;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Session-level build-assistance self-disclosure.
/// </summary>
public sealed class BuildDisclosure : IEnumerable<KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>>
{
    internal const int NibbleCount = 12;
    internal const int ByteCount = 6;

    /// <summary>
    /// Software-design disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel SoftwareDesign;

    /// <summary>
    /// UI disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel UI;

    /// <summary>
    /// Universe-rendering disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel UniverseRendering;

    /// <summary>
    /// Input disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel Input;

    /// <summary>
    /// Engine-control disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel EngineControl;

    /// <summary>
    /// Navigation disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel Navigation;

    /// <summary>
    /// Scanner-control disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel ScannerControl;

    /// <summary>
    /// Weapon-systems disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel WeaponSystems;

    /// <summary>
    /// Resource-control disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel ResourceControl;

    /// <summary>
    /// Fleet-control disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel FleetControl;

    /// <summary>
    /// Mission-control disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel MissionControl;

    /// <summary>
    /// Chat disclosure.
    /// </summary>
    public readonly BuildDisclosureLevel Chat;

    /// <summary>
    /// Creates a build-assistance disclosure snapshot.
    /// </summary>
    public BuildDisclosure(BuildDisclosureLevel softwareDesign, BuildDisclosureLevel ui, BuildDisclosureLevel universeRendering,
        BuildDisclosureLevel input, BuildDisclosureLevel engineControl, BuildDisclosureLevel navigation,
        BuildDisclosureLevel scannerControl, BuildDisclosureLevel weaponSystems, BuildDisclosureLevel resourceControl,
        BuildDisclosureLevel fleetControl, BuildDisclosureLevel missionControl, BuildDisclosureLevel chat)
    {
        SoftwareDesign = softwareDesign;
        UI = ui;
        UniverseRendering = universeRendering;
        Input = input;
        EngineControl = engineControl;
        Navigation = navigation;
        ScannerControl = scannerControl;
        WeaponSystems = weaponSystems;
        ResourceControl = resourceControl;
        FleetControl = fleetControl;
        MissionControl = missionControl;
        Chat = chat;
    }

    /// <summary>
    /// Returns the disclosed level of one build aspect.
    /// </summary>
    public BuildDisclosureLevel this[BuildDisclosureAspect aspect] => aspect switch
    {
        BuildDisclosureAspect.SoftwareDesign => SoftwareDesign,
        BuildDisclosureAspect.UI => UI,
        BuildDisclosureAspect.UniverseRendering => UniverseRendering,
        BuildDisclosureAspect.Input => Input,
        BuildDisclosureAspect.EngineControl => EngineControl,
        BuildDisclosureAspect.Navigation => Navigation,
        BuildDisclosureAspect.ScannerControl => ScannerControl,
        BuildDisclosureAspect.WeaponSystems => WeaponSystems,
        BuildDisclosureAspect.ResourceControl => ResourceControl,
        BuildDisclosureAspect.FleetControl => FleetControl,
        BuildDisclosureAspect.MissionControl => MissionControl,
        BuildDisclosureAspect.Chat => Chat,
        _ => throw new ArgumentOutOfRangeException(nameof(aspect), aspect, null)
    };

    /// <summary>
    /// Enumerates all disclosed build aspects in fixed wire order.
    /// </summary>
    public IEnumerator<KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>> GetEnumerator()
    {
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.SoftwareDesign, SoftwareDesign);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.UI, UI);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.UniverseRendering, UniverseRendering);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.Input, Input);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.EngineControl, EngineControl);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.Navigation, Navigation);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.ScannerControl, ScannerControl);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.WeaponSystems, WeaponSystems);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.ResourceControl, ResourceControl);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.FleetControl, FleetControl);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.MissionControl, MissionControl);
        yield return new KeyValuePair<BuildDisclosureAspect, BuildDisclosureLevel>(BuildDisclosureAspect.Chat, Chat);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Serializes this disclosure back to its connect-query hex form.
    /// </summary>
    public override string ToString()
    {
        return string.Create(NibbleCount, this, static (span, disclosure) =>
        {
            span[0] = NibbleToHex((byte)disclosure.SoftwareDesign);
            span[1] = NibbleToHex((byte)disclosure.UI);
            span[2] = NibbleToHex((byte)disclosure.UniverseRendering);
            span[3] = NibbleToHex((byte)disclosure.Input);
            span[4] = NibbleToHex((byte)disclosure.EngineControl);
            span[5] = NibbleToHex((byte)disclosure.Navigation);
            span[6] = NibbleToHex((byte)disclosure.ScannerControl);
            span[7] = NibbleToHex((byte)disclosure.WeaponSystems);
            span[8] = NibbleToHex((byte)disclosure.ResourceControl);
            span[9] = NibbleToHex((byte)disclosure.FleetControl);
            span[10] = NibbleToHex((byte)disclosure.MissionControl);
            span[11] = NibbleToHex((byte)disclosure.Chat);
        });
    }

    internal static bool TryRead(Flattiverse.Connector.Network.PacketReader reader, out BuildDisclosure? disclosure)
    {
        if (!reader.Read(out byte packed0) ||
            !reader.Read(out byte packed1) ||
            !reader.Read(out byte packed2) ||
            !reader.Read(out byte packed3) ||
            !reader.Read(out byte packed4) ||
            !reader.Read(out byte packed5))
        {
            disclosure = null;
            return false;
        }

        BuildDisclosureLevel softwareDesign = (BuildDisclosureLevel)(packed0 >> 4);
        BuildDisclosureLevel ui = (BuildDisclosureLevel)(packed0 & 0x0F);
        BuildDisclosureLevel universeRendering = (BuildDisclosureLevel)(packed1 >> 4);
        BuildDisclosureLevel input = (BuildDisclosureLevel)(packed1 & 0x0F);
        BuildDisclosureLevel engineControl = (BuildDisclosureLevel)(packed2 >> 4);
        BuildDisclosureLevel navigation = (BuildDisclosureLevel)(packed2 & 0x0F);
        BuildDisclosureLevel scannerControl = (BuildDisclosureLevel)(packed3 >> 4);
        BuildDisclosureLevel weaponSystems = (BuildDisclosureLevel)(packed3 & 0x0F);
        BuildDisclosureLevel resourceControl = (BuildDisclosureLevel)(packed4 >> 4);
        BuildDisclosureLevel fleetControl = (BuildDisclosureLevel)(packed4 & 0x0F);
        BuildDisclosureLevel missionControl = (BuildDisclosureLevel)(packed5 >> 4);
        BuildDisclosureLevel chat = (BuildDisclosureLevel)(packed5 & 0x0F);

        if (!IsValid(softwareDesign) || !IsValid(ui) || !IsValid(universeRendering) || !IsValid(input) ||
            !IsValid(engineControl) || !IsValid(navigation) || !IsValid(scannerControl) || !IsValid(weaponSystems) ||
            !IsValid(resourceControl) || !IsValid(fleetControl) || !IsValid(missionControl) || !IsValid(chat))
        {
            disclosure = null;
            return false;
        }

        disclosure = new BuildDisclosure(softwareDesign, ui, universeRendering, input, engineControl, navigation,
            scannerControl, weaponSystems, resourceControl, fleetControl, missionControl, chat);
        return true;
    }

    private static bool IsValid(BuildDisclosureLevel level)
    {
        return level <= BuildDisclosureLevel.AgenticTool;
    }

    private static char NibbleToHex(byte value)
    {
        return (char)(value < 10 ? '0' + value : 'A' + value - 10);
    }
}
