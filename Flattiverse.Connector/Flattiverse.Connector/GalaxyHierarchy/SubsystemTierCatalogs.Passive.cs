namespace Flattiverse.Connector.GalaxyHierarchy;

static partial class SubsystemTierCatalogs
{
    private delegate void BatteryGetter(byte tier, out float maximum, out float load);

    private static readonly SubsystemTierInfo[] EnergyBatteryTierInfos = CreateBatteryTierInfos(SubsystemKind.EnergyBattery,
        SubsystemSlot.EnergyBattery, GetEnergyBatteryValues, "Stores regular ship energy.");
    private static readonly SubsystemTierInfo[] IonBatteryTierInfos = CreateBatteryTierInfos(SubsystemKind.IonBattery,
        SubsystemSlot.IonBattery, GetIonBatteryValues, "Stores ions for advanced subsystems.");
    private static readonly SubsystemTierInfo[] NeutrinoBatteryTierInfos = CreateBatteryTierInfos(SubsystemKind.NeutrinoBattery,
        SubsystemSlot.NeutrinoBattery, GetNeutrinoBatteryValues, "Stores neutrinos for late-game subsystems.");
    private static readonly SubsystemTierInfo[] EnergyCellTierInfos = CreateCellTierInfos(SubsystemKind.EnergyCell,
        SubsystemSlot.EnergyCell, "Collects regular energy from stars.");
    private static readonly SubsystemTierInfo[] IonCellTierInfos = CreateCellTierInfos(SubsystemKind.IonCell,
        SubsystemSlot.IonCell, "Collects ions from ion sources.");
    private static readonly SubsystemTierInfo[] NeutrinoCellTierInfos = CreateCellTierInfos(SubsystemKind.NeutrinoCell,
        SubsystemSlot.NeutrinoCell, "Collects neutrinos from neutrino sources.");
    private static readonly SubsystemTierInfo[] HullTierInfos = CreateHullTierInfos();
    private static readonly SubsystemTierInfo[] ShieldTierInfos = CreateShieldTierInfos();
    private static readonly SubsystemTierInfo[] ArmorTierInfos = CreateArmorTierInfos();
    private static readonly SubsystemTierInfo[] RepairTierInfos = CreateRepairTierInfos();
    private static readonly SubsystemTierInfo[] CargoTierInfos = CreateCargoTierInfos();
    private static readonly SubsystemTierInfo[] StructureOptimizerTierInfos = CreateStructureOptimizerTierInfos();

    private static SubsystemTierInfo[] CreateBatteryTierInfos(SubsystemKind kind, SubsystemSlot slot, BatteryGetter getter,
        string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            getter(tier, out float maximum, out float load);
            result[tier] = CreateTierInfo(kind, slot, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("maximum", "Maximum capacity", "", maximum, maximum)
            ], description);
        }

        return result;
    }

    private static void GetEnergyBatteryValues(byte tier, out float maximum, out float load)
    {
        ShipBalancing.GetBattery(tier, out maximum, out load);
    }

    private static void GetIonBatteryValues(byte tier, out float maximum, out float load)
    {
        ShipBalancing.GetIonBattery(tier, out maximum, out load);
    }

    private static void GetNeutrinoBatteryValues(byte tier, out float maximum, out float load)
    {
        ShipBalancing.GetNeutrinoBattery(tier, out maximum, out load);
    }

    private static SubsystemTierInfo[] CreateCellTierInfos(SubsystemKind kind, SubsystemSlot slot, string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetEnergyCell(tier, out float efficiency, out float load);
            result[tier] = CreateTierInfo(kind, slot, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("efficiency", "Efficiency", "", efficiency, efficiency)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateHullTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.Hull);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.Hull, "Defines hull hit points.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetHull(tier, out float maximum, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.Hull, SubsystemSlot.Hull, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("maximum", "Maximum hull", "", maximum, maximum)
            ], "Defines hull hit points.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateShieldTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.Shield);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.Shield, "Stores damage in a rechargeable shield layer.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetShield(tier, out float maximum, out float maximumRate, out float fullCost, out float load);
            SubsystemResourceUsageFormula[] usages = tier == 5 ?
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", null, null, null, null, null,
                    new ResourceFormula(0.9f, 1f, 0f, 1f, 0f), null)
            ] :
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetShieldFormula(tier, fullCost),
                    null, null, null, null, null, null)
            ];

            result[tier] = CreateTierInfo(SubsystemKind.Shield, SubsystemSlot.Shield, tier, load, usages,
            [
                new SubsystemPropertyInfo("maximum", "Maximum shield", "", maximum, maximum),
                new SubsystemPropertyInfo("minimumRate", "Minimum charge rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum charge rate", "/tick", maximumRate, maximumRate)
            ], "Stores damage in a rechargeable shield layer.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateArmorTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.Armor);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.Armor, "Reduces incoming hull damage.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetArmor(tier, out float reduction, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.Armor, SubsystemSlot.Armor, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("reduction", "Damage reduction", "", reduction, reduction)
            ], "Reduces incoming hull damage.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateRepairTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.Repair);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.Repair, "Repairs hull while the ship is almost stationary.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetRepair(tier, out float maximumRate, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.Repair, SubsystemSlot.Repair, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power", GetRepairFormula(tier), null,
                    null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRate", "Minimum repair rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum repair rate", "/tick", maximumRate, maximumRate)
            ], "Repairs hull while the ship is almost stationary.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateCargoTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.Cargo);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.Cargo, "Stores resources and nebula cargo.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetCargo(tier, out float maximumMetal, out float maximumCarbon, out float maximumHydrogen, out float maximumSilicon,
                out float maximumNebula, out float load);
            result[tier] = CreateTierInfo(SubsystemKind.Cargo, SubsystemSlot.Cargo, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("maximumMetal", "Maximum metal", "", maximumMetal, maximumMetal),
                new SubsystemPropertyInfo("maximumCarbon", "Maximum carbon", "", maximumCarbon, maximumCarbon),
                new SubsystemPropertyInfo("maximumHydrogen", "Maximum hydrogen", "", maximumHydrogen, maximumHydrogen),
                new SubsystemPropertyInfo("maximumSilicon", "Maximum silicon", "", maximumSilicon, maximumSilicon),
                new SubsystemPropertyInfo("maximumNebula", "Maximum nebula", "", maximumNebula, maximumNebula)
            ], "Stores resources and nebula cargo.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateStructureOptimizerTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.StructureOptimizer);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.StructureOptimizer, "Passively reduces effective structural load.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float reductionPercent = ShipBalancing.GetStructureOptimizerReductionPercent(tier);
            result[tier] = CreateTierInfo(SubsystemKind.StructureOptimizer, SubsystemSlot.StructureOptimizer, tier, 0f,
                System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("reductionPercent", "Structural load reduction", "", reductionPercent, reductionPercent)
            ], "Passively reduces effective structural load.");
        }

        return result;
    }
}
