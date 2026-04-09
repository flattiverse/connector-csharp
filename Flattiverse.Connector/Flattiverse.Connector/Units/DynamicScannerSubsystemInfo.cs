using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a dynamic scanner subsystem on a scanned player unit.
/// </summary>
public class DynamicScannerSubsystemInfo
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

    internal DynamicScannerSubsystemInfo()
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

    internal DynamicScannerSubsystemInfo(DynamicScannerSubsystemInfo other)
    {
        _exists = other._exists;
        _maximumWidth = other._maximumWidth;
        _maximumLength = other._maximumLength;
        _widthSpeed = other._widthSpeed;
        _lengthSpeed = other._lengthSpeed;
        _angleSpeed = other._angleSpeed;
        _active = other._active;
        _currentWidth = other._currentWidth;
        _currentLength = other._currentLength;
        _currentAngle = other._currentAngle;
        _targetWidth = other._targetWidth;
        _targetLength = other._targetLength;
        _targetAngle = other._targetAngle;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
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
    /// Whether the scanner was active during the reported tick.
    /// After switching the scanner off, the current geometry typically drops back to zero until it is activated again.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// Current scan width reported by the server for this tick.
    /// This is the live runtime value, not necessarily the requested target width.
    /// </summary>
    public float CurrentWidth
    {
        get { return _currentWidth; }
    }

    /// <summary>
    /// Current scan length reported by the server for this tick.
    /// </summary>
    public float CurrentLength
    {
        get { return _currentLength; }
    }

    /// <summary>
    /// Current absolute world-space scan center angle reported for this tick.
    /// </summary>
    public float CurrentAngle
    {
        get { return _currentAngle; }
    }

    /// <summary>
    /// Target scan width currently requested on the server.
    /// </summary>
    public float TargetWidth
    {
        get { return _targetWidth; }
    }

    /// <summary>
    /// Target scan length currently requested on the server.
    /// </summary>
    public float TargetLength
    {
        get { return _targetLength; }
    }

    /// <summary>
    /// Target absolute world-space scan center angle currently requested on the server.
    /// </summary>
    public float TargetAngle
    {
        get { return _targetAngle; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the scanner subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by scanning during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by scanning during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by scanning during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
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
            return true;
        }

        if (!reader.Read(out _maximumWidth) ||
            !reader.Read(out _maximumLength) ||
            !reader.Read(out _widthSpeed) ||
            !reader.Read(out _lengthSpeed) ||
            !reader.Read(out _angleSpeed) ||
            !reader.Read(out byte active) ||
            !reader.Read(out _currentWidth) ||
            !reader.Read(out _currentLength) ||
            !reader.Read(out _currentAngle) ||
            !reader.Read(out _targetWidth) ||
            !reader.Read(out _targetLength) ||
            !reader.Read(out _targetAngle) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _active = active != 0;
        _status = (SubsystemStatus)status;
        return true;
    }
}
