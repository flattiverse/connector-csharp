namespace Flattiverse.Connector.GalaxyHierarchy;

static partial class SubsystemTierCatalogs
{
    private static readonly SubsystemTierInfo[] DynamicShotLauncherTierInfos = CreateDynamicShotLauncherTierInfos();
    private static readonly SubsystemTierInfo[] StaticShotLauncherTierInfos = CreateStaticShotLauncherTierInfos();
    private static readonly SubsystemTierInfo[] DynamicShotMagazineTierInfos = CreateDynamicShotMagazineTierInfos();
    private static readonly SubsystemTierInfo[] StaticShotMagazineTierInfos = CreateStaticShotMagazineTierInfos();
    private static readonly SubsystemTierInfo[] DynamicShotFabricatorTierInfos = CreateDynamicShotFabricatorTierInfos();
    private static readonly SubsystemTierInfo[] StaticShotFabricatorTierInfos = CreateStaticShotFabricatorTierInfos();
    private static readonly SubsystemTierInfo[] DynamicInterceptorLauncherTierInfos = CreateDynamicInterceptorLauncherTierInfos();
    private static readonly SubsystemTierInfo[] StaticInterceptorLauncherTierInfos = CreateStaticInterceptorLauncherTierInfos();
    private static readonly SubsystemTierInfo[] DynamicInterceptorMagazineTierInfos = CreateDynamicInterceptorMagazineTierInfos();
    private static readonly SubsystemTierInfo[] StaticInterceptorMagazineTierInfos = CreateStaticInterceptorMagazineTierInfos();
    private static readonly SubsystemTierInfo[] DynamicInterceptorFabricatorTierInfos = CreateDynamicInterceptorFabricatorTierInfos();
    private static readonly SubsystemTierInfo[] StaticInterceptorFabricatorTierInfos = CreateStaticInterceptorFabricatorTierInfos();
    private static readonly SubsystemTierInfo[] ClassicRailgunTierInfos = CreateRailgunTierInfos(false);
    private static readonly SubsystemTierInfo[] ModernRailgunTierInfos = CreateRailgunTierInfos(true);
    private static readonly SubsystemTierInfo[] JumpDriveTierInfos = CreateJumpDriveTierInfos();

