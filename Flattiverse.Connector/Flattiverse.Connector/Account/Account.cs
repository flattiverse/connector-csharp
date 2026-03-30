using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Account;

/// <summary>
/// An account snapshot used for tournament administration and tournament state.
/// </summary>
public class Account
{
    private readonly Galaxy _galaxy;
    private readonly int _id;
    private readonly string _name;
    private readonly bool _admin;
    private readonly int _rank;
    private readonly long _playerKills;
    private readonly long _playerDeaths;
    private readonly bool _hasAvatar;
    private readonly float? _tournamentElo;

    internal Account(Galaxy galaxy, int id, string name, bool admin, int rank, long playerKills, long playerDeaths, bool hasAvatar,
        float? tournamentElo)
    {
        _galaxy = galaxy;
        _id = id;
        _name = name;
        _admin = admin;
        _rank = rank;
        _playerKills = playerKills;
        _playerDeaths = playerDeaths;
        _hasAvatar = hasAvatar;
        _tournamentElo = tournamentElo;
    }

    /// <summary>
    /// Stable account id from persistent galaxy storage.
    /// </summary>
    public int Id
    {
        get { return _id; }
    }

    /// <summary>
    /// Account name as currently known by the galaxy.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// Whether the account currently has admin permissions.
    /// </summary>
    public bool Admin
    {
        get { return _admin; }
    }

    /// <summary>
    /// Global account rank mirrored from persistent storage.
    /// </summary>
    public int Rank
    {
        get { return _rank; }
    }

    /// <summary>
    /// Lifetime player-kill statistic stored on the account.
    /// </summary>
    public long PlayerKills
    {
        get { return _playerKills; }
    }

    /// <summary>
    /// Lifetime player-death statistic stored on the account.
    /// </summary>
    public long PlayerDeaths
    {
        get { return _playerDeaths; }
    }

    /// <summary>
    /// Whether the account currently has a persisted avatar that can be downloaded.
    /// </summary>
    public bool HasAvatar
    {
        get { return _hasAvatar; }
    }

    /// <summary>
    /// Tournament Elo mirrored by the server, or <see langword="null" /> if no tournament rating is stored.
    /// </summary>
    public float? TournamentElo
    {
        get { return _tournamentElo; }
    }

    /// <summary>
    /// Downloads the small persisted avatar image of this account.
    /// </summary>
    /// <param name="progressState">
    /// Optional progress object that is updated while the connector downloads avatar chunks from the galaxy.
    /// </param>
    /// <returns>The complete small avatar as raw bytes.</returns>
    /// <exception cref="AvatarNotAvailableGameException">
    /// Thrown, if <see cref="HasAvatar" /> is <see langword="false" />.
    /// </exception>
    public async Task<byte[]> DownloadSmallAvatar(ProgressState? progressState = null)
    {
        if (!_hasAvatar)
            throw new AvatarNotAvailableGameException();

        return await ChunkedTransfer.DownloadBytes(_galaxy.Connection, delegate (ref PacketWriter writer, int offset, ushort maximumLength)
        {
            writer.Command = 0xF3;
            writer.Write(_id);
            writer.Write(offset);
            writer.Write(maximumLength);
        }, progressState, "small account avatar").ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads the large persisted avatar image of this account.
    /// </summary>
    /// <param name="progressState">
    /// Optional progress object that is updated while the connector downloads avatar chunks from the galaxy.
    /// </param>
    /// <returns>The complete large avatar as raw bytes.</returns>
    /// <exception cref="AvatarNotAvailableGameException">
    /// Thrown, if <see cref="HasAvatar" /> is <see langword="false" />.
    /// </exception>
    public async Task<byte[]> DownloadBigAvatar(ProgressState? progressState = null)
    {
        if (!_hasAvatar)
            throw new AvatarNotAvailableGameException();

        return await ChunkedTransfer.DownloadBytes(_galaxy.Connection, delegate (ref PacketWriter writer, int offset, ushort maximumLength)
        {
            writer.Command = 0xF4;
            writer.Write(_id);
            writer.Write(offset);
            writer.Write(maximumLength);
        }, progressState, "big account avatar").ConfigureAwait(false);
    }

    internal static bool TryRead(Galaxy galaxy, PacketReader reader, out Account? account)
    {
        account = null;

        if (!reader.Read(out int id) ||
            !reader.Read(out string name) ||
            !reader.Read(out byte admin) ||
            !reader.Read(out int rank) ||
            !reader.Read(out long playerKills) ||
            !reader.Read(out long playerDeaths) ||
            !reader.Read(out byte hasAvatar) ||
            !reader.Read(out byte hasTournamentElo))
            return false;

        float? tournamentElo = null;

        if (hasTournamentElo != 0x00)
        {
            if (!reader.Read(out float parsedTournamentElo))
                return false;

            tournamentElo = parsedTournamentElo;
        }

        account = new Account(galaxy, id, name, admin != 0x00, rank, playerKills, playerDeaths, hasAvatar != 0x00, tournamentElo);
        return true;
    }

    internal static bool TryRead(Galaxy galaxy, ref PacketReaderLarge reader, out Account? account)
    {
        account = null;

        if (!reader.Read(out int id) ||
            !reader.Read(out string? name) ||
            !reader.Read(out byte admin) ||
            !reader.Read(out int rank) ||
            !reader.Read(out long playerKills) ||
            !reader.Read(out long playerDeaths) ||
            !reader.Read(out byte hasAvatar) ||
            !reader.Read(out byte hasTournamentElo) ||
            name is null)
            return false;

        float? tournamentElo = null;

        if (hasTournamentElo != 0x00)
        {
            if (!reader.Read(out float parsedTournamentElo))
                return false;

            tournamentElo = parsedTournamentElo;
        }

        account = new Account(galaxy, id, name, admin != 0x00, rank, playerKills, playerDeaths, hasAvatar != 0x00, tournamentElo);
        return true;
    }
}
