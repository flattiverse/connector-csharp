using System.Diagnostics;
using System.Text;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Identifies the logical subsystem family independent of the concrete slot.
/// </summary>
public enum SubsystemKind : byte
{
    /// <summary>The main standard-energy battery.</summary>
    EnergyBattery,
    /// <summary>The ion battery.</summary>
    IonBattery,
    /// <summary>The neutrino battery.</summary>
    NeutrinoBattery,
    /// <summary>The standard-energy solar cell.</summary>
    EnergyCell,
    /// <summary>The ion cell.</summary>
    IonCell,
    /// <summary>The neutrino cell.</summary>
    NeutrinoCell,
    /// <summary>The hull subsystem.</summary>
    Hull,
    /// <summary>The shield subsystem.</summary>
    Shield,
    /// <summary>The armor subsystem.</summary>
    Armor,
    /// <summary>The repair subsystem.</summary>
    Repair,
    /// <summary>The cargo subsystem.</summary>
    Cargo,
    /// <summary>The resource-miner subsystem.</summary>
    ResourceMiner,
    /// <summary>The nebula-collector subsystem.</summary>
    NebulaCollector,
    /// <summary>The structure-optimizer subsystem.</summary>
    StructureOptimizer,
    /// <summary>The classic-ship engine family.</summary>
    ClassicShipEngine,
    /// <summary>The modern-ship engine family.</summary>
    ModernShipEngine,
    /// <summary>The dynamic scanner family for classic ships.</summary>
    DynamicScanner,
    /// <summary>The static scanner family for modern ships.</summary>
    StaticScanner,
    /// <summary>The classic dynamic shot launcher.</summary>
    DynamicShotLauncher,
    /// <summary>The modern static shot launcher.</summary>
    StaticShotLauncher,
    /// <summary>The classic dynamic shot magazine.</summary>
    DynamicShotMagazine,
    /// <summary>The modern static shot magazine.</summary>
    StaticShotMagazine,
    /// <summary>The classic dynamic shot fabricator.</summary>
    DynamicShotFabricator,
    /// <summary>The modern static shot fabricator.</summary>
    StaticShotFabricator,
    /// <summary>The classic dynamic interceptor launcher.</summary>
    DynamicInterceptorLauncher,
    /// <summary>The modern static interceptor launcher.</summary>
    StaticInterceptorLauncher,
    /// <summary>The classic dynamic interceptor magazine.</summary>
    DynamicInterceptorMagazine,
    /// <summary>The modern static interceptor magazine.</summary>
    StaticInterceptorMagazine,
    /// <summary>The classic dynamic interceptor fabricator.</summary>
    DynamicInterceptorFabricator,
    /// <summary>The modern static interceptor fabricator.</summary>
    StaticInterceptorFabricator,
    /// <summary>The classic railgun subsystem.</summary>
    ClassicRailgun,
    /// <summary>The modern railgun subsystem.</summary>
    ModernRailgun,
    /// <summary>The jump-drive subsystem.</summary>
    JumpDrive,
}

/// <summary>
/// Identifies one configurable or runtime-relevant subsystem component.
/// </summary>
public enum SubsystemComponentKind : byte
{
    /// <summary>A constant base term that does not depend on a configurable input.</summary>
    Base,
    /// <summary>A normalized power fraction in the range 0..1.</summary>
    NormalizedPower,
    /// <summary>The scan width component.</summary>
    Width,
    /// <summary>The scan range component.</summary>
    Range,
    /// <summary>The projectile relative-speed component.</summary>
    RelativeSpeed,
    /// <summary>The projectile lifetime-in-ticks component.</summary>
    Ticks,
    /// <summary>The projectile or explosion load component.</summary>
    ExplosionLoad,
    /// <summary>The projectile damage component.</summary>
    Damage,
}

