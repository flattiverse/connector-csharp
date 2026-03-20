using System.Collections;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Session-level runtime self-disclosure.
/// </summary>
public sealed class RuntimeDisclosure : IEnumerable<KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>>
{
    internal const int NibbleCount = 10;
    internal const int ByteCount = 5;

    /// <summary>
    /// Engine-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel EngineControl;

    /// <summary>
    /// Navigation disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel Navigation;

    /// <summary>
    /// Scanner-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel ScannerControl;

    /// <summary>
    /// Weapon-aiming disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel WeaponAiming;

    /// <summary>
    /// Weapon-target-selection disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel WeaponTargetSelection;

    /// <summary>
    /// Resource-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel ResourceControl;

    /// <summary>
    /// Fleet-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel FleetControl;

    /// <summary>
    /// Mission-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel MissionControl;

    /// <summary>
    /// Loadout-control disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel LoadoutControl;

    /// <summary>
    /// Chat disclosure.
    /// </summary>
    public readonly RuntimeDisclosureLevel Chat;

    /// <summary>
    /// Creates a runtime disclosure snapshot.
    /// </summary>
    public RuntimeDisclosure(RuntimeDisclosureLevel engineControl, RuntimeDisclosureLevel navigation, RuntimeDisclosureLevel scannerControl,
        RuntimeDisclosureLevel weaponAiming, RuntimeDisclosureLevel weaponTargetSelection, RuntimeDisclosureLevel resourceControl,
        RuntimeDisclosureLevel fleetControl, RuntimeDisclosureLevel missionControl, RuntimeDisclosureLevel loadoutControl,
        RuntimeDisclosureLevel chat)
    {
        EngineControl = engineControl;
        Navigation = navigation;
        ScannerControl = scannerControl;
        WeaponAiming = weaponAiming;
        WeaponTargetSelection = weaponTargetSelection;
        ResourceControl = resourceControl;
        FleetControl = fleetControl;
        MissionControl = missionControl;
        LoadoutControl = loadoutControl;
        Chat = chat;
    }

    /// <summary>
    /// Returns the disclosed level of one runtime aspect.
    /// </summary>
    public RuntimeDisclosureLevel this[RuntimeDisclosureAspect aspect] => aspect switch
    {
        RuntimeDisclosureAspect.EngineControl => EngineControl,
        RuntimeDisclosureAspect.Navigation => Navigation,
        RuntimeDisclosureAspect.ScannerControl => ScannerControl,
        RuntimeDisclosureAspect.WeaponAiming => WeaponAiming,
        RuntimeDisclosureAspect.WeaponTargetSelection => WeaponTargetSelection,
        RuntimeDisclosureAspect.ResourceControl => ResourceControl,
        RuntimeDisclosureAspect.FleetControl => FleetControl,
        RuntimeDisclosureAspect.MissionControl => MissionControl,
        RuntimeDisclosureAspect.LoadoutControl => LoadoutControl,
        RuntimeDisclosureAspect.Chat => Chat,
        _ => throw new ArgumentOutOfRangeException(nameof(aspect), aspect, null)
    };

    /// <summary>
    /// Enumerates all disclosed runtime aspects in fixed wire order.
    /// </summary>
    public IEnumerator<KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>> GetEnumerator()
    {
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.EngineControl, EngineControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.Navigation, Navigation);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.ScannerControl, ScannerControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.WeaponAiming, WeaponAiming);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.WeaponTargetSelection, WeaponTargetSelection);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.ResourceControl, ResourceControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.FleetControl, FleetControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.MissionControl, MissionControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.LoadoutControl, LoadoutControl);
        yield return new KeyValuePair<RuntimeDisclosureAspect, RuntimeDisclosureLevel>(RuntimeDisclosureAspect.Chat, Chat);
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
            span[0] = NibbleToHex((byte)disclosure.EngineControl);
            span[1] = NibbleToHex((byte)disclosure.Navigation);
            span[2] = NibbleToHex((byte)disclosure.ScannerControl);
            span[3] = NibbleToHex((byte)disclosure.WeaponAiming);
            span[4] = NibbleToHex((byte)disclosure.WeaponTargetSelection);
            span[5] = NibbleToHex((byte)disclosure.ResourceControl);
            span[6] = NibbleToHex((byte)disclosure.FleetControl);
            span[7] = NibbleToHex((byte)disclosure.MissionControl);
            span[8] = NibbleToHex((byte)disclosure.LoadoutControl);
            span[9] = NibbleToHex((byte)disclosure.Chat);
        });
    }

    internal static bool TryRead(Flattiverse.Connector.Network.PacketReader reader, out RuntimeDisclosure? disclosure)
    {
        if (!reader.Read(out byte packed0) ||
            !reader.Read(out byte packed1) ||
            !reader.Read(out byte packed2) ||
            !reader.Read(out byte packed3) ||
            !reader.Read(out byte packed4))
        {
            disclosure = null;
            return false;
        }

        RuntimeDisclosureLevel engineControl = (RuntimeDisclosureLevel)(packed0 >> 4);
        RuntimeDisclosureLevel navigation = (RuntimeDisclosureLevel)(packed0 & 0x0F);
        RuntimeDisclosureLevel scannerControl = (RuntimeDisclosureLevel)(packed1 >> 4);
        RuntimeDisclosureLevel weaponAiming = (RuntimeDisclosureLevel)(packed1 & 0x0F);
        RuntimeDisclosureLevel weaponTargetSelection = (RuntimeDisclosureLevel)(packed2 >> 4);
        RuntimeDisclosureLevel resourceControl = (RuntimeDisclosureLevel)(packed2 & 0x0F);
        RuntimeDisclosureLevel fleetControl = (RuntimeDisclosureLevel)(packed3 >> 4);
        RuntimeDisclosureLevel missionControl = (RuntimeDisclosureLevel)(packed3 & 0x0F);
        RuntimeDisclosureLevel loadoutControl = (RuntimeDisclosureLevel)(packed4 >> 4);
        RuntimeDisclosureLevel chat = (RuntimeDisclosureLevel)(packed4 & 0x0F);

        if (!IsValid(engineControl) || !IsValid(navigation) || !IsValid(scannerControl) || !IsValid(weaponAiming) ||
            !IsValid(weaponTargetSelection) || !IsValid(resourceControl) || !IsValid(fleetControl) ||
            !IsValid(missionControl) || !IsValid(loadoutControl) || !IsValid(chat))
        {
            disclosure = null;
            return false;
        }

        disclosure = new RuntimeDisclosure(engineControl, navigation, scannerControl, weaponAiming, weaponTargetSelection,
            resourceControl, fleetControl, missionControl, loadoutControl, chat);
        return true;
    }

    private static bool IsValid(RuntimeDisclosureLevel level)
    {
        return level <= RuntimeDisclosureLevel.AiControlled;
    }

    private static char NibbleToHex(byte value)
    {
        return (char)(value < 10 ? '0' + value : 'A' + value - 10);
    }
}
