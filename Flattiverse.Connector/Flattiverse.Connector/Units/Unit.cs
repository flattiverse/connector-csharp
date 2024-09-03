using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using InvalidDataException = System.IO.InvalidDataException;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a unit in Flattiverse. Each unit in a Cluster derives from this class. This class has
/// properties and methods which most units have in common. Derived classes overwrite those properties
/// and methods, other properties and methods are added via interfaces.
/// </summary>
public class Unit
{
    /// <summary>
    /// This is the name of the unit. A unit can't change her name after it has been set up.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The cluster this unit is in.
    /// </summary>
    protected Cluster _cluster;

    internal Unit(Cluster cluster, string name)
    {
        _cluster = cluster;
        Name = name;
    }

    internal Unit(Unit unit)
    {
        _cluster = unit._cluster;
        Name = unit.Name;
    }
    
    /// <summary>
    /// The radius of the unit.
    /// </summary>
    public virtual float Radius => 3f;
    
    /// <summary>
    /// The position of the unit.
    /// </summary>
    public virtual Vector Position => Vector.Null;
    
    /// <summary>
    /// The movement of the unit.
    /// </summary>
    public virtual Vector Movement => Vector.Null;
    
    /// <summary>
    /// The direction the unit is looking into.
    /// </summary>
    public virtual float Angle => 0f;

    /// <summary>
    /// If true, other units can hide behind this unit.
    /// </summary>
    public virtual bool IsMasking => true;
    
    /// <summary>
    /// If true, a crash with this unit is lethal.
    /// </summary>
    public virtual bool IsSolid => true;
    
    /// <summary>
    /// If true, the unit can be edited via map editor calls.
    /// </summary>
    public virtual bool CanBeEdited => false;

    /// <summary>
    /// The gravity of this unit. This is how much this unit pulls others towards it.
    /// </summary>
    public virtual float Gravity => 0f;

    /// <summary>
    /// The mobility of the unit.
    /// </summary>
    public virtual Mobility Mobility => Mobility.Still;
    
    /// <summary>
    /// The kind of the unit for a better switch() experience.
    /// </summary>
    public virtual UnitKind Kind => UnitKind.Sun;
    
    /// <summary>
    /// The cluster the unit is in.
    /// </summary>
    public virtual Cluster Cluster => _cluster;
    
    /// <summary>
    /// The team of the unit.
    /// </summary>
    public virtual Team? Team => null;

    internal static bool TryReadUnit(UnitKind kind, Cluster cluster, string name, PacketReader reader, [NotNullWhen(true)] out Unit? unit)
    {
        switch (kind)
        {
            default:
            case UnitKind.Sun:
            case UnitKind.BlackHole:
            case UnitKind.Moon:
            case UnitKind.Meteoroid:
                unit = null;
                return false;
            case UnitKind.Planet:
                unit = new Planet(cluster, name, reader);
                return true;
            case UnitKind.ClassicShipPlayerUnit:
                unit = new ClassicShipPlayerUnit(cluster, name, reader);
                return true;
        }
    }

    /// <summary>
    /// Deep clones the unit.
    /// </summary>
    /// <returns>The cloned unit.</returns>
    public virtual Unit Clone()
    {
        throw new InvalidOperationException("Should be overwritten from derived class.");
    }

    internal virtual void UpdateMovement(PacketReader reader)
    {
    }
}