using System.Diagnostics;

namespace Flattiverse.Connector.GalaxyHierarchy;

static class ShipBalancing
{
    public static float CalculateRadius(float effectiveLoad)
    {
        return 1f + 47f * effectiveLoad / 100f;
    }

    public static float CalculateGravity(float effectiveLoad)
    {
        return 0.01f + 0.11f * effectiveLoad / 100f;
    }

    public static float CalculateClassicSpeedLimit(float effectiveLoad)
    {
        return 6f - 2f * MathF.Pow(effectiveLoad / 100f, 0.8f);
    }

    public static float CalculateModernSpeedLimit(float effectiveLoad)
    {
        return 6.5f - 2f * MathF.Pow(effectiveLoad / 100f, 0.8f);
    }

    public static float CalculateEngineEfficiency(float effectiveLoad)
    {
        return 1.2f - 0.45f * MathF.Pow(effectiveLoad / 100f, 0.85f);
    }

    public static float CalculateEngineEnergy(float value, float maximum, float fullCost)
    {
        Debug.Assert(!float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f, "Invalid engine value specified.");
        Debug.Assert(!float.IsNaN(maximum) && !float.IsInfinity(maximum) && maximum >= 0f, "Invalid engine maximum specified.");
        Debug.Assert(!float.IsNaN(fullCost) && !float.IsInfinity(fullCost) && fullCost >= 0f, "Invalid engine full cost specified.");

        if (maximum <= 0f || value <= 0f || fullCost == 0f)
            return 0f;

        float power01 = value / maximum;
        return fullCost * (0.30f * power01 + 0.70f * power01 * power01 * power01);
    }

    public static float CalculateShieldEnergy(byte tier, float rate, float maximumRate, float fullCost)
    {
        Debug.Assert(maximumRate >= 0f, "Invalid shield maximum rate specified.");

        if (rate <= 0f || maximumRate <= 0f || fullCost == 0f)
            return 0f;

        float power01 = rate / maximumRate;
        float curve = tier switch
        {
            1 => 0.70f * power01 + 0.30f * power01 * power01 * power01,
            2 => 0.55f * power01 + 0.45f * power01 * power01 * power01,
            3 => 0.40f * power01 + 0.60f * power01 * power01 * power01,
            4 => 0.46f * power01 + 0.54f * power01 * power01 * power01,
            5 => 0.52f * power01 + 0.48f * power01 * power01 * power01,
            _ => 0f
        };

        return fullCost * curve;
    }

    public static float CalculateRepairEnergy(byte tier, float rate, float maximumRate)
    {
        if (rate <= 0f || maximumRate <= 0f)
            return 0f;

        float power01 = rate / maximumRate;

        return tier switch
        {
            1 => 18f * MathF.Pow(power01, 0.35f) + 7f * power01 * power01 * power01,
            2 => 24f * MathF.Pow(power01, 0.35f) + 12f * power01 * power01 * power01,
            3 => 32f * MathF.Pow(power01, 0.35f) + 20f * power01 * power01 * power01,
            4 => 44f * MathF.Pow(power01, 0.35f) + 32f * power01 * power01 * power01,
            5 => 58f * MathF.Pow(power01, 0.35f) + 50f * power01 * power01 * power01,
            _ => 0f
        };
    }

    public static float CalculateScannerEnergy(float width, float length)
    {
        Debug.Assert(!float.IsNaN(width) && !float.IsInfinity(width) && width >= 0f, "Invalid scanner width specified.");
        Debug.Assert(!float.IsNaN(length) && !float.IsInfinity(length) && length >= 0f, "Invalid scanner length specified.");

        if (width <= 0f || length <= 0f)
            return 0f;

        float lengthCost = 0.3926f * MathF.Pow(length, 0.5f) + 2.76e-10f * length * length * length * length - 0.617f;
        float widthCost = 0.141176f * width - 0.705882f;
        float energy = lengthCost + widthCost;

        return energy > 0f ? energy : 0f;
    }

