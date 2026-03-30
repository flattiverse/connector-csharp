using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Jump-drive subsystem of a controllable.
/// </summary>
public class JumpDriveSubsystem : Subsystem
{
    private float _energyCost;

    internal JumpDriveSubsystem(Controllable controllable, bool exists) : base(controllable, "JumpDrive", exists, SubsystemSlot.JumpDrive)
    {
        _energyCost = exists ? 1000f : 0f;
    }

    /// <summary>
    /// Energy required for one jump.
    /// </summary>
    public float EnergyCost
    {
        get { return _energyCost; }
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
        ResetRuntimeStatus();
    }

    internal void SetEnergyCost(float energyCost)
    {
        _energyCost = Exists ? energyCost : 0f;
    }
}
