using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// General infos about ships or bases.
/// </summary>
public class ControllableInfo : INamedUnit
{
    /// <summary>
    /// The galaxy instance this ControllableInfo belongs to.
    /// </summary>
    public readonly Galaxy Galaxy;

    /// <summary>
    /// The player this ControlableInfo belongs to.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// The id of this ControllableInfo.
    /// </summary>
    public readonly byte Id;
    
    private readonly string _name;

    private bool _alive;
    
    private bool _active;

    internal ControllableInfo(Galaxy galaxy, Player player, byte id, string name, bool alive)
    {
        Galaxy = galaxy;
        Player = player;
        Id = id;
        
        _name = name;
        
        _alive = alive;
        
        _active = true;
    }
    
    /// <summary>
    /// The name of the controllable.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// true, if the corresponding PlayerUnit is alive.
    /// </summary>
    public bool Alive => _alive;
    
    /// <summary>
    /// true, if the corresponding PlayerUnit is still in use.
    /// </summary>
    public bool Active => _active;
    
    /// <summary>
    /// Specifies the kind of the PlayerUnit.
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
        
        Galaxy.PushEvent(new ContinuedControllableInfoPlayerEvent(Player, this));
    }

    internal void SetDeadByNeutralColission(UnitKind kind, string collider)
    {
        _alive = false;
        
        Galaxy.PushEvent(new NeutralDestroyedControllableInfoPlayerEvent(Player, this, kind, collider));
    }

    internal void SetDeadByPlayerShip(PlayerUnitDestroyedReason reason, ControllableInfo info)
    {
        _alive = false;
        
        Galaxy.PushEvent(new PlayerUnitDestroyedControllableInfoPlayerEvent(Player, this, reason, info));
    }
    
    internal void SetDead(PlayerUnitDestroyedReason reason)
    {
        _alive = false;
        
        Galaxy.PushEvent(new DestroyedControllableInfoPlayerEvent(Player, this, reason));
    }

    internal static bool New(UnitKind kind, Player player, byte id, string name, bool alive, [NotNullWhen(true)] out ControllableInfo? info)
    {
        switch (kind)
        {
            case UnitKind.ClassicShipPlayerUnit:
                info = new ClassicShipControllableInfo(player.Galaxy, player, id, name, alive);
                return true;
            case UnitKind.NewShipPlayerUnit:
                info = new NewShipControllableInfo(player.Galaxy, player, id, name, alive);
                return true;
            default:
                info = null;
                return false;
        }
    }
}