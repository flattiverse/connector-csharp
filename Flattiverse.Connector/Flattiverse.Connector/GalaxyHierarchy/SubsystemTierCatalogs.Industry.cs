namespace Flattiverse.Connector.GalaxyHierarchy;

static partial class SubsystemTierCatalogs
{
    private static readonly SubsystemTierInfo[] ResourceMinerTierInfos = CreateResourceMinerTierInfos(false);
    private static readonly SubsystemTierInfo[] ModernResourceMinerTierInfos = CreateResourceMinerTierInfos(true);
    private static readonly SubsystemTierInfo[] NebulaCollectorTierInfos = CreateNebulaCollectorTierInfos(false);
    private static readonly SubsystemTierInfo[] ModernNebulaCollectorTierInfos = CreateNebulaCollectorTierInfos(true);
    private static readonly SubsystemTierInfo[] ClassicEngineTierInfos = CreateClassicEngineTierInfos();
    private static readonly SubsystemTierInfo[] ModernEngineTierInfos = CreateModernEngineTierInfos();
    private static readonly SubsystemTierInfo[] DynamicScannerTierInfos = CreateDynamicScannerTierInfos();
    private static readonly SubsystemTierInfo[] StaticScannerTierInfos = CreateStaticScannerTierInfos();

    private static SubsystemTierInfo[] CreateResourceMinerTierInfos(bool modern)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.ResourceMiner);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.ResourceMiner, "Mines standard asteroid resources.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetResourceMiner(tier, modern, out float maximumRate, out float fullCost, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.ResourceMiner, SubsystemSlot.ResourceMiner, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetEngineLikeFormula(fullCost),
                    null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRate", "Minimum mining rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum mining rate", "/tick", maximumRate, maximumRate)
            ], "Mines standard asteroid resources.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateNebulaCollectorTierInfos(bool modern)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.NebulaCollector);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.NebulaCollector, "Collects nebula matter at direct contact.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetNebulaCollector(tier, modern, out float maximumRate, out float fullCost, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.NebulaCollector, SubsystemSlot.NebulaCollector, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetEngineLikeFormula(fullCost),
                    null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRate", "Minimum collection rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum collection rate", "/tick", maximumRate, maximumRate)
            ], "Collects nebula matter at direct contact.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateClassicEngineTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.PrimaryEngine);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.ClassicShipEngine, "Provides thrust for classic ships.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetClassicEngine(tier, out float maximum, out float fullCost, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.ClassicShipEngine, SubsystemSlot.PrimaryEngine, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetEngineLikeFormula(fullCost),
                    null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("maximumThrust", "Maximum thrust", "/tick", maximum, maximum)
            ], "Provides thrust for classic ships.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateModernEngineTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.ModernEngineN);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.ModernShipEngine, "Provides thrust for one modern engine slot.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetModernEngine(tier, out float maximumThrust, out float maximumThrustChangePerTick, out float fullCost,
                out float load);
            result[tier] = CreateTierInfo(SubsystemKind.ModernShipEngine, SubsystemSlot.ModernEngineN, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetEngineLikeFormula(fullCost),
                    null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("maximumThrust", "Maximum thrust", "/tick", maximumThrust, maximumThrust),
                new SubsystemPropertyInfo("maximumThrustChangePerTick", "Maximum thrust change", "/tick", maximumThrustChangePerTick,
                    maximumThrustChangePerTick)
            ], "Provides thrust for one modern engine slot.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicScannerTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.PrimaryScanner);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.DynamicScanner, "Classic directional scanner.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetClassicScanner(tier, out float maximumWidth, out float maximumLength, out float widthSpeed,
                out float lengthSpeed, out float angleSpeed, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.DynamicScanner, SubsystemSlot.PrimaryScanner, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Width, "Scan width", tier == 5 ? null : GetScannerWidthFormula(),
                    null, null, null, null, null, tier == 5 ? new ResourceFormula(0.00141176f, 1f, 0f, 0f, -0.00705882f) : null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Range, "Scan range", tier == 5 ? null : GetScannerRangeFormula(),
                    null, null, null, null, null, tier == 5 ? new ResourceFormula(0.003926f, 0.5f, 2.76e-12f, 4f, -0.00617f) : null)
            ],
            [
                new SubsystemPropertyInfo("maximumWidth", "Maximum width", "deg", maximumWidth, maximumWidth),
                new SubsystemPropertyInfo("maximumRange", "Maximum range", "", maximumLength, maximumLength),
                new SubsystemPropertyInfo("widthSpeed", "Width speed", "/tick", widthSpeed, widthSpeed),
                new SubsystemPropertyInfo("rangeSpeed", "Range speed", "/tick", lengthSpeed, lengthSpeed),
                new SubsystemPropertyInfo("angleSpeed", "Angle speed", "deg/tick", angleSpeed, angleSpeed)
            ], "Classic directional scanner.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateStaticScannerTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.ModernScannerN);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.StaticScanner, "Modern slot-based scanner.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetModernScanner(tier, out float maximumWidth, out float maximumLength, out float widthSpeed,
                out float lengthSpeed, out float angleSpeed, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.StaticScanner, SubsystemSlot.ModernScannerN, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Width, "Scan width", tier == 5 ? null : GetScannerWidthFormula(),
                    null, null, null, null, null, tier == 5 ? new ResourceFormula(0.00141176f, 1f, 0f, 0f, -0.00705882f) : null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Range, "Scan range", tier == 5 ? null : GetScannerRangeFormula(),
                    null, null, null, null, null, tier == 5 ? new ResourceFormula(0.003926f, 0.5f, 2.76e-12f, 4f, -0.00617f) : null)
            ],
            [
                new SubsystemPropertyInfo("maximumWidth", "Maximum width", "deg", maximumWidth, maximumWidth),
                new SubsystemPropertyInfo("maximumRange", "Maximum range", "", maximumLength, maximumLength),
                new SubsystemPropertyInfo("widthSpeed", "Width speed", "/tick", widthSpeed, widthSpeed),
                new SubsystemPropertyInfo("rangeSpeed", "Range speed", "/tick", lengthSpeed, lengthSpeed),
                new SubsystemPropertyInfo("angleSpeed", "Angle speed", "deg/tick", angleSpeed, angleSpeed)
            ], "Modern slot-based scanner.");
        }

        return result;
    }
}
