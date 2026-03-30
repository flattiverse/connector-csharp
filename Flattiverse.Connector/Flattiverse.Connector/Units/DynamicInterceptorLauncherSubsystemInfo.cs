namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a configurable interceptor launcher on a scanned player unit.
/// Its semantics are identical to <see cref="DynamicShotLauncherSubsystemInfo" />, but the launched projectile type is
/// an interceptor instead of a shot.
/// </summary>
public class DynamicInterceptorLauncherSubsystemInfo : DynamicShotLauncherSubsystemInfo
{
    internal DynamicInterceptorLauncherSubsystemInfo()
    {
    }
}
