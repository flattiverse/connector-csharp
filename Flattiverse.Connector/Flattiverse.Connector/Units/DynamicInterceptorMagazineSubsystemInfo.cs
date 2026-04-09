namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of an interceptor magazine on a scanned player unit.
/// Its semantics are identical to <see cref="DynamicShotMagazineSubsystemInfo" />, but the ammunition consists of
/// interceptors instead of shots.
/// </summary>
public class DynamicInterceptorMagazineSubsystemInfo : DynamicShotMagazineSubsystemInfo
{
    internal DynamicInterceptorMagazineSubsystemInfo()
    {
    }

    internal DynamicInterceptorMagazineSubsystemInfo(DynamicInterceptorMagazineSubsystemInfo other) : base(other)
    {
    }
}