/// <summary>
/// One concrete input value for one subsystem component, used to evaluate tier metadata formulas.
/// </summary>
public readonly struct SubsystemComponentValue
{
    private readonly SubsystemComponentKind _componentKind;
    private readonly float _value;

    /// <summary>
    /// Creates one concrete component input for formula evaluation.
    /// </summary>
    public SubsystemComponentValue(SubsystemComponentKind componentKind, float value)
    {
        _componentKind = componentKind;
        _value = value;
    }

    /// <summary>
    /// The subsystem component that receives this input value.
    /// </summary>
    public SubsystemComponentKind ComponentKind
    {
        get { return _componentKind; }
    }

    /// <summary>
    /// The numeric input value that should be inserted into the component formula.
    /// </summary>
    public float Value
    {
        get { return _value; }
    }

    /// <summary>
    /// Returns a compact textual representation such as <c>Range=300</c>.
    /// </summary>
    public override string ToString()
    {
        return $"{_componentKind}={_value:G9}";
    }
}

/// <summary>
/// Identifies one resource or energy channel used by balancing metadata.
/// </summary>
public enum Resource : byte
{
    /// <summary>Standard ship energy.</summary>
    Energy,
    /// <summary>Metal resource.</summary>
    Metal,
    /// <summary>Carbon resource.</summary>
    Carbon,
    /// <summary>Hydrogen resource.</summary>
    Hydrogen,
    /// <summary>Silicon resource.</summary>
    Silicon,
    /// <summary>Ion resource.</summary>
    Ions,
    /// <summary>Neutrino resource.</summary>
    Neutrinos,
}

/// <summary>
/// Represents f(x) = a * x^y + b * x^z + c for one resource-cost component.
/// x is the input value of the referenced subsystem component, for example normalized power,
/// scan width, scan range, projectile speed, projectile lifetime, projectile load, or damage.
/// </summary>
public sealed class ResourceFormula
{
    private readonly float _a;
    private readonly float _y;
    private readonly float _b;
    private readonly float _z;
    private readonly float _c;

    /// <summary>
    /// Creates a new polynomial-like resource formula.
    /// </summary>
    /// <param name="a">Coefficient of the first term.</param>
    /// <param name="y">Exponent of the first term.</param>
    /// <param name="b">Coefficient of the second term.</param>
    /// <param name="z">Exponent of the second term.</param>
    /// <param name="c">Constant term.</param>
    public ResourceFormula(float a, float y, float b, float z, float c)
    {
        _a = a;
        _y = y;
        _b = b;
        _z = z;
        _c = c;
    }

    /// <summary>
    /// Coefficient of the first term.
    /// </summary>
    public float A
    {
        get { return _a; }
    }

    /// <summary>
    /// Exponent of the first term.
    /// </summary>
    public float Y
    {
        get { return _y; }
    }

    /// <summary>
    /// Coefficient of the second term.
    /// </summary>
    public float B
    {
        get { return _b; }
    }

    /// <summary>
    /// Exponent of the second term.
    /// </summary>
    public float Z
    {
        get { return _z; }
    }

    /// <summary>
    /// Constant term.
    /// </summary>
    public float C
    {
        get { return _c; }
    }

    /// <summary>
    /// Evaluates the formula for the supplied input value.
    /// </summary>
    public float Evaluate(float x)
    {
        return _a * MathF.Pow(x, _y) + _b * MathF.Pow(x, _z) + _c;
    }

    /// <summary>
    /// Returns the formula as a compact algebraic expression using <c>x</c> as the variable.
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        AppendTerm(builder, _a, _y);
        AppendTerm(builder, _b, _z);
        AppendConstant(builder, _c);

        if (builder.Length == 0)
            builder.Append('0');

        return builder.ToString();
    }

    private static void AppendTerm(StringBuilder builder, float coefficient, float exponent)
    {
        if (coefficient == 0f)
            return;

        if (builder.Length > 0)
        {
            if (coefficient > 0f)
                builder.Append(" + ");
            else
                builder.Append(" - ");
        }
        else if (coefficient < 0f)
            builder.Append('-');

        float absoluteCoefficient = MathF.Abs(coefficient);
        bool printVariable = exponent != 0f;
        bool printCoefficient = !printVariable || absoluteCoefficient != 1f;

        if (printCoefficient)
            builder.Append($"{absoluteCoefficient:G9}");

        if (!printVariable)
            return;

        if (printCoefficient)
            builder.Append(" * ");

        builder.Append('x');

        if (exponent != 1f)
            builder.Append($"^{exponent:G9}");
    }

    private static void AppendConstant(StringBuilder builder, float value)
    {
        if (value == 0f)
            return;

        if (builder.Length > 0)
        {
            if (value > 0f)
                builder.Append(" + ");
            else
                builder.Append(" - ");
        }
        else if (value < 0f)
            builder.Append('-');

        builder.Append($"{MathF.Abs(value):G9}");
    }
}

