using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Engine subsystem of one modern-ship thruster slot.
/// </summary>
public class ModernShipEngineSubsystem : Subsystem
{
    private float _maximumForwardThrust;
    private float _maximumReverseThrust;
    private float _maximumThrustChangePerTick;
    private float _currentThrust;
    private float _targetThrust;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ModernShipEngineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximumForwardThrust = 0f;
        _maximumReverseThrust = 0f;
        _maximumThrustChangePerTick = 0f;
        _currentThrust = 0f;
        _targetThrust = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    public float MaximumForwardThrust
    {
        get { return _maximumForwardThrust; }
    }

    public float MaximumReverseThrust
    {
        get { return _maximumReverseThrust; }
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

    public bool CalculateCost(float thrust, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        InvalidArgumentKind thrustValidity = RangeTolerance.ClampRange(thrust, -_maximumReverseThrust, _maximumForwardThrust, out thrust);

        if (thrustValidity != InvalidArgumentKind.Valid)
            return false;

        float absoluteThrust = MathF.Abs(thrust);
        energy = absoluteThrust * absoluteThrust * absoluteThrust * 20000f;

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    public async Task SetThrust(float thrust)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind thrustValidity = RangeTolerance.ClampRange(thrust, -_maximumReverseThrust, _maximumForwardThrust, out thrust);

        if (thrustValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(thrustValidity, "thrust");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA1;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
            writer.Write(thrust);
        });
    }

    public Task Off()
    {
        return SetThrust(0f);
    }

    internal void SetCapabilities(float maximumForwardThrust, float maximumReverseThrust, float maximumThrustChangePerTick)
    {
        _maximumForwardThrust = Exists ? maximumForwardThrust : 0f;
        _maximumReverseThrust = Exists ? maximumReverseThrust : 0f;
        _maximumThrustChangePerTick = Exists ? maximumThrustChangePerTick : 0f;
    }

    internal void ResetRuntime()
    {
        _currentThrust = 0f;
        _targetThrust = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float currentThrust, float targetThrust, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _currentThrust = currentThrust;
        _targetThrust = targetThrust;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ModernShipEngineSubsystemEvent(Controllable, Slot, Status, _currentThrust, _targetThrust, _consumedEnergyThisTick,
            _consumedIonsThisTick, _consumedNeutrinosThisTick);
    }
}
