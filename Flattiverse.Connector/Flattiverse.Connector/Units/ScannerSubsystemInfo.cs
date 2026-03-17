namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a scanner subsystem on a scanned player unit.
/// </summary>
public class ScannerSubsystemInfo
{
    private bool _exists;
    private float _maximumWidth;
    private float _maximumLength;
    private float _widthSpeed;
    private float _lengthSpeed;
    private float _angleSpeed;
    private bool _active;
    private float _currentWidth;
    private float _currentLength;
    private float _currentAngle;
    private float _targetWidth;
    private float _targetLength;
    private float _targetAngle;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ScannerSubsystemInfo()
    {
        _exists = false;
        _maximumWidth = 0f;
        _maximumLength = 0f;
        _widthSpeed = 0f;
        _lengthSpeed = 0f;
        _angleSpeed = 0f;
        _active = false;
        _currentWidth = 0f;
        _currentLength = 0f;
        _currentAngle = 0f;
        _targetWidth = 0f;
        _targetLength = 0f;
        _targetAngle = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Maximum configured scan width reported for this subsystem.
    /// </summary>
    public float MaximumWidth
    {
        get { return _maximumWidth; }
    }

    /// <summary>
    /// Maximum configured scan length reported for this subsystem.
    /// </summary>
    public float MaximumLength
    {
        get { return _maximumLength; }
    }

    /// <summary>
    /// Width speed capability of the reported scanner.
    /// </summary>
    public float WidthSpeed
    {
        get { return _widthSpeed; }
    }

    /// <summary>
    /// Length speed capability of the reported scanner.
    /// </summary>
    public float LengthSpeed
    {
        get { return _lengthSpeed; }
    }

    /// <summary>
    /// Angle speed capability of the reported scanner.
    /// </summary>
    public float AngleSpeed
    {
        get { return _angleSpeed; }
    }

    /// <summary>
    /// Whether the scanner was active for the tick.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// Current reported scan width.
    /// </summary>
    public float CurrentWidth
    {
        get { return _currentWidth; }
    }

    /// <summary>
    /// Current reported scan length.
    /// </summary>
    public float CurrentLength
    {
        get { return _currentLength; }
    }

    /// <summary>
    /// Current reported scan center angle.
    /// </summary>
    public float CurrentAngle
    {
        get { return _currentAngle; }
    }

    /// <summary>
    /// Last reported target width.
    /// </summary>
    public float TargetWidth
    {
        get { return _targetWidth; }
    }

    /// <summary>
    /// Last reported target length.
    /// </summary>
    public float TargetLength
    {
        get { return _targetLength; }
    }

    /// <summary>
    /// Last reported target angle.
    /// </summary>
    public float TargetAngle
    {
        get { return _targetAngle; }
    }

    /// <summary>
    /// Status of the reported scanner subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the scanner during the tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the scanner during the tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the scanner during the tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal void Update(bool exists, float maximumWidth, float maximumLength, float widthSpeed, float lengthSpeed, float angleSpeed,
        bool active, float currentWidth, float currentLength, float currentAngle, float targetWidth, float targetLength, float targetAngle,
        SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _exists = exists;
        _maximumWidth = exists ? maximumWidth : 0f;
        _maximumLength = exists ? maximumLength : 0f;
        _widthSpeed = exists ? widthSpeed : 0f;
        _lengthSpeed = exists ? lengthSpeed : 0f;
        _angleSpeed = exists ? angleSpeed : 0f;
        _active = exists && active;
        _currentWidth = exists ? currentWidth : 0f;
        _currentLength = exists ? currentLength : 0f;
        _currentAngle = exists ? currentAngle : 0f;
        _targetWidth = exists ? targetWidth : 0f;
        _targetLength = exists ? targetLength : 0f;
        _targetAngle = exists ? targetAngle : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _consumedEnergyThisTick = exists ? consumedEnergyThisTick : 0f;
        _consumedIonsThisTick = exists ? consumedIonsThisTick : 0f;
        _consumedNeutrinosThisTick = exists ? consumedNeutrinosThisTick : 0f;
    }
}