    private static SubsystemTierInfo[] CreateDynamicShotLauncherTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.DynamicShotLauncher);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.DynamicShotLauncher, "Configurable classic projectile launcher.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            const float minimumSpeed = 0.1f;
            const ushort minimumTicks = 2;
            const float minimumExplosionLoad = 1f;
            const float minimumDamage = 1f;
            ShipBalancing.GetDynamicShotLauncher(tier, out float maximumSpeed, out ushort maximumTicks, out float maximumLoad,
                out float maximumDamage, out float structureLoad);
            result[tier] = CreateTierInfo(SubsystemKind.DynamicShotLauncher, SubsystemSlot.DynamicShotLauncher, tier, structureLoad,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Base, "Base shot cost", ConstantFormula(20f), null, null, null, null,
                    null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.RelativeSpeed, "Projectile speed", LinearFormula(60f, 0f), null,
                    null, null, null, null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Ticks, "Lifetime", LinearFormula(3f, 0f), null, null, null, null,
                    null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.ExplosionLoad, "Explosion load", LinearFormula(15f, 0f), null,
                    null, null, null, null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Damage, "Damage", LinearFormula(20f, 0f), null, null, null, null,
                    null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRelativeSpeed", "Minimum projectile speed", "", minimumSpeed, minimumSpeed),
                new SubsystemPropertyInfo("maximumRelativeSpeed", "Maximum projectile speed", "", maximumSpeed, maximumSpeed),
                new SubsystemPropertyInfo("minimumTicks", "Minimum lifetime", "ticks", minimumTicks, minimumTicks),
                new SubsystemPropertyInfo("maximumTicks", "Maximum lifetime", "ticks", maximumTicks, maximumTicks),
                new SubsystemPropertyInfo("minimumExplosionLoad", "Minimum explosion load", "", minimumExplosionLoad, minimumExplosionLoad),
                new SubsystemPropertyInfo("maximumExplosionLoad", "Maximum explosion load", "", maximumLoad, maximumLoad),
                new SubsystemPropertyInfo("minimumDamage", "Minimum damage", "", minimumDamage, minimumDamage),
                new SubsystemPropertyInfo("maximumDamage", "Maximum damage", "", maximumDamage, maximumDamage)
            ], "Configurable classic projectile launcher.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateStaticShotLauncherTierInfos()
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(SubsystemSlot.StaticShotLauncherN);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(SubsystemKind.StaticShotLauncher, "Configurable modern projectile launcher.");

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            const float minimumSpeed = 0.1f;
            const ushort minimumTicks = 2;
            const float minimumExplosionLoad = 1f;
            const float minimumDamage = 1f;
            ShipBalancing.GetStaticShotLauncher(tier, out float maximumSpeed, out ushort maximumTicks, out float maximumLoad,
                out float maximumDamage, out float structureLoad);
            result[tier] = CreateTierInfo(SubsystemKind.StaticShotLauncher, SubsystemSlot.StaticShotLauncherN, tier, structureLoad,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Base, "Base shot cost", ConstantFormula(20f), null, null, null, null,
                    null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.RelativeSpeed, "Projectile speed", LinearFormula(60f, 0f), null,
                    null, null, null, null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Ticks, "Lifetime", LinearFormula(3f, 0f), null, null, null, null,
                    null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.ExplosionLoad, "Explosion load", LinearFormula(15f, 0f), null,
                    null, null, null, null, null),
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Damage, "Damage", LinearFormula(20f, 0f), null, null, null, null,
                    null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRelativeSpeed", "Minimum projectile speed", "", minimumSpeed, minimumSpeed),
                new SubsystemPropertyInfo("maximumRelativeSpeed", "Maximum projectile speed", "", maximumSpeed, maximumSpeed),
                new SubsystemPropertyInfo("minimumTicks", "Minimum lifetime", "ticks", minimumTicks, minimumTicks),
                new SubsystemPropertyInfo("maximumTicks", "Maximum lifetime", "ticks", maximumTicks, maximumTicks),
                new SubsystemPropertyInfo("minimumExplosionLoad", "Minimum explosion load", "", minimumExplosionLoad, minimumExplosionLoad),
                new SubsystemPropertyInfo("maximumExplosionLoad", "Maximum explosion load", "", maximumLoad, maximumLoad),
                new SubsystemPropertyInfo("minimumDamage", "Minimum damage", "", minimumDamage, minimumDamage),
                new SubsystemPropertyInfo("maximumDamage", "Maximum damage", "", maximumDamage, maximumDamage)
            ], "Configurable modern projectile launcher.");
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicShotMagazineTierInfos()
    {
        return CreateMagazineTierInfos(SubsystemKind.DynamicShotMagazine, SubsystemSlot.DynamicShotMagazine, false,
            "Stores classic projectile ammunition.");
    }

    private static SubsystemTierInfo[] CreateStaticShotMagazineTierInfos()
    {
        return CreateMagazineTierInfos(SubsystemKind.StaticShotMagazine, SubsystemSlot.StaticShotMagazineN, true,
            "Stores modern projectile ammunition.");
    }

    private static SubsystemTierInfo[] CreateMagazineTierInfos(SubsystemKind kind, SubsystemSlot slot, bool modern, string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumShots;
            float startingShots;
            float load;

            if (modern)
                ShipBalancing.GetStaticShotMagazine(tier, out maximumShots, out startingShots, out load);
            else
                ShipBalancing.GetDynamicShotMagazine(tier, out maximumShots, out startingShots, out load);

            result[tier] = CreateTierInfo(kind, slot, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("maximumShots", "Maximum shots", "", maximumShots, maximumShots),
                new SubsystemPropertyInfo("startingShots", "Starting shots", "", startingShots, startingShots)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicShotFabricatorTierInfos()
    {
        return CreateFabricatorTierInfos(SubsystemKind.DynamicShotFabricator, SubsystemSlot.DynamicShotFabricator, false,
            "Produces classic projectile ammunition over time.");
    }

    private static SubsystemTierInfo[] CreateStaticShotFabricatorTierInfos()
    {
        return CreateFabricatorTierInfos(SubsystemKind.StaticShotFabricator, SubsystemSlot.StaticShotFabricatorN, true,
            "Produces modern projectile ammunition over time.");
    }

    private static SubsystemTierInfo[] CreateFabricatorTierInfos(SubsystemKind kind, SubsystemSlot slot, bool modern, string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumRate;
            float fullCost;
            float load;

            if (modern)
                ShipBalancing.GetStaticShotFabricator(tier, out maximumRate, out fullCost, out load);
            else
                ShipBalancing.GetDynamicShotFabricator(tier, out maximumRate, out fullCost, out load);

            result[tier] = CreateTierInfo(kind, slot, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power",
                    GetEngineLikeFormula(fullCost), null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRate", "Minimum production rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum production rate", "/tick", maximumRate, maximumRate)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicInterceptorLauncherTierInfos()
    {
        return CreateInterceptorLauncherTierInfos(SubsystemKind.DynamicInterceptorLauncher, SubsystemSlot.DynamicInterceptorLauncher,
            false, "Classic anti-projectile interceptor launcher.");
    }

    private static SubsystemTierInfo[] CreateStaticInterceptorLauncherTierInfos()
    {
        return CreateInterceptorLauncherTierInfos(SubsystemKind.StaticInterceptorLauncher, SubsystemSlot.StaticInterceptorLauncherE, true,
            "Modern anti-projectile interceptor launcher.");
    }

    private static SubsystemTierInfo[] CreateInterceptorLauncherTierInfos(SubsystemKind kind, SubsystemSlot slot, bool modern,
        string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumSpeed;
            ushort ticks;
            float fixedLoad;
            float fixedDamage;
            float structureLoad;

            if (modern)
                ShipBalancing.GetStaticInterceptorLauncher(tier, out maximumSpeed, out ticks, out fixedLoad, out fixedDamage,
                    out structureLoad);
            else
                ShipBalancing.GetDynamicInterceptorLauncher(tier, out maximumSpeed, out ticks, out fixedLoad, out fixedDamage,
                    out structureLoad);

            result[tier] = CreateTierInfo(kind, slot, tier, structureLoad,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Base, "Base shot cost", ConstantFormula(450f * tier), null, null,
                    null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("maximumRelativeSpeed", "Maximum projectile speed", "", maximumSpeed, maximumSpeed),
                new SubsystemPropertyInfo("ticks", "Lifetime", "ticks", ticks, ticks),
                new SubsystemPropertyInfo("fixedExplosionLoad", "Fixed explosion load", "", fixedLoad, fixedLoad),
                new SubsystemPropertyInfo("fixedDamage", "Fixed damage", "", fixedDamage, fixedDamage)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicInterceptorMagazineTierInfos()
    {
        return CreateInterceptorMagazineTierInfos(SubsystemKind.DynamicInterceptorMagazine, SubsystemSlot.DynamicInterceptorMagazine, false,
            "Stores classic interceptor ammunition.");
    }

    private static SubsystemTierInfo[] CreateStaticInterceptorMagazineTierInfos()
    {
        return CreateInterceptorMagazineTierInfos(SubsystemKind.StaticInterceptorMagazine, SubsystemSlot.StaticInterceptorMagazineE, true,
            "Stores modern interceptor ammunition.");
    }

    private static SubsystemTierInfo[] CreateInterceptorMagazineTierInfos(SubsystemKind kind, SubsystemSlot slot, bool modern,
        string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumShots;
            float startingShots;
            float load;

            if (modern)
                ShipBalancing.GetStaticInterceptorMagazine(tier, out maximumShots, out startingShots, out load);
            else
                ShipBalancing.GetDynamicInterceptorMagazine(tier, out maximumShots, out startingShots, out load);

            result[tier] = CreateTierInfo(kind, slot, tier, load, System.Array.Empty<SubsystemResourceUsageFormula>(),
            [
                new SubsystemPropertyInfo("maximumShots", "Maximum shots", "", maximumShots, maximumShots),
                new SubsystemPropertyInfo("startingShots", "Starting shots", "", startingShots, startingShots)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateDynamicInterceptorFabricatorTierInfos()
    {
        return CreateInterceptorFabricatorTierInfos(SubsystemKind.DynamicInterceptorFabricator, SubsystemSlot.DynamicInterceptorFabricator,
            false, "Produces classic interceptor ammunition over time.");
    }

    private static SubsystemTierInfo[] CreateStaticInterceptorFabricatorTierInfos()
    {
        return CreateInterceptorFabricatorTierInfos(SubsystemKind.StaticInterceptorFabricator, SubsystemSlot.StaticInterceptorFabricatorE,
            true, "Produces modern interceptor ammunition over time.");
    }

    private static SubsystemTierInfo[] CreateInterceptorFabricatorTierInfos(SubsystemKind kind, SubsystemSlot slot, bool modern,
        string description)
    {
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumRate;
            float fullCost;
            float load;

            if (modern)
                ShipBalancing.GetStaticInterceptorFabricator(tier, out maximumRate, out fullCost, out load);
            else
                ShipBalancing.GetDynamicInterceptorFabricator(tier, out maximumRate, out fullCost, out load);

            result[tier] = CreateTierInfo(kind, slot, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.NormalizedPower, "Normalized power",
                    GetEngineLikeFormula(fullCost), null, null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("minimumRate", "Minimum production rate", "/tick", 0f, 0f),
                new SubsystemPropertyInfo("maximumRate", "Maximum production rate", "/tick", maximumRate, maximumRate)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateRailgunTierInfos(bool modern)
    {
        SubsystemKind kind = modern ? SubsystemKind.ModernRailgun : SubsystemKind.ClassicRailgun;
        SubsystemSlot slot = modern ? SubsystemSlot.ModernRailgunN : SubsystemSlot.Railgun;
        string description = modern ? "Modern slot-based railgun." : "Classic railgun.";
        byte maximumTier = ShipUpgradeBalancing.GetMaximumTier(slot);
        SubsystemTierInfo[] result = new SubsystemTierInfo[maximumTier + 1];
        result[0] = CreateTierZero(kind, description);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            ShipBalancing.GetRailgun(tier, modern, out float projectileSpeed, out ushort lifetime, out float energyCost,
                out float metalCost, out float load);
            result[tier] = CreateTierInfo(kind, slot, tier, load,
            [
                new SubsystemResourceUsageFormula(SubsystemComponentKind.Base, "Fire", ConstantFormula(energyCost), ConstantFormula(metalCost),
                    null, null, null, null, null)
            ],
            [
                new SubsystemPropertyInfo("projectileSpeed", "Projectile speed", "", projectileSpeed, projectileSpeed),
                new SubsystemPropertyInfo("projectileLifetime", "Projectile lifetime", "ticks", lifetime, lifetime),
                new SubsystemPropertyInfo("energyCost", "Energy cost", "", energyCost, energyCost),
                new SubsystemPropertyInfo("metalCost", "Metal cost", "", metalCost, metalCost)
            ], description);
        }

        return result;
    }

    private static SubsystemTierInfo[] CreateJumpDriveTierInfos()
    {
        SubsystemTierInfo[] result = new SubsystemTierInfo[2];
        result[0] = CreateTierZero(SubsystemKind.JumpDrive, "Allows wormhole jumps.");
        ShipBalancing.GetJumpDrive(out float energyCost, out float load);
        result[1] = CreateTierInfo(SubsystemKind.JumpDrive, SubsystemSlot.JumpDrive, 1, load,
        [
            new SubsystemResourceUsageFormula(SubsystemComponentKind.Base, "Jump", ConstantFormula(energyCost), null, null, null, null, null,
                null)
        ],
        [
            new SubsystemPropertyInfo("energyCost", "Jump energy cost", "", energyCost, energyCost)
        ], "Allows wormhole jumps.");
        return result;
    }
}
