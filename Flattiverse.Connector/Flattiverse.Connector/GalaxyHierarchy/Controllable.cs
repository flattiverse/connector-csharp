using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// With this class you actually can control a player unit.
/// </summary>
public class Controllable : IDisposable, INamedUnit
{
    private readonly string _name;

    /// <summary>
    /// The id of the controllable.
    /// </summary>
    public readonly byte Id;
    
    private Cluster _cluster;
    
    private bool _active;
    
    private bool _alive;

    private Vector _position;
    private Vector _movement;

    internal Controllable(byte id, string name, Cluster cluster, PacketReader reader)
    {
        _cluster = cluster;

        Id = id;
        _name = name;
        
        _active = true;

        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read controllable.");
    }

    /// <summary>
    /// The name of the controllable.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// Specifies the kind of the controllable.
    /// </summary>
    public virtual UnitKind Kind => UnitKind.ClassicShipPlayerUnit;
    
    /// <summary>
    /// The cluster this unit currently is in.
    /// </summary>
    public Cluster Cluster => _cluster;
    
    /// <summary>
    /// The position of the unit.
    /// </summary>
    public Vector Position => new Vector(_position);
    
    /// <summary>
    /// The movement of the unit.
    /// </summary>
    public Vector Movement => new Vector(_movement);
    
    /// <summary>
    /// true, if the unit is alive.
    /// </summary>
    public bool Alive => _alive;
    
    /// <summary>
    /// true if this object still can be used. If the unit has been disposed this is false.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// Call this to continue the game with this unit after you are dead or when you have created the unit.
    /// </summary>
    public async Task Continue()
    {
        PacketWriter writer = new PacketWriter(new byte[1]);

        writer.Command = 0x84;
    
        writer.Write(Id);
    
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }

    internal void Deactivate()
    {
        _active = false;
        _alive = false;
    }
    
    /// <summary>
    /// Call this to suicide (=self destroy).
    /// </summary>
    public async Task Suicide()
    {
        PacketWriter writer = new PacketWriter(new byte[1]);

        writer.Command = 0x85;
    
        writer.Write(Id);
    
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }
    
    /// <summary>
    /// Call this to close the unit.
    /// </summary>
    public void Dispose()
    {
        PacketWriter writer = new PacketWriter(new byte[1]);

        writer.Command = 0x8F;
        writer.Write(Id);
        
        _cluster.Galaxy.Connection.Send(writer);
        _cluster.Galaxy.Connection.Flush();
    }

    internal static bool New(UnitKind kind, Cluster cluster, byte id, string name, PacketReader reader, out Controllable? controllable)
    {
        switch (kind)
        {
            case UnitKind.ClassicShipPlayerUnit:
                controllable = new ClassicShipControllable(cluster, id, name, reader);
                return true;
            default:
                controllable = null;
                return false;
        }
    }

    internal void Deceased()
    {
        _alive = false;
        
        _position = Vector.Null;
        _movement = Vector.Null;
    }

    internal void Updated(PacketReader reader)
    {
        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldan't read ControllableUpdate.");
    }
}