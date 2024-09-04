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
}