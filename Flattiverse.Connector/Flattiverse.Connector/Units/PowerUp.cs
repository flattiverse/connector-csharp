using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Shared base class for visible collectible power-ups.
/// </summary>
public abstract class PowerUp : SteadyUnit
{
    private float _amount;

    internal PowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _amount = 0f;
    }

    internal PowerUp(PowerUp powerUp) : base(powerUp)
    {
        _amount = powerUp._amount;
    }

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <summary>
    /// The configured payload amount of this power-up.
    /// </summary>
    public float Amount
    {
        get { return _amount; }
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _amount))
            throw new InvalidDataException("Couldn't read PowerUp.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Amount={_amount:0.###}";
    }
}
