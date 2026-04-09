using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Persistent roster entry for one player-owned controllable.
/// This owner-side identity survives deaths and exists independently from the visible
/// <see cref="PlayerUnit" /> mirror in a cluster.
/// </summary>
public class ControllableInfo : INamedUnit
{
    /// <summary>
    /// The galaxy instance this ControllableInfo belongs to.
    /// </summary>
    public readonly Galaxy Galaxy;

    /// <summary>
    /// The player who owns this controllable entry.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// The id of this ControllableInfo.
    /// </summary>
    public readonly byte Id;
    
    private readonly string _name;
    private readonly Score _score;

    private bool _alive;
    
    private bool _active;

    internal ControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive)
    {
        Galaxy = galaxy;
        Player = player;
        Id = id;
        
        _name = name;
        _score = new Score();
        
        _alive = alive;
        
        _active = true;
    }
    
    /// <summary>
    /// The name of the controllable.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// True while this controllable currently has an alive in-world runtime.
    /// </summary>
    public bool Alive => _alive;
    
    /// <summary>
    /// True while this controllable registration still exists on the server.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// Current live score of this controllable inside one galaxy session.
    /// </summary>
    public Score Score => _score;
    
    /// <summary>
    /// Runtime unit kind this controllable uses while it is alive in the world.
    /// </summary>
    public virtual UnitKind Kind => throw new InvalidOperationException("Must be implemented in the derived class.");

    internal void Deactivate()
    {
        _active = false;
        _alive = false;
    }

    internal void SetAlive()
    {
        _alive = true;
        
        Galaxy.PushEvent(new ContinuedControllableInfoEvent(Player, this));
    }

    internal void SetDeadByNeutralCollision(UnitKind kind, string collider)
    {
        _alive = false;
        
        Galaxy.PushEvent(new DestroyedControllableInfoByNeutralUnitEvent(Player, this, kind, collider));
    }

    internal void SetDeadByPlayerShip(PlayerUnitDestroyedReason reason, ControllableInfo info)
    {
        _alive = false;
        
        Galaxy.PushEvent(new DestroyedControllableInfoByPlayerUnitEvent(Player, this, reason, info));
    }
    
    internal void SetDead(PlayerUnitDestroyedReason reason)
    {
        _alive = false;
        
        Galaxy.PushEvent(new DestroyedControllableInfoEvent(Player, this, reason));
    }

    internal static bool New(UnitKind kind, Player player, byte id, string name, bool alive, [NotNullWhen(true)] out ControllableInfo? info)
    {
        switch (kind)
        {
            case UnitKind.ClassicShipPlayerUnit:
                info = new ClassicShipControllableInfo(player.Galaxy, player, id, name, alive);
                return true;
            case UnitKind.ModernShipPlayerUnit:
                info = new ModernShipControllableInfo(player.Galaxy, player, id, name, alive);
                return true;
            default:
                info = null;
                return false;
        }
    }
}
