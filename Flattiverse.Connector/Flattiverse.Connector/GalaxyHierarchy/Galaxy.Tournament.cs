using System.Xml.Linq;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using TournamentAccount = Flattiverse.Connector.Account.Account;

namespace Flattiverse.Connector.GalaxyHierarchy;

public partial class Galaxy
{
    private Tournament? _tournament;

    /// <summary>
    /// Current tournament snapshot mirrored from the server, or <see langword="null" /> if no tournament is configured.
    /// The reference changes through <see cref="TournamentCreatedEvent" />, <see cref="TournamentUpdatedEvent" />, and
    /// <see cref="TournamentRemovedEvent" />.
    /// </summary>
    public Tournament? Tournament
    {
        get { return _tournament; }
    }

    /// <summary>
    /// Configures a tournament from a typed connector-side description.
    /// </summary>
    /// <param name="configuration">
    /// Tournament configuration to upload. The current server expects a tournament setup with series mode, duration,
    /// participating teams, participant account ids, and optional finished-match history.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown, if <paramref name="configuration" /> is <see langword="null" />.</exception>
    /// <exception cref="TournamentAlreadyConfiguredGameException">
    /// Thrown, if the galaxy already has a configured tournament.
    /// </exception>
    /// <exception cref="TournamentModeNotAllowedGameException">
    /// Thrown, if the current galaxy game mode does not allow tournaments.
    /// </exception>
    /// <exception cref="GameException">
    /// Thrown, if the server rejects the generated tournament configuration for another protocol-level reason.
    /// </exception>
    public async Task ConfigureTournament(TournamentConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        XElement root = new XElement("Tournament",
            new XAttribute("Mode", configuration.Mode),
            new XAttribute("DurationTicks", configuration.DurationTicks));

        for (int teamIndex = 0; teamIndex < configuration.Teams.Count; teamIndex++)
        {
            TournamentTeamConfiguration teamConfiguration = configuration.Teams[teamIndex];
            XElement teamElement = new XElement("Team", new XAttribute("Id", teamConfiguration.Team.Id));

            for (int accountIndex = 0; accountIndex < teamConfiguration.AccountIds.Count; accountIndex++)
                teamElement.Add(new XElement("Account", new XAttribute("Id", teamConfiguration.AccountIds[accountIndex])));

            root.Add(teamElement);
        }

        for (int historyIndex = 0; historyIndex < configuration.WinningTeamIds.Count; historyIndex++)
            root.Add(new XElement("Match", new XAttribute("WinnerTeamId", configuration.WinningTeamIds[historyIndex])));

        string xml = new XDocument(root).ToString(SaveOptions.DisableFormatting);

        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x60;
            writer.Write(xml);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Advances the configured tournament from preparation into the commencing stage.
    /// </summary>
    /// <exception cref="TournamentNotConfiguredGameException">Thrown, if no tournament is configured.</exception>
    /// <exception cref="TournamentWrongStageGameException">
    /// Thrown, if the current tournament stage does not allow this transition.
    /// </exception>
    public async Task CommenceTournament()
    {
        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x61;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts a previously commenced tournament so that it enters the running stage.
    /// </summary>
    /// <exception cref="TournamentNotConfiguredGameException">Thrown, if no tournament is configured.</exception>
    /// <exception cref="TournamentWrongStageGameException">
    /// Thrown, if the current tournament stage does not allow this transition.
    /// </exception>
    public async Task StartTournament()
    {
        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x62;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the currently configured tournament from the galaxy.
    /// </summary>
    /// <exception cref="TournamentNotConfiguredGameException">Thrown, if no tournament is configured.</exception>
    /// <exception cref="TournamentWrongStageGameException">
    /// Thrown, if the current tournament stage does not allow cancellation.
    /// </exception>
    public async Task CancelTournament()
    {
        await Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x63;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Queries the account list that the server exposes for tournament tooling.
    /// </summary>
    /// <param name="progressState">
    /// Optional progress object that is updated while the connector downloads chunked account pages.
    /// </param>
    /// <returns>
    /// All account snapshots returned by the server for tournament configuration, currently limited to the account
    /// states relevant for tournament administration.
    /// </returns>
    /// <exception cref="PermissionFailedGameException">Thrown, if the current connection has no admin rights.</exception>
    public async Task<TournamentAccount[]> QueryAccounts(ProgressState? progressState = null)
    {
        return await ChunkedTransfer.DownloadItems(Connection, delegate (ref PacketWriter writer, int offset, ushort maximumCount)
        {
            writer.Command = 0x64;
            writer.Write(offset);
            writer.Write(maximumCount);
        }, delegate (ref PacketReaderLarge reader, out TournamentAccount account)
        {
            if (!TournamentAccount.TryRead(this, ref reader, out TournamentAccount? parsedAccount) || parsedAccount is null)
            {
                account = null!;
                return false;
            }

            account = parsedAccount;
            return true;
        }, progressState, ChunkedTransfer.AccountChunkMaximumCount, "account query result").ConfigureAwait(false);
    }

    [Command(0xD0)]
    private void TournamentUpsert(PacketReader reader)
    {
        Tournament tournament = ReadTournament(reader);

        if (_tournament is null)
        {
            _tournament = tournament;
            PushEvent(new TournamentCreatedEvent(tournament));
            return;
        }

        Tournament previousTournament = _tournament;
        _tournament = tournament;
        PushEvent(new TournamentUpdatedEvent(previousTournament, tournament));
    }

    [Command(0xD1)]
    private void TournamentRemoved()
    {
        if (_tournament is null)
            throw new InvalidDataException("Server removed a tournament although none exists.");

        Tournament removedTournament = _tournament;
        _tournament = null;
        PushEvent(new TournamentRemovedEvent(removedTournament));
    }

    [Command(0xD2)]
    private void TournamentMessage(string message)
    {
        PushEvent(new TournamentMessageEvent(message));
    }

    private Tournament ReadTournament(PacketReader reader)
    {
        if (!reader.Read(out byte stageValue) ||
            !reader.Read(out byte modeValue) ||
            !reader.Read(out uint durationTicks) ||
            !reader.Read(out byte teamCount))
            throw new InvalidDataException("Couldn't read tournament header.");

        TournamentStage stage = (TournamentStage)stageValue;
        TournamentMode mode = (TournamentMode)modeValue;
        byte[] winningTeamIds;
        Team[] winningTeams;
        TournamentTeam[] teams = new TournamentTeam[teamCount];

        for (int teamIndex = 0; teamIndex < teams.Length; teamIndex++)
        {
            if (!reader.Read(out byte teamId) || !reader.Read(out byte participantCount))
                throw new InvalidDataException("Couldn't read tournament team header.");

            if (!Teams.TryGet(teamId, out Team? team) || team is null)
                throw new InvalidDataException($"Tournament references unknown team #{teamId}.");

            TournamentAccount[] participants = new TournamentAccount[participantCount];

            for (int participantIndex = 0; participantIndex < participants.Length; participantIndex++)
                if (!TournamentAccount.TryRead(this, reader, out TournamentAccount? account) || account is null)
                    throw new InvalidDataException("Couldn't read tournament participant.");
                else
                    participants[participantIndex] = account;

            teams[teamIndex] = new TournamentTeam(team, participants, 0);
        }

        if (!reader.Read(out byte historyCount))
            throw new InvalidDataException("Couldn't read tournament history count.");

        winningTeamIds = new byte[historyCount];
        winningTeams = new Team[historyCount];

        for (int historyIndex = 0; historyIndex < historyCount; historyIndex++)
        {
            if (!reader.Read(out byte winningTeamId))
                throw new InvalidDataException("Couldn't read tournament history entry.");

            if (!TryFindTournamentTeam(teams, winningTeamId, out Team? winningTeam) || winningTeam is null)
                throw new InvalidDataException($"Tournament history references unknown team #{winningTeamId}.");

            winningTeamIds[historyIndex] = winningTeamId;
            winningTeams[historyIndex] = winningTeam;
        }

        TournamentMatchResult[] matchHistory = new TournamentMatchResult[winningTeamIds.Length];

        for (int historyIndex = 0; historyIndex < winningTeamIds.Length; historyIndex++)
            matchHistory[historyIndex] = new TournamentMatchResult(historyIndex + 1, winningTeams[historyIndex]);

        TournamentTeam[] teamsWithWins = new TournamentTeam[teams.Length];

        for (int teamIndex = 0; teamIndex < teams.Length; teamIndex++)
        {
            int wins = 0;

            for (int historyIndex = 0; historyIndex < winningTeamIds.Length; historyIndex++)
                if (winningTeamIds[historyIndex] == teams[teamIndex].Team.Id)
                    wins++;

            TournamentAccount[] participants = new TournamentAccount[teams[teamIndex].Participants.Count];

            for (int participantIndex = 0; participantIndex < participants.Length; participantIndex++)
                participants[participantIndex] = teams[teamIndex].Participants[participantIndex];

            teamsWithWins[teamIndex] = new TournamentTeam(teams[teamIndex].Team, participants, wins);
        }

        return new Tournament(stage, mode, durationTicks, teamsWithWins, matchHistory);
    }

    private static bool TryFindTournamentTeam(TournamentTeam[] teams, byte teamId, out Team? team)
    {
        for (int teamIndex = 0; teamIndex < teams.Length; teamIndex++)
            if (teams[teamIndex].Team.Id == teamId)
            {
                team = teams[teamIndex].Team;
                return true;
            }

        team = null;
        return false;
    }
}