/// <summary>
/// Fixed resource and time costs for upgrades, downgrades, or operating examples.
/// </summary>
public sealed class Costs
{
    private readonly int _ticks;
    private readonly float _energy;
    private readonly float _metal;
    private readonly float _carbon;
    private readonly float _hydrogen;
    private readonly float _silicon;
    private readonly float _ions;
    private readonly float _neutrinos;

    /// <summary>
    /// Creates one fixed cost vector.
    /// </summary>
    public Costs(int ticks, float energy, float metal, float carbon, float hydrogen, float silicon, float ions, float neutrinos)
    {
        _ticks = ticks;
        _energy = energy;
        _metal = metal;
        _carbon = carbon;
        _hydrogen = hydrogen;
        _silicon = silicon;
        _ions = ions;
        _neutrinos = neutrinos;
    }

    /// <summary>
    /// Required time in server ticks.
    /// </summary>
    public int Ticks
    {
        get { return _ticks; }
    }

    /// <summary>
    /// Required standard energy.
    /// </summary>
    public float Energy
    {
        get { return _energy; }
    }

    /// <summary>
    /// Required metal.
    /// </summary>
    public float Metal
    {
        get { return _metal; }
    }

    /// <summary>
    /// Required carbon.
    /// </summary>
    public float Carbon
    {
        get { return _carbon; }
    }

    /// <summary>
    /// Required hydrogen.
    /// </summary>
    public float Hydrogen
    {
        get { return _hydrogen; }
    }

    /// <summary>
    /// Required silicon.
    /// </summary>
    public float Silicon
    {
        get { return _silicon; }
    }

    /// <summary>
    /// Required ions.
    /// </summary>
    public float Ions
    {
        get { return _ions; }
    }

    /// <summary>
    /// Required neutrinos.
    /// </summary>
    public float Neutrinos
    {
        get { return _neutrinos; }
    }

    /// <summary>
    /// True when at least one cost component is non-zero.
    /// </summary>
    public bool Any
    {
        get
        {
            return _ticks != 0 || _energy != 0f || _metal != 0f || _carbon != 0f || _hydrogen != 0f || _silicon != 0f || _ions != 0f
                || _neutrinos != 0f;
        }
    }

    /// <summary>
    /// Returns the component-wise sum of this cost vector and another one.
    /// </summary>
    public Costs Add(Costs other)
    {
        return new Costs(_ticks + other._ticks, _energy + other._energy, _metal + other._metal, _carbon + other._carbon,
            _hydrogen + other._hydrogen, _silicon + other._silicon, _ions + other._ions, _neutrinos + other._neutrinos);
    }

    /// <summary>
    /// Returns a compact textual representation of all non-zero cost components.
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        Append(builder, "Ticks", _ticks);
        Append(builder, "Energy", _energy);
        Append(builder, "Metal", _metal);
        Append(builder, "Carbon", _carbon);
        Append(builder, "Hydrogen", _hydrogen);
        Append(builder, "Silicon", _silicon);
        Append(builder, "Ions", _ions);
        Append(builder, "Neutrinos", _neutrinos);

        if (builder.Length == 0)
            return "0";

        return builder.ToString();
    }

    private static void Append(StringBuilder builder, string label, float value)
    {
        if (value == 0f)
            return;

        if (builder.Length > 0)
            builder.Append(", ");

        builder.Append(label);
        builder.Append('=');
        builder.Append($"{value:G9}");
    }

    private static void Append(StringBuilder builder, string label, int value)
    {
        if (value == 0)
            return;

        if (builder.Length > 0)
            builder.Append(", ");

        builder.Append(label);
        builder.Append('=');
        builder.Append(value);
    }
}

