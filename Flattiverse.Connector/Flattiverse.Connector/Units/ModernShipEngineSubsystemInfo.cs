using Flattiverse.Connector.Network;

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

    internal ModernShipEngineSubsystemInfo(ModernShipEngineSubsystemInfo other)
    {
        _exists = other._exists;
        _maximumThrust = other._maximumThrust;
        _maximumThrustChangePerTick = other._maximumThrustChangePerTick;
        _currentThrust = other._currentThrust;
        _targetThrust = other._targetThrust;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
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

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _maximumThrust = 0f;
            _maximumThrustChangePerTick = 0f;
            _currentThrust = 0f;
            _targetThrust = 0f;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out float maximumForwardThrust) ||
            !reader.Read(out float maximumReverseThrust) ||
            !reader.Read(out _maximumThrustChangePerTick) ||
            !reader.Read(out _currentThrust) ||
            !reader.Read(out _targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _maximumThrust = MathF.Max(maximumForwardThrust, maximumReverseThrust);
        _status = (SubsystemStatus)status;
        return true;
    }
}
