using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Shared base class for team-bound target units.
/// </summary>
public abstract class Target : SteadyUnit
{
    private Team _team;

    internal Target(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte teamId) || !cluster.Galaxy.Teams.TryGet(teamId, out Team? team))
            throw new InvalidDataException("Couldn't read Target.");

        _team = team;
    }

    internal Target(Target target) : base(target)
    {
        _team = target._team;
    }

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Team? Team => _team;

    /// <summary>
    /// Updates the owning team for this target during runtime sync.
    /// </summary>
    protected void UpdateTargetTeam(Team team)
    {
        _team = team;
    }
}
