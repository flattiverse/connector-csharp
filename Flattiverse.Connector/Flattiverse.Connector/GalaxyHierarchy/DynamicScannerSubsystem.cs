using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Represents a persistent dynamic scanner subsystem configuration.
/// </summary>
public class DynamicScannerSubsystem : Subsystem
{
    private const float EnergyScale = 0.000282943f;
    private const float MinimumWidthValue = 5f;
    private const float MinimumLengthValue = 20f;

    private readonly byte _id;
    private readonly float _maximumWidth;
    private readonly float _maximumLength;
    private readonly float _widthSpeed;
    private readonly float _lengthSpeed;
    private readonly float _angleSpeed;

    private float _currentWidth;
    private float _currentLength;
    private float _currentAngle;

    private float _targetWidth;
    private float _targetLength;
    private float _targetAngle;

    private bool _active;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal DynamicScannerSubsystem(Controllable controllable, string name, byte id, bool exists, float maximumWidth, float maximumLength,
        float widthSpeed, float lengthSpeed, float angleSpeed, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _id = id;
        _maximumWidth = exists ? maximumWidth : 0f;
        _maximumLength = exists ? maximumLength : 0f;
        _widthSpeed = exists ? widthSpeed : 0f;
        _lengthSpeed = exists ? lengthSpeed : 0f;
        _angleSpeed = exists ? angleSpeed : 0f;

        ResetRuntime();
    }

    internal static DynamicScannerSubsystem CreateClassicShipPrimaryScanner(Controllable controllable)
    {
        return new DynamicScannerSubsystem(controllable, "MainScanner", 0, true, 90f, 300f, 2.5f, 10f, 5f, SubsystemSlot.PrimaryScanner);
    }

    internal static DynamicScannerSubsystem CreateClassicShipSecondaryScanner(Controllable controllable)
    {
        return new DynamicScannerSubsystem(controllable, "SecondaryScanner", 1, false, 0f, 0f, 0f, 0f, 0f, SubsystemSlot.SecondaryScanner);
    }

    /// <summary>
    /// The minimum configurable scan width in degree.
    /// </summary>
    public float MinimumWidth
    {
        get { return MinimumWidthValue; }
    }

    /// <summary>
    /// The minimum configurable scan length.
    /// </summary>
    public float MinimumLength
    {
        get { return MinimumLengthValue; }
    }

    /// <summary>
    /// The maximum configurable scan width in degree.
    /// </summary>
    public float MaximumWidth
    {
        get { return _maximumWidth; }
    }

    /// <summary>
    /// The maximum configurable scan length.
    /// </summary>
    public float MaximumLength
    {
        get { return _maximumLength; }
    }

    /// <summary>
    /// The maximum width change per tick in degree.
    /// </summary>
    public float WidthSpeed
    {
        get { return _widthSpeed; }
    }

    /// <summary>
    /// The maximum length change per tick.
    /// </summary>
    public float LengthSpeed
    {
        get { return _lengthSpeed; }
    }

    /// <summary>
    /// The maximum angle change per tick in degree.
    /// </summary>
    public float AngleSpeed
    {
        get { return _angleSpeed; }
    }

    /// <summary>
    /// The currently configured scan width in degree.
    /// </summary>
    public float CurrentWidth
    {
        get { return _currentWidth; }
    }

    /// <summary>
    /// The currently configured scan length.
    /// </summary>
    public float CurrentLength
    {
        get { return _currentLength; }
    }

    /// <summary>
    /// The currently configured absolute scan center angle in degree.
    /// </summary>
    public float CurrentAngle
    {
        get { return _currentAngle; }
    }

    /// <summary>
    /// The target scan width in degree.
    /// </summary>
    public float TargetWidth
    {
        get { return _targetWidth; }
    }

    /// <summary>
    /// The target scan length.
    /// </summary>
    public float TargetLength
    {
        get { return _targetLength; }
    }

    /// <summary>
    /// The target absolute scan center angle in degree.
    /// </summary>
    public float TargetAngle
    {
        get { return _targetAngle; }
    }

    /// <summary>
    /// Whether the scanner is currently active on the server.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// The energy consumed by the scanner during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed by the scanner during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed by the scanner during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the scanner tick costs. The current placeholder model scales with the scanned surface and is tuned so
    /// that a maximum scan of 90 x 300 costs about 20 energy per tick.
    /// This also accepts the smaller runtime dimensions that occur while the scanner ramps up after activation.
    /// Values just above the maximum width or length are clipped before the cost is calculated.
    /// </summary>
    public bool CalculateCost(float width, float length, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampMaximum(width, _maximumWidth, out width) != InvalidArgumentKind.Valid)
            return false;

        if (RangeTolerance.ClampMaximum(length, _maximumLength, out length) != InvalidArgumentKind.Valid)
            return false;

        energy = MathF.PI * length * length * width / 360f * EnergyScale;

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Sets the target scanner configuration on the server.
    /// Width and length values just outside the valid range are clipped before they are sent.
    /// Scanner angles are absolute world angles.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if an argument is invalid.</exception>
    public async Task Set(float width, float length, float angle)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind widthValidity = RangeTolerance.ClampRange(width, MinimumWidthValue, _maximumWidth, out width);

        if (widthValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(widthValidity, "width");

        InvalidArgumentKind lengthValidity = RangeTolerance.ClampRange(length, MinimumLengthValue, _maximumLength, out length);

        if (lengthValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(lengthValidity, "length");

        InvalidArgumentKind angleValidity = RangeTolerance.Validate(angle);

        if (angleValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(angleValidity, "angle");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x89;
            writer.Write(Controllable.Id);
            writer.Write(_id);
            writer.Write(width);
            writer.Write(length);
            writer.Write(angle);
        });
    }

    /// <summary>
    /// Turns the scanner on.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    public async Task On()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8A;
            writer.Write(Controllable.Id);
            writer.Write(_id);
        });
    }

    /// <summary>
    /// Turns the scanner off.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    public async Task Off()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8B;
            writer.Write(Controllable.Id);
            writer.Write(_id);
        });
    }

    internal void ResetRuntime()
    {
        _currentWidth = 0f;
        _currentLength = 0f;
        _currentAngle = 0f;
        _targetWidth = 0f;
        _targetLength = 0f;
        _targetAngle = 0f;
        _active = false;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(bool active, float currentWidth, float currentLength, float currentAngle, float targetWidth,
        float targetLength, float targetAngle, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick)
    {
        _active = active;
        _currentWidth = currentWidth;
        _currentLength = currentLength;
        _currentAngle = currentAngle;
        _targetWidth = targetWidth;
        _targetLength = targetLength;
        _targetAngle = targetAngle;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicScannerSubsystemEvent(Controllable, Slot, Status, _active, _currentWidth, _currentLength, _currentAngle,
            _targetWidth, _targetLength, _targetAngle, _consumedEnergyThisTick, _consumedIonsThisTick, _consumedNeutrinosThisTick);
    }
}