/// <summary>
/// Numeric property metadata for one subsystem tier.
/// </summary>
public sealed class SubsystemPropertyInfo
{
    private readonly string _key;
    private readonly string _label;
    private readonly string _unit;
    private readonly float _minimumValue;
    private readonly float _maximumValue;

    /// <summary>
    /// Creates one numeric subsystem property descriptor.
    /// </summary>
    public SubsystemPropertyInfo(string key, string label, string unit, float minimumValue, float maximumValue)
    {
        _key = key;
        _label = label;
        _unit = unit;
        _minimumValue = minimumValue;
        _maximumValue = maximumValue;
    }

    /// <summary>
    /// Stable machine-readable property key.
    /// </summary>
    public string Key
    {
        get { return _key; }
    }

    /// <summary>
    /// Human-readable property label.
    /// </summary>
    public string Label
    {
        get { return _label; }
    }

    /// <summary>
    /// Display unit for the numeric value or range.
    /// </summary>
    public string Unit
    {
        get { return _unit; }
    }

    /// <summary>
    /// Lower bound of the property value for this tier.
    /// Equal to <see cref="MaximumValue"/> for exact values.
    /// </summary>
    public float MinimumValue
    {
        get { return _minimumValue; }
    }

    /// <summary>
    /// Upper bound of the property value for this tier.
    /// Equal to <see cref="MinimumValue"/> for exact values.
    /// </summary>
    public float MaximumValue
    {
        get { return _maximumValue; }
    }

    /// <summary>
    /// Returns the property as either a single value or a range.
    /// </summary>
    public override string ToString()
    {
        if (_minimumValue == _maximumValue)
            return string.IsNullOrEmpty(_unit) ? $"{_label}={_minimumValue:G9}" : $"{_label}={_minimumValue:G9} {_unit}";

        return string.IsNullOrEmpty(_unit)
            ? $"{_label}={_minimumValue:G9}..{_maximumValue:G9}"
            : $"{_label}={_minimumValue:G9}..{_maximumValue:G9} {_unit}";
    }
}

/// <summary>
/// Bundles all resource formulas that contribute to one subsystem component.
/// </summary>
public sealed class SubsystemResourceUsageFormula
{
    private readonly SubsystemComponentKind _componentKind;
    private readonly string _label;
    private readonly ResourceFormula? _energy;
    private readonly ResourceFormula? _metal;
    private readonly ResourceFormula? _carbon;
    private readonly ResourceFormula? _hydrogen;
    private readonly ResourceFormula? _silicon;
    private readonly ResourceFormula? _ions;
    private readonly ResourceFormula? _neutrinos;

    /// <summary>
    /// Creates a grouped resource-usage formula for one subsystem component.
    /// </summary>
    public SubsystemResourceUsageFormula(SubsystemComponentKind componentKind, string label, ResourceFormula? energy,
        ResourceFormula? metal, ResourceFormula? carbon, ResourceFormula? hydrogen, ResourceFormula? silicon, ResourceFormula? ions,
        ResourceFormula? neutrinos)
    {
        _componentKind = componentKind;
        _label = label;
        _energy = energy;
        _metal = metal;
        _carbon = carbon;
        _hydrogen = hydrogen;
        _silicon = silicon;
        _ions = ions;
        _neutrinos = neutrinos;
    }

    /// <summary>
    /// The subsystem component that drives these costs.
    /// </summary>
    public SubsystemComponentKind ComponentKind
    {
        get { return _componentKind; }
    }

    /// <summary>
    /// Human-readable label for the component contribution.
    /// </summary>
    public string Label
    {
        get { return _label; }
    }

    /// <summary>
    /// Formula for standard energy or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Energy
    {
        get { return _energy; }
    }

    /// <summary>
    /// Formula for metal or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Metal
    {
        get { return _metal; }
    }

    /// <summary>
    /// Formula for carbon or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Carbon
    {
        get { return _carbon; }
    }

    /// <summary>
    /// Formula for hydrogen or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Hydrogen
    {
        get { return _hydrogen; }
    }

