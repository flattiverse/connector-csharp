using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The controllable of a classic ship.
/// </summary>
public class ClassicShipControllable : Controllable
{
    internal ClassicShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name,
        cluster, reader)
    {
    }
    
    /// <summary>
    /// Call this to move your ship. This vector will be the impulse your ship gets every tick until you specify a new vector. Length of 0 will turn off your engines.
    /// </summary>
    public async Task Move(Vector movement)
    {
        if (!Active)
            throw new SpecifiedElementNotFoundGameException();
        
        if (!Alive)
            throw new YouNeedToContinueFirstGameException();
        
        if (float.IsNaN(movement.X) || float.IsNaN(movement.Y))
            throw new InvalidArgumentGameException(InvalidArgumentKind.ContainedNaN, "movement");
        
        if (movement.IsDamaged)
            throw new InvalidArgumentGameException(InvalidArgumentKind.ContainedInfinity, "movement");
        
        if (movement.Length > 0.101f)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "movement");
        
        PacketWriter writer = new PacketWriter(new byte[12]);

        writer.Command = 0x87;
        
        writer.Write(Id);
        movement.Write(writer);
    
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }

    /// <summary>
    /// Call this to move your ship. This vector will be the impulse your ship gets every tick until you specify a new vector. Length of 0 will turn off your engines.
    /// </summary>
    public async Task Shoot(Vector relativeMovement, ushort ticks, float load, float damage)
    {
        if (!Active)
            throw new SpecifiedElementNotFoundGameException();
        
        if (!Alive)
            throw new YouNeedToContinueFirstGameException();
        
        if (float.IsNaN(relativeMovement.X) || float.IsNaN(relativeMovement.Y))
            throw new InvalidArgumentGameException(InvalidArgumentKind.ContainedNaN, "relativeMovement");
        
        if (relativeMovement.IsDamaged)
            throw new InvalidArgumentGameException(InvalidArgumentKind.ContainedInfinity, "relativeMovement");
        
        if (relativeMovement.Length > 3.001f)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "relativeMovement");
        
        if (relativeMovement.Length < 0.099f)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "relativeMovement");
        
        if (ticks < 3)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "ticks");

        if (ticks > 140)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "ticks");

        if (load < 3)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "load");

        if (load > 25)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "load");

        if (damage < 0.099f)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "damage");

        if (damage > 3.001f)
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooLarge, "damage");

        PacketWriter writer = new PacketWriter(new byte[32]);

        writer.Command = 0x88;
        
        writer.Write(Id);
        relativeMovement.Write(writer);
        writer.Write(ticks);
        writer.Write(load);
        writer.Write(damage);
    
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }

    /// <inheritdoc/>
    public override float Gravity => 14f;
    
    /// <inheritdoc/>
    public override float Size => 14f;
}