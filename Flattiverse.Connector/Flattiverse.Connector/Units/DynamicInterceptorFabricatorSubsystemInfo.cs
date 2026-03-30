namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of an interceptor fabricator on a scanned player unit.
/// Its semantics are identical to <see cref="DynamicShotFabricatorSubsystemInfo" />, but it fabricates interceptor
/// ammunition instead of shot ammunition.
/// </summary>
public class DynamicInterceptorFabricatorSubsystemInfo : DynamicShotFabricatorSubsystemInfo
{
    internal DynamicInterceptorFabricatorSubsystemInfo()
    {
    }
}