    /// <summary>
    /// Formula for silicon or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Silicon
    {
        get { return _silicon; }
    }

    /// <summary>
    /// Formula for ions or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Ions
    {
        get { return _ions; }
    }

    /// <summary>
    /// Formula for neutrinos or <see langword="null"/> when not used.
    /// </summary>
    public ResourceFormula? Neutrinos
    {
        get { return _neutrinos; }
    }

    /// <summary>
    /// Evaluates all configured resource formulas for the supplied component value.
    /// </summary>
    public Costs CalculateCosts(float x)
    {
        return new Costs(0, Evaluate(_energy, x), Evaluate(_metal, x), Evaluate(_carbon, x), Evaluate(_hydrogen, x), Evaluate(_silicon, x),
            Evaluate(_ions, x), Evaluate(_neutrinos, x));
    }

    /// <summary>
    /// Returns a compact textual representation of all configured resource formulas.
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        Append(builder, "Energy", _energy);
        Append(builder, "Metal", _metal);
        Append(builder, "Carbon", _carbon);
        Append(builder, "Hydrogen", _hydrogen);
        Append(builder, "Silicon", _silicon);
        Append(builder, "Ions", _ions);
        Append(builder, "Neutrinos", _neutrinos);

        if (builder.Length == 0)
            return $"{_label}: 0";

        return $"{_label}: {builder}";
    }

    private static float Evaluate(ResourceFormula? formula, float x)
    {
        return formula is null ? 0f : formula.Evaluate(x);
    }

    private static void Append(StringBuilder builder, string label, ResourceFormula? formula)
    {
        if (formula is null)
            return;

        if (builder.Length > 0)
            builder.Append(", ");

        builder.Append(label);
        builder.Append('=');
        builder.Append(formula);
    }
}

/// <summary>
/// Absolute balancing snapshot for one concrete subsystem tier.
/// UpgradeCost describes the step from tier-1 to this tier.
/// DowngradeCost describes the step from this tier to tier-1.
/// </summary>
public sealed class SubsystemTierInfo
{
    private readonly SubsystemKind _subsystemKind;
    private readonly int _tier;
    private readonly float _structuralLoad;
    private readonly SubsystemResourceUsageFormula[] _resourceUsages;
    private readonly Costs _upgradeCost;
    private readonly Costs _downgradeCost;
    private readonly SubsystemPropertyInfo[] _properties;
    private readonly string _description;

    /// <summary>
    /// Creates one absolute metadata snapshot for one concrete subsystem tier.
    /// </summary>
    public SubsystemTierInfo(SubsystemKind subsystemKind, int tier, float structuralLoad, SubsystemResourceUsageFormula[] resourceUsages,
        Costs upgradeCost, Costs downgradeCost, SubsystemPropertyInfo[] properties, string description)
    {
        Debug.Assert(tier >= 0, "Tier must not be negative.");
        Debug.Assert(structuralLoad >= 0f, "Structural load must not be negative.");

        _subsystemKind = subsystemKind;
        _tier = tier;
        _structuralLoad = structuralLoad;
        _resourceUsages = resourceUsages;
        _upgradeCost = upgradeCost;
        _downgradeCost = downgradeCost;
        _properties = properties;
        _description = description;
    }

    /// <summary>
    /// Logical subsystem family described by this tier snapshot.
    /// </summary>
    public SubsystemKind SubsystemKind
    {
        get { return _subsystemKind; }
    }

    /// <summary>
    /// Concrete tier index. Tier 0 always means not installed.
    /// </summary>
    public int Tier
    {
        get { return _tier; }
    }

    /// <summary>
    /// Structural load added by installing this tier.
    /// </summary>
    public float StructuralLoad
    {
        get { return _structuralLoad; }
    }

    /// <summary>
    /// Resource-usage formulas grouped by subsystem component.
    /// </summary>
    public IReadOnlyList<SubsystemResourceUsageFormula> ResourceUsages
    {
        get { return _resourceUsages; }
    }

    /// <summary>
    /// Cost of upgrading from the previous tier to this tier.
    /// </summary>
    public Costs UpgradeCost
    {
        get { return _upgradeCost; }
    }

