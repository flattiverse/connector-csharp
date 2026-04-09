using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Jump-drive subsystem of a controllable.
/// </summary>
public class JumpDriveSubsystem : Subsystem
{
    private float _energyCost;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal JumpDriveSubsystem(Controllable controllable, bool exists) : base(controllable, "JumpDrive", exists, SubsystemSlot.JumpDrive)
    {
        _energyCost = exists ? 6000f : 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal JumpDriveSubsystem(Controllable controllable, PacketReader reader) : base(controllable, "JumpDrive", false, SubsystemSlot.JumpDrive)
    {
        _energyCost = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable jump drive state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float energyCost))
            throw new InvalidDataException("Couldn't read controllable jump drive state.");

        SetEnergyCost(energyCost);
        SetReportedTier(tier);
    }

    /// <summary>
    /// Energy required for one jump.
    /// </summary>
    public float EnergyCost
    {
        get { return _energyCost; }
    }

    /// <summary>
    /// Standard energy consumed by the jump drive during the current tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the jump drive during the current tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the jump drive during the current tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the current fixed jump cost.
    /// </summary>
    public bool CalculateCost(out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        energy = _energyCost;
        return true;
    }

    /// <summary>
    /// Requests a worm-hole jump on the server.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if the jump is currently not possible.</exception>
    public async Task Jump()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x95;
            writer.Write(Controllable.Id);
        });
    }

    internal void ResetRuntime()
    {
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime((SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    internal void SetEnergyCost(float energyCost)
    {
        _energyCost = Exists ? energyCost : 0f;

        RefreshTier();
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        ShipBalancing.GetJumpDrive(out float energyCost, out float load);
        SetTier(Matches(_energyCost, energyCost) ? (byte)1 : (byte)0);
    }
}
