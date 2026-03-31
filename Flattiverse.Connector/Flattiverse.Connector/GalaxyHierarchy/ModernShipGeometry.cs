using System.Diagnostics;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

internal static class ModernShipGeometry
{
    public const float Radius = 14f;
    public const float SpeedLimit = 3.4f;
    public const float ScannerMaximumAngleOffset = 22.5f;
    public const float InterceptorMaximumAngleOffset = 45f;

    public static readonly SubsystemSlot[] EngineSlots =
    {
        SubsystemSlot.ModernEngineN,
        SubsystemSlot.ModernEngineNE,
        SubsystemSlot.ModernEngineE,
        SubsystemSlot.ModernEngineSE,
        SubsystemSlot.ModernEngineS,
        SubsystemSlot.ModernEngineSW,
        SubsystemSlot.ModernEngineW,
        SubsystemSlot.ModernEngineNW
    };

    public static readonly SubsystemSlot[] ScannerSlots =
    {
        SubsystemSlot.ModernScannerN,
        SubsystemSlot.ModernScannerNE,
        SubsystemSlot.ModernScannerE,
        SubsystemSlot.ModernScannerSE,
        SubsystemSlot.ModernScannerS,
        SubsystemSlot.ModernScannerSW,
        SubsystemSlot.ModernScannerW,
        SubsystemSlot.ModernScannerNW
    };

    public static readonly SubsystemSlot[] ShotLauncherSlots =
    {
        SubsystemSlot.StaticShotLauncherN,
        SubsystemSlot.StaticShotLauncherNE,
        SubsystemSlot.StaticShotLauncherE,
        SubsystemSlot.StaticShotLauncherSE,
        SubsystemSlot.StaticShotLauncherS,
        SubsystemSlot.StaticShotLauncherSW,
        SubsystemSlot.StaticShotLauncherW,
        SubsystemSlot.StaticShotLauncherNW
    };

    public static readonly SubsystemSlot[] ShotMagazineSlots =
    {
        SubsystemSlot.StaticShotMagazineN,
        SubsystemSlot.StaticShotMagazineNE,
        SubsystemSlot.StaticShotMagazineE,
        SubsystemSlot.StaticShotMagazineSE,
        SubsystemSlot.StaticShotMagazineS,
        SubsystemSlot.StaticShotMagazineSW,
        SubsystemSlot.StaticShotMagazineW,
        SubsystemSlot.StaticShotMagazineNW
    };

    public static readonly SubsystemSlot[] ShotFabricatorSlots =
    {
        SubsystemSlot.StaticShotFabricatorN,
        SubsystemSlot.StaticShotFabricatorNE,
        SubsystemSlot.StaticShotFabricatorE,
        SubsystemSlot.StaticShotFabricatorSE,
        SubsystemSlot.StaticShotFabricatorS,
        SubsystemSlot.StaticShotFabricatorSW,
        SubsystemSlot.StaticShotFabricatorW,
        SubsystemSlot.StaticShotFabricatorNW
    };

    public static readonly SubsystemSlot[] RailgunSlots =
    {
        SubsystemSlot.ModernRailgunN,
        SubsystemSlot.ModernRailgunNE,
        SubsystemSlot.ModernRailgunE,
        SubsystemSlot.ModernRailgunSE,
        SubsystemSlot.ModernRailgunS,
        SubsystemSlot.ModernRailgunSW,
        SubsystemSlot.ModernRailgunW,
        SubsystemSlot.ModernRailgunNW
    };

    public static bool TryGetLocalAngle(SubsystemSlot slot, out float localAngle)
    {
        switch (slot)
        {
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.ModernRailgunN:
                localAngle = 0f;
                return true;
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.ModernRailgunNE:
                localAngle = 315f;
                return true;
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.ModernRailgunE:
                localAngle = 270f;
                return true;
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.ModernRailgunSE:
                localAngle = 225f;
                return true;
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.ModernRailgunS:
                localAngle = 180f;
                return true;
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.ModernRailgunSW:
                localAngle = 135f;
                return true;
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticInterceptorLauncherW:
            case SubsystemSlot.StaticInterceptorMagazineW:
            case SubsystemSlot.StaticInterceptorFabricatorW:
            case SubsystemSlot.ModernRailgunW:
                localAngle = 90f;
                return true;
            case SubsystemSlot.ModernEngineNW:
            case SubsystemSlot.ModernScannerNW:
            case SubsystemSlot.StaticShotLauncherNW:
            case SubsystemSlot.StaticShotMagazineNW:
            case SubsystemSlot.StaticShotFabricatorNW:
            case SubsystemSlot.ModernRailgunNW:
                localAngle = 45f;
                return true;
            default:
                localAngle = 0f;
                return false;
        }
    }

    public static float NormalizeAngle(float angle)
    {
        float normalizedAngle = angle % 360f;

        if (normalizedAngle < 0f)
            normalizedAngle += 360f;

        return normalizedAngle;
    }

    public static float NormalizeSignedAngle(float angle)
    {
        float normalizedAngle = NormalizeAngle(angle + 180f) - 180f;

        if (normalizedAngle <= -180f)
            normalizedAngle += 360f;

        return normalizedAngle;
    }

    public static float GetAbsoluteAngle(float shipAngle, SubsystemSlot slot)
    {
        bool found = TryGetLocalAngle(slot, out float localAngle);

        Debug.Assert(found, $"No modern-ship angle mapping found for slot {slot}.");
        return NormalizeAngle(shipAngle + localAngle);
    }
}
