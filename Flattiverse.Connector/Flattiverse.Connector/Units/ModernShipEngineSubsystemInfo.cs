namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a modern-ship engine subsystem on a scanned player unit.
/// </summary>
public class ModernShipEngineSubsystemInfo
{
    private bool _exists;
    private float _maximumThrust;
    private float _maximumThrustChangePerTick;
    private float _currentThrust;
    private float _targetThrust;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ModernShipEngineSubsystemInfo()
    {
        _exists = false;
        _maximumThrust = 0f;
        _maximumThrustChangePerTick = 0f;
        _currentThrust = 0f;
        _targetThrust = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    public bool Exists
    {
        get { return _exists; }
    }

    public float MaximumThrust
    {
        get { return _maximumThrust; }
    }

    public float MaximumForwardThrust
    {
        get { return _maximumThrust; }
    }

    public float MaximumReverseThrust
    {
        get { return _maximumThrust; }
    }

    public float MaximumThrustChangePerTick
    {
        get { return _maximumThrustChangePerTick; }
    }

    public float CurrentThrust
    {
        get { return _currentThrust; }
    }

    public float TargetThrust
    {
        get { return _targetThrust; }
    }

    public SubsystemStatus Status
    {
        get { return _status; }
    }

    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal void Update(bool exists, float maximumThrust, float maximumThrustChangePerTick,
        float currentThrust, float targetThrust, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick)
    {
        _exists = exists;
        _maximumThrust = exists ? maximumThrust : 0f;
        _maximumThrustChangePerTick = exists ? maximumThrustChangePerTick : 0f;
        _currentThrust = exists ? currentThrust : 0f;
        _targetThrust = exists ? targetThrust : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _consumedEnergyThisTick = exists ? consumedEnergyThisTick : 0f;
        _consumedIonsThisTick = exists ? consumedIonsThisTick : 0f;
        _consumedNeutrinosThisTick = exists ? consumedNeutrinosThisTick : 0f;
    }
}
