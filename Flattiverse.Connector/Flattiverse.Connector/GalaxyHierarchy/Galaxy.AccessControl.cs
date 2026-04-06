using Flattiverse.Connector.Network;
using AccountSummary = Flattiverse.Connector.Account.Account;

namespace Flattiverse.Connector.GalaxyHierarchy;

public partial class Galaxy
{
    /// <summary>
    /// Queries one ACL list for this galaxy.
    /// </summary>
    /// <param name="kind">Whether to query the player or admin ACL list.</param>
    /// <param name="progressState">
    /// Optional progress object that is updated while the connector downloads chunked account pages.
    /// </param>
    /// <returns>All account snapshots currently listed in the requested ACL.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown, if <paramref name="kind" /> is not supported.</exception>
    /// <exception cref="PermissionFailedGameException">Thrown, if the current connection has no admin rights.</exception>
    public async Task<AccountSummary[]> QueryAclAccounts(GalaxyAclKind kind, ProgressState? progressState = null)
    {
        ValidateAclKind(kind);

        return await ChunkedTransfer.DownloadItems(Connection, delegate (ref PacketWriter writer, int offset, ushort maximumCount)
        {
            writer.Command = 0x65;
            writer.Write((byte)kind);
            writer.Write(offset);
            writer.Write(maximumCount);
        }, delegate (ref PacketReaderLarge reader, out AccountSummary account)
        {
            if (!AccountSummary.TryRead(this, ref reader, out AccountSummary? parsedAccount) || parsedAccount is null)
            {
                account = null!;
                return false;
            }

            account = parsedAccount;
            return true;
        }, progressState, ChunkedTransfer.AccountChunkMaximumCount, "ACL account query result").ConfigureAwait(false);
    }

    /// <summary>
    /// Adds one account id to one ACL list of this galaxy.
    /// </summary>
    /// <param name="kind">Target ACL list.</param>
    /// <param name="accountId">Persistent account id to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, if <paramref name="kind" /> or <paramref name="accountId" /> is outside the supported range.
    /// </exception>
    /// <exception cref="PermissionFailedGameException">Thrown, if the current connection has no admin rights.</exception>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the account id does not exist.</exception>
    public async Task AddAclAccount(GalaxyAclKind kind, int accountId)
    {
        ValidateAclKind(kind);

        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId), accountId, "Account id must be positive.");

        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x66;
            writer.Write((byte)kind);
            writer.Write(accountId);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes one account id from one ACL list of this galaxy.
    /// </summary>
    /// <param name="kind">Target ACL list.</param>
    /// <param name="accountId">Persistent account id to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown, if <paramref name="kind" /> or <paramref name="accountId" /> is outside the supported range.
    /// </exception>
    /// <exception cref="PermissionFailedGameException">Thrown, if the current connection has no admin rights.</exception>
    public async Task RemoveAclAccount(GalaxyAclKind kind, int accountId)
    {
        ValidateAclKind(kind);

        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId), accountId, "Account id must be positive.");

        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x67;
            writer.Write((byte)kind);
            writer.Write(accountId);
        }).ConfigureAwait(false);
    }

    private static void ValidateAclKind(GalaxyAclKind kind)
    {
        switch (kind)
        {
            case GalaxyAclKind.Player:
            case GalaxyAclKind.Admin:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "Only player/admin ACL kinds are supported.");
        }
    }
}
