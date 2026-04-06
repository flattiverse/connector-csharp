using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

public partial class Galaxy
{
    /// <summary>
    /// Starts rebuilding the static segment data on the server.
    /// </summary>
    /// <exception cref="StaticMapRebuildLockedGameException">
    /// Thrown, if the current galaxy tournament state blocks rebuilding.
    /// </exception>
    public async Task RebuildStaticMap()
    {
        await Connection.SendSessionRequestAndGetReply(delegate(ref PacketWriter writer)
        {
            writer.Command = 0x68;
        }).ConfigureAwait(false);
    }
}