    /// <summary>
    /// Cost of downgrading from this tier to the previous tier.
    /// </summary>
    public Costs DowngradeCost
    {
        get { return _downgradeCost; }
    }

    /// <summary>
    /// Numeric property limits and exact values exposed by this tier.
    /// </summary>
    public IReadOnlyList<SubsystemPropertyInfo> Properties
    {
        get { return _properties; }
    }

    /// <summary>
    /// Human-readable description of this tier and its qualitative behavior.
    /// </summary>
    public string Description
    {
        get { return _description; }
    }

    /// <summary>
    /// Evaluates all resource-usage formulas for the supplied component inputs and returns their summed costs.
    /// Unspecified components contribute nothing.
    /// </summary>
    /// <exception cref="DuplicateSubsystemComponentValueGameException">
    /// Thrown when the same <see cref="SubsystemComponentKind" /> is supplied more than once.
    /// </exception>
    public Costs CalculateResourceUsage(params SubsystemComponentValue[] componentValues)
    {
        Costs result = new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

        for (int leftIndex = 0; leftIndex < componentValues.Length; leftIndex++)
            for (int rightIndex = leftIndex + 1; rightIndex < componentValues.Length; rightIndex++)
                if (componentValues[leftIndex].ComponentKind == componentValues[rightIndex].ComponentKind)
                    throw new DuplicateSubsystemComponentValueGameException(componentValues[leftIndex].ComponentKind);

        for (int componentIndex = 0; componentIndex < componentValues.Length; componentIndex++)
            for (int usageIndex = 0; usageIndex < _resourceUsages.Length; usageIndex++)
                if (_resourceUsages[usageIndex].ComponentKind == componentValues[componentIndex].ComponentKind)
                    result = result.Add(_resourceUsages[usageIndex].CalculateCosts(componentValues[componentIndex].Value));

        return result;
    }

    /// <summary>
    /// Looks up one property by its stable key.
    /// </summary>
    public bool TryGetProperty(string key, out SubsystemPropertyInfo? property)
    {
        for (int index = 0; index < _properties.Length; index++)
            if (string.Equals(_properties[index].Key, key, StringComparison.Ordinal))
            {
                property = _properties[index];
                return true;
            }

        property = null;
        return false;
    }

    /// <summary>
    /// Calculates the resulting ship radius for the supplied effective structural load.
    /// </summary>
    public static float CalculateRadius(float effectiveStructuralLoad)
    {
        return ShipBalancing.CalculateRadius(effectiveStructuralLoad);
    }

    /// <summary>
    /// Calculates the resulting ship gravity for the supplied effective structural load.
    /// </summary>
    public static float CalculateGravity(float effectiveStructuralLoad)
    {
        return ShipBalancing.CalculateGravity(effectiveStructuralLoad);
    }

    /// <summary>
    /// Calculates the classic-ship speed limit for the supplied effective structural load.
    /// </summary>
    public static float CalculateClassicSpeedLimit(float effectiveStructuralLoad)
    {
        return ShipBalancing.CalculateClassicSpeedLimit(effectiveStructuralLoad);
    }

    /// <summary>
    /// Calculates the modern-ship speed limit for the supplied effective structural load.
    /// </summary>
    public static float CalculateModernSpeedLimit(float effectiveStructuralLoad)
    {
        return ShipBalancing.CalculateModernSpeedLimit(effectiveStructuralLoad);
    }

    /// <summary>
    /// Calculates the engine-efficiency multiplier for the supplied effective structural load.
    /// </summary>
    public static float CalculateEngineEfficiency(float effectiveStructuralLoad)
    {
        return ShipBalancing.CalculateEngineEfficiency(effectiveStructuralLoad);
    }

    /// <summary>
    /// Returns a compact textual summary of the tier.
    /// </summary>
    public override string ToString()
    {
        return $"{_subsystemKind} T{_tier}: StructuralLoad={_structuralLoad:G9}, UpgradeCost=({_upgradeCost}), DowngradeCost=({_downgradeCost})";
    }
}
