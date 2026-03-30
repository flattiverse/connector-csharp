using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Summary entry of one editable unit in a cluster.
/// </summary>
public class EditableUnitSummary
{
    private readonly string _name;
    private readonly UnitKind _kind;

    internal EditableUnitSummary(string name, UnitKind kind)
    {
        _name = name;
        _kind = kind;
    }

    /// <summary>
    /// Name of the editable unit inside its cluster.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// Concrete unit kind of the editable unit.
    /// </summary>
    public UnitKind Kind
    {
        get { return _kind; }
    }
}
