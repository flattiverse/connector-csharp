namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if you call a method concurrently which doesn't support multiple parallel calls.
/// </summary>
public class CantCallThisConcurrentGameException : GameException
{
    internal CantCallThisConcurrentGameException() : base(0x11, "[0x11] This method cannot be called concurrently.")
    {
    }
}