    public static float CalculateShotLaunchEnergy(float speed, ushort ticks, float load, float damage)
    {
        Debug.Assert(!float.IsNaN(speed) && !float.IsInfinity(speed) && speed >= 0f, "Invalid shot speed specified.");
        Debug.Assert(!float.IsNaN(load) && !float.IsInfinity(load) && load >= 0f, "Invalid shot load specified.");
        Debug.Assert(!float.IsNaN(damage) && !float.IsInfinity(damage) && damage >= 0f, "Invalid shot damage specified.");

        float energy = 20f + 60f * speed + 3f * ticks + 15f * load + 20f * damage;
        return !float.IsNaN(energy) && !float.IsInfinity(energy) ? energy : 0f;
    }

    public static void GetBattery(byte tier, out float maximum, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 14000f; load = 1f; return;
            case 2: maximum = 20000f; load = 2f; return;
            case 3: maximum = 27000f; load = 3f; return;
            case 4: maximum = 35000f; load = 4f; return;
            case 5: maximum = 44000f; load = 5f; return;
            case 6: maximum = 54000f; load = 6f; return;
            case 7: maximum = 65000f; load = 7f; return;
            case 8: maximum = 77000f; load = 8f; return;
            case 9: maximum = 90000f; load = 10f; return;
            default: maximum = 0f; load = 0f; return;
        }
    }

    public static void GetIonBattery(byte tier, out float maximum, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 1000f; load = 1f; return;
            case 2: maximum = 1500f; load = 2f; return;
            case 3: maximum = 2200f; load = 3f; return;
            case 4: maximum = 3200f; load = 4f; return;
            case 5: maximum = 4500f; load = 5f; return;
            default: maximum = 0f; load = 0f; return;
        }
    }

    public static void GetNeutrinoBattery(byte tier, out float maximum, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 120f; load = 1f; return;
            case 2: maximum = 200f; load = 1f; return;
            case 3: maximum = 320f; load = 2f; return;
            case 4: maximum = 500f; load = 3f; return;
            case 5: maximum = 800f; load = 4f; return;
            default: maximum = 0f; load = 0f; return;
        }
    }

    public static void GetEnergyCell(byte tier, out float efficiency, out float load)
    {
        switch (tier)
        {
            case 1: efficiency = 0.25f; load = 1f; return;
            case 2: efficiency = 0.40f; load = 2f; return;
            case 3: efficiency = 0.55f; load = 3f; return;
            case 4: efficiency = 0.75f; load = 4f; return;
            case 5: efficiency = 1.00f; load = 5f; return;
            default: efficiency = 0f; load = 0f; return;
        }
    }

    public static void GetHull(byte tier, out float maximum, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 35f; load = 2f; return;
            case 2: maximum = 50f; load = 4f; return;
            case 3: maximum = 70f; load = 6f; return;
            case 4: maximum = 90f; load = 8f; return;
            case 5: maximum = 120f; load = 12f; return;
            default: maximum = 0f; load = 0f; return;
        }
    }

    public static void GetArmor(byte tier, out float reduction, out float load)
    {
        switch (tier)
        {
            case 1: reduction = 1f; load = 1f; return;
            case 2: reduction = 2f; load = 2f; return;
            case 3: reduction = 3f; load = 3f; return;
            case 4: reduction = 4f; load = 4f; return;
            case 5: reduction = 5f; load = 5f; return;
            default: reduction = 0f; load = 0f; return;
        }
    }

    public static void GetShield(byte tier, out float maximum, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 20f; maximumRate = 0.10f; fullCost = 16f; load = 1f; return;
            case 2: maximum = 35f; maximumRate = 0.14f; fullCost = 26f; load = 2f; return;
            case 3: maximum = 50f; maximumRate = 0.18f; fullCost = 39f; load = 4f; return;
            case 4: maximum = 65f; maximumRate = 0.23f; fullCost = 58f; load = 6f; return;
            case 5: maximum = 80f; maximumRate = 0.28f; fullCost = 82f; load = 8f; return;
            default: maximum = 0f; maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetRepair(byte tier, out float maximumRate, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = 0.05f; load = 1f; return;
            case 2: maximumRate = 0.07f; load = 2f; return;
            case 3: maximumRate = 0.10f; load = 3f; return;
            case 4: maximumRate = 0.14f; load = 4f; return;
            case 5: maximumRate = 0.19f; load = 5f; return;
            default: maximumRate = 0f; load = 0f; return;
        }
    }

    public static void GetCargo(byte tier, out float maximumMetal, out float maximumCarbon, out float maximumHydrogen,
        out float maximumSilicon, out float maximumNebula, out float load)
    {
        switch (tier)
        {
            case 1: maximumMetal = 250f; maximumCarbon = 12f; maximumHydrogen = 12f; maximumSilicon = 12f; maximumNebula = 16f; load = 1f; return;
            case 2: maximumMetal = 500f; maximumCarbon = 20f; maximumHydrogen = 20f; maximumSilicon = 20f; maximumNebula = 24f; load = 2f; return;
            case 3: maximumMetal = 750f; maximumCarbon = 30f; maximumHydrogen = 30f; maximumSilicon = 30f; maximumNebula = 36f; load = 3f; return;
            case 4: maximumMetal = 1000f; maximumCarbon = 42f; maximumHydrogen = 42f; maximumSilicon = 42f; maximumNebula = 50f; load = 4f; return;
            case 5: maximumMetal = 1250f; maximumCarbon = 56f; maximumHydrogen = 56f; maximumSilicon = 56f; maximumNebula = 68f; load = 5f; return;
            default: maximumMetal = 0f; maximumCarbon = 0f; maximumHydrogen = 0f; maximumSilicon = 0f; maximumNebula = 0f; load = 0f; return;
        }
    }

    public static void GetResourceMiner(byte tier, bool modern, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = modern ? 0.0022f : 0.0020f; fullCost = 10f; load = 1f; return;
            case 2: maximumRate = modern ? 0.0033f : 0.0030f; fullCost = 14f; load = 2f; return;
            case 3: maximumRate = modern ? 0.0046f : 0.0042f; fullCost = 20f; load = 3f; return;
            case 4: maximumRate = modern ? 0.0061f : 0.0056f; fullCost = 30f; load = 4f; return;
            case 5: maximumRate = modern ? 0.0079f : 0.0072f; fullCost = 44f; load = 5f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetNebulaCollector(byte tier, bool modern, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = modern ? 0.0165f : 0.0150f; fullCost = 6f; load = 1f; return;
            case 2: maximumRate = modern ? 0.0242f : 0.0220f; fullCost = 9f; load = 2f; return;
            case 3: maximumRate = modern ? 0.0341f : 0.0310f; fullCost = 14f; load = 3f; return;
            case 4: maximumRate = modern ? 0.0462f : 0.0420f; fullCost = 22f; load = 4f; return;
            case 5: maximumRate = modern ? 0.0605f : 0.0550f; fullCost = 34f; load = 5f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetClassicEngine(byte tier, out float maximum, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximum = 0.038f; fullCost = 8f; load = 3f; return;
            case 2: maximum = 0.055f; fullCost = 13f; load = 6f; return;
            case 3: maximum = 0.073f; fullCost = 18f; load = 9f; return;
            case 4: maximum = 0.092f; fullCost = 25f; load = 12f; return;
            case 5: maximum = 0.112f; fullCost = 35f; load = 15f; return;
            default: maximum = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetModernEngine(byte tier, out float maximumThrust, out float maximumThrustChangePerTick, out float fullCost,
        out float load)
    {
        switch (tier)
        {
            case 1: maximumThrust = 0.016f; maximumThrustChangePerTick = 0.006f; fullCost = 3.8f; load = 0.50f; return;
            case 2: maximumThrust = 0.023f; maximumThrustChangePerTick = 0.009f; fullCost = 5.6f; load = 1.00f; return;
            case 3: maximumThrust = 0.031f; maximumThrustChangePerTick = 0.012f; fullCost = 7.2f; load = 1.50f; return;
            case 4: maximumThrust = 0.040f; maximumThrustChangePerTick = 0.015f; fullCost = 10f; load = 2.00f; return;
            default: maximumThrust = 0f; maximumThrustChangePerTick = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetClassicScanner(byte tier, out float maximumWidth, out float maximumLength, out float widthSpeed,
        out float lengthSpeed, out float angleSpeed, out float load)
    {
        switch (tier)
        {
            case 1: maximumWidth = 60f; maximumLength = 200f; widthSpeed = 2.5f; lengthSpeed = 10f; angleSpeed = 5f; load = 1f; return;
            case 2: maximumWidth = 90f; maximumLength = 300f; widthSpeed = 2.5f; lengthSpeed = 10f; angleSpeed = 5f; load = 2f; return;
            case 3: maximumWidth = 90f; maximumLength = 360f; widthSpeed = 2.5f; lengthSpeed = 10f; angleSpeed = 5f; load = 3f; return;
            case 4: maximumWidth = 90f; maximumLength = 430f; widthSpeed = 2.5f; lengthSpeed = 10f; angleSpeed = 5f; load = 4f; return;
            case 5: maximumWidth = 90f; maximumLength = 500f; widthSpeed = 2.5f; lengthSpeed = 10f; angleSpeed = 5f; load = 5f; return;
            default: maximumWidth = 0f; maximumLength = 0f; widthSpeed = 0f; lengthSpeed = 0f; angleSpeed = 0f; load = 0f; return;
        }
    }

    public static void GetModernScanner(byte tier, out float maximumWidth, out float maximumLength, out float widthSpeed,
        out float lengthSpeed, out float angleSpeed, out float load)
    {
        switch (tier)
        {
            case 1: maximumWidth = 35f; maximumLength = 200f; widthSpeed = 1f; lengthSpeed = 10f; angleSpeed = 5f; load = 0.20f; return;
            case 2: maximumWidth = 45f; maximumLength = 300f; widthSpeed = 1f; lengthSpeed = 10f; angleSpeed = 5f; load = 0.40f; return;
            case 3: maximumWidth = 55f; maximumLength = 360f; widthSpeed = 1f; lengthSpeed = 10f; angleSpeed = 5f; load = 0.60f; return;
            case 4: maximumWidth = 65f; maximumLength = 430f; widthSpeed = 1f; lengthSpeed = 10f; angleSpeed = 5f; load = 0.80f; return;
            case 5: maximumWidth = 75f; maximumLength = 500f; widthSpeed = 1f; lengthSpeed = 10f; angleSpeed = 5f; load = 1.00f; return;
            default: maximumWidth = 0f; maximumLength = 0f; widthSpeed = 0f; lengthSpeed = 0f; angleSpeed = 0f; load = 0f; return;
        }
    }

    public static void GetDynamicShotMagazine(byte tier, out float maximumShots, out float startingShots, out float load)
    {
        switch (tier)
        {
            case 1: maximumShots = 4f; startingShots = 2f; load = 1f; return;
            case 2: maximumShots = 8f; startingShots = 4f; load = 2f; return;
            case 3: maximumShots = 12f; startingShots = 6f; load = 4f; return;
            case 4: maximumShots = 18f; startingShots = 9f; load = 6f; return;
            case 5: maximumShots = 24f; startingShots = 12f; load = 9f; return;
            default: maximumShots = 0f; startingShots = 0f; load = 0f; return;
        }
    }

    public static void GetStaticShotMagazine(byte tier, out float maximumShots, out float startingShots, out float load)
    {
        switch (tier)
        {
            case 1: maximumShots = 1f; startingShots = 1f; load = 0.40f; return;
            case 2: maximumShots = 2f; startingShots = 1f; load = 0.70f; return;
            case 3: maximumShots = 3f; startingShots = 2f; load = 1.00f; return;
            case 4: maximumShots = 4f; startingShots = 2f; load = 1.40f; return;
            case 5: maximumShots = 5f; startingShots = 3f; load = 1.90f; return;
            default: maximumShots = 0f; startingShots = 0f; load = 0f; return;
        }
    }

    public static void GetDynamicShotFabricator(byte tier, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = 0.012f; fullCost = 8f; load = 1f; return;
            case 2: maximumRate = 0.018f; fullCost = 11f; load = 2f; return;
            case 3: maximumRate = 0.025f; fullCost = 16f; load = 3f; return;
            case 4: maximumRate = 0.033f; fullCost = 25f; load = 5f; return;
            case 5: maximumRate = 0.042f; fullCost = 39f; load = 7f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetStaticShotFabricator(byte tier, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = 0.0033f; fullCost = 2.64f; load = 0.40f; return;
            case 2: maximumRate = 0.00495f; fullCost = 3.76f; load = 0.60f; return;
            case 3: maximumRate = 0.006875f; fullCost = 5.50f; load = 0.80f; return;
            case 4: maximumRate = 0.009075f; fullCost = 8.62f; load = 1.10f; return;
            case 5: maximumRate = 0.01155f; fullCost = 13.86f; load = 1.50f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetDynamicShotLauncher(byte tier, out float maximumSpeed, out ushort maximumTicks, out float maximumLoad,
        out float maximumDamage, out float structureLoad)
    {
        switch (tier)
        {
            case 1: maximumSpeed = 2.2f; maximumTicks = 100; maximumLoad = 20f; maximumDamage = 16f; structureLoad = 2f; return;
            case 2: maximumSpeed = 2.8f; maximumTicks = 100; maximumLoad = 20f; maximumDamage = 20f; structureLoad = 4f; return;
            case 3: maximumSpeed = 3.4f; maximumTicks = 100; maximumLoad = 20f; maximumDamage = 24f; structureLoad = 6f; return;
            case 4: maximumSpeed = 4.0f; maximumTicks = 100; maximumLoad = 20f; maximumDamage = 28f; structureLoad = 8f; return;
            case 5: maximumSpeed = 4.8f; maximumTicks = 100; maximumLoad = 20f; maximumDamage = 32f; structureLoad = 10f; return;
            default: maximumSpeed = 0f; maximumTicks = 0; maximumLoad = 0f; maximumDamage = 0f; structureLoad = 0f; return;
        }
    }

    public static void GetStaticShotLauncher(byte tier, out float maximumSpeed, out ushort maximumTicks, out float maximumLoad,
        out float maximumDamage, out float structureLoad)
    {
        GetDynamicShotLauncher(tier, out maximumSpeed, out maximumTicks, out maximumLoad, out maximumDamage, out float dynamicLoad);

        structureLoad = tier switch
        {
            1 => 0.70f,
            2 => 1.40f,
            3 => 2.10f,
            4 => 2.80f,
            5 => 3.50f,
            _ => 0f
        };

        if (dynamicLoad == 0f)
            structureLoad = 0f;
    }

    public static void GetDynamicInterceptorMagazine(byte tier, out float maximumShots, out float startingShots, out float load)
    {
        switch (tier)
        {
            case 1: maximumShots = 6f; startingShots = 3f; load = 1f; return;
            case 2: maximumShots = 12f; startingShots = 6f; load = 2f; return;
            case 3: maximumShots = 20f; startingShots = 10f; load = 3f; return;
            case 4: maximumShots = 32f; startingShots = 16f; load = 4f; return;
            case 5: maximumShots = 48f; startingShots = 24f; load = 5f; return;
            default: maximumShots = 0f; startingShots = 0f; load = 0f; return;
        }
    }

    public static void GetStaticInterceptorMagazine(byte tier, out float maximumShots, out float startingShots, out float load)
    {
        switch (tier)
        {
            case 1: maximumShots = 4f; startingShots = 2f; load = 0.5f; return;
            case 2: maximumShots = 9f; startingShots = 4f; load = 1f; return;
            case 3: maximumShots = 16f; startingShots = 8f; load = 1.5f; return;
            case 4: maximumShots = 24f; startingShots = 12f; load = 2f; return;
            case 5: maximumShots = 32f; startingShots = 16f; load = 2.5f; return;
            default: maximumShots = 0f; startingShots = 0f; load = 0f; return;
        }
    }

    public static void GetDynamicInterceptorFabricator(byte tier, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = 0.010f; fullCost = 4f; load = 2f; return;
            case 2: maximumRate = 0.018f; fullCost = 7f; load = 4f; return;
            case 3: maximumRate = 0.030f; fullCost = 12f; load = 6f; return;
            case 4: maximumRate = 0.046f; fullCost = 21f; load = 8f; return;
            case 5: maximumRate = 0.068f; fullCost = 36f; load = 10f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetStaticInterceptorFabricator(byte tier, out float maximumRate, out float fullCost, out float load)
    {
        switch (tier)
        {
            case 1: maximumRate = 0.0060f; fullCost = 2.4f; load = 1f; return;
            case 2: maximumRate = 0.0108f; fullCost = 4.2f; load = 2f; return;
            case 3: maximumRate = 0.0180f; fullCost = 7.2f; load = 3f; return;
            case 4: maximumRate = 0.0276f; fullCost = 12.6f; load = 4f; return;
            case 5: maximumRate = 0.0408f; fullCost = 21.6f; load = 5f; return;
            default: maximumRate = 0f; fullCost = 0f; load = 0f; return;
        }
    }

    public static void GetDynamicInterceptorLauncher(byte tier, out float maximumSpeed, out ushort ticks, out float fixedLoad,
        out float fixedDamage, out float structureLoad)
    {
        switch (tier)
        {
            case 1: maximumSpeed = 4f; ticks = 120; fixedLoad = 25f; fixedDamage = 0f; structureLoad = 1f; return;
            case 2: maximumSpeed = 6f; ticks = 120; fixedLoad = 25f; fixedDamage = 0f; structureLoad = 2f; return;
            default: maximumSpeed = 0f; ticks = 0; fixedLoad = 0f; fixedDamage = 0f; structureLoad = 0f; return;
        }
    }

    public static void GetStaticInterceptorLauncher(byte tier, out float maximumSpeed, out ushort ticks, out float fixedLoad,
        out float fixedDamage, out float structureLoad)
    {
        GetDynamicInterceptorLauncher(tier, out maximumSpeed, out ticks, out fixedLoad, out fixedDamage, out float dynamicLoad);

        structureLoad = tier switch
        {
            1 => 0.80f,
            2 => 1.60f,
            _ => 0f
        };

        if (dynamicLoad == 0f)
            structureLoad = 0f;
    }

    public static void GetJumpDrive(out float energyCost, out float load)
    {
        energyCost = 6000f;
        load = 0f;
    }

    public static float GetStructureOptimizerReductionPercent(byte tier)
    {
        return tier switch
        {
            1 => 0.12f,
            2 => 0.20f,
            3 => 0.26f,
            4 => 0.30f,
            5 => 1f / 3f,
            _ => 0f
        };
    }

    public static void GetRailgun(byte tier, bool modern, out float projectileSpeed, out ushort lifetime, out float energyCost,
        out float metalCost, out float load)
    {
        lifetime = 250;
        energyCost = 300f;
        metalCost = 1f;

        switch (tier)
        {
            case 1: projectileSpeed = 4f; load = modern ? 1.2f : 3f; return;
            case 2: projectileSpeed = 5f; load = modern ? 1.6f : 4f; return;
            case 3: projectileSpeed = 6f; load = modern ? 2.0f : 5f; return;
            default: projectileSpeed = 0f; lifetime = 0; energyCost = 0f; metalCost = 0f; load = 0f; return;
        }
    }

    public static float CalculateRailDamage(float finalProjectileSpeed)
    {
        return 4f * finalProjectileSpeed;
    }

}
