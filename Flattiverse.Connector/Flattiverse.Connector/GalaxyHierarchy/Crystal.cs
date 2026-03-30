namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// One account-wide crystal.
/// </summary>
public class Crystal
{
    private readonly string _name;
    private readonly float _hue;
    private readonly CrystalGrade _grade;
    private readonly float _energyBatteryMultiplier;
    private readonly float _ionsBatteryMultiplier;
    private readonly float _neutrinosBatteryMultiplier;
    private readonly float _hullMultiplier;
    private readonly float _shieldMultiplier;
    private readonly float _armorMultiplier;
    private readonly float _energyCellMultiplier;
    private readonly float _ionsCellMultiplier;
    private readonly float _neutrinosCellMultiplier;
    private readonly float _shotWeaponProductionMultiplier;
    private readonly float _interceptorWeaponProductionMultiplier;
    private readonly float _crystalCargoLimitMultiplier;
    private readonly bool _locked;

    internal Crystal(string name, float hue, CrystalGrade grade, float energyBatteryMultiplier, float ionsBatteryMultiplier,
        float neutrinosBatteryMultiplier, float hullMultiplier, float shieldMultiplier, float armorMultiplier,
        float energyCellMultiplier, float ionsCellMultiplier, float neutrinosCellMultiplier,
        float shotWeaponProductionMultiplier, float interceptorWeaponProductionMultiplier,
        float crystalCargoLimitMultiplier, bool locked)
    {
        _name = name;
        _hue = hue;
        _grade = grade;
        _energyBatteryMultiplier = energyBatteryMultiplier;
        _ionsBatteryMultiplier = ionsBatteryMultiplier;
        _neutrinosBatteryMultiplier = neutrinosBatteryMultiplier;
        _hullMultiplier = hullMultiplier;
        _shieldMultiplier = shieldMultiplier;
        _armorMultiplier = armorMultiplier;
        _energyCellMultiplier = energyCellMultiplier;
        _ionsCellMultiplier = ionsCellMultiplier;
        _neutrinosCellMultiplier = neutrinosCellMultiplier;
        _shotWeaponProductionMultiplier = shotWeaponProductionMultiplier;
        _interceptorWeaponProductionMultiplier = interceptorWeaponProductionMultiplier;
        _crystalCargoLimitMultiplier = crystalCargoLimitMultiplier;
        _locked = locked;
    }

    /// <summary>
    /// Crystal name within the owning account inventory.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// Hue value carried by the crystal, currently derived from harvested nebula material.
    /// </summary>
    public float Hue
    {
        get { return _hue; }
    }

    /// <summary>
    /// Quality grade of the crystal.
    /// </summary>
    public CrystalGrade Grade
    {
        get { return _grade; }
    }

    /// <summary>
    /// Effect-axis multiplier for the energy-battery stat.
    /// </summary>
    public float EnergyBatteryMultiplier
    {
        get { return _energyBatteryMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for the ion-battery stat.
    /// </summary>
    public float IonsBatteryMultiplier
    {
        get { return _ionsBatteryMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for the neutrino-battery stat.
    /// </summary>
    public float NeutrinosBatteryMultiplier
    {
        get { return _neutrinosBatteryMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for hull-related stats.
    /// </summary>
    public float HullMultiplier
    {
        get { return _hullMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for shield-related stats.
    /// </summary>
    public float ShieldMultiplier
    {
        get { return _shieldMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for armor-related stats.
    /// </summary>
    public float ArmorMultiplier
    {
        get { return _armorMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for the energy-cell stat.
    /// </summary>
    public float EnergyCellMultiplier
    {
        get { return _energyCellMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for the ion-cell stat.
    /// </summary>
    public float IonsCellMultiplier
    {
        get { return _ionsCellMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for the neutrino-cell stat.
    /// </summary>
    public float NeutrinosCellMultiplier
    {
        get { return _neutrinosCellMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for shot-weapon production.
    /// </summary>
    public float ShotWeaponProductionMultiplier
    {
        get { return _shotWeaponProductionMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for interceptor-weapon production.
    /// </summary>
    public float InterceptorWeaponProductionMultiplier
    {
        get { return _interceptorWeaponProductionMultiplier; }
    }

    /// <summary>
    /// Effect-axis multiplier for crystal-cargo capacity or efficiency.
    /// </summary>
    public float CrystalCargoLimitMultiplier
    {
        get { return _crystalCargoLimitMultiplier; }
    }

    /// <summary>
    /// Whether rename and destroy operations are currently forbidden for this crystal.
    /// </summary>
    public bool Locked
    {
        get { return _locked; }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{_name} [{_grade}] Hue={_hue:0.###}, EnergyBattery={_energyBatteryMultiplier:0.000}, IonsBattery={_ionsBatteryMultiplier:0.000}, NeutrinosBattery={_neutrinosBatteryMultiplier:0.000}, Hull={_hullMultiplier:0.000}, Shield={_shieldMultiplier:0.000}, Armor={_armorMultiplier:0.000}, EnergyCell={_energyCellMultiplier:0.000}, IonsCell={_ionsCellMultiplier:0.000}, NeutrinosCell={_neutrinosCellMultiplier:0.000}, ShotProduction={_shotWeaponProductionMultiplier:0.000}, InterceptorProduction={_interceptorWeaponProductionMultiplier:0.000}, CrystalCargo={_crystalCargoLimitMultiplier:0.000}, Locked={_locked}";
    }
}
