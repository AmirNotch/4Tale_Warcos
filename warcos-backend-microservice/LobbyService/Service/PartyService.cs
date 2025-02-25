using Lobby.Models.db;
using Lobby.Models.Matchmaking;
using Lobby.Models.Parties;
using Lobby.Service.OpenMatch;
using Lobby.Repository.Interface;
using Lobby.Util;
using Lobby.Validation;
using Microsoft.EntityFrameworkCore;
using OpenMatch;

namespace Lobby.Service;

public class PartyService
{
    private readonly ILogger<PartyService> _logger;
    private readonly IOpenMatchTicketService _openMatchTicketService;
    private readonly IPartyRepository _partyRepository;
    private readonly IGameRegimeRepository _gameRegimeRepository;
    private readonly IValidationStorage _validationStorage;

    public PartyService(ILogger<PartyService> logger, IOpenMatchTicketService openMatchTicketService, 
        IPartyRepository partyRepository, IGameRegimeRepository gameRegimeRepository, 
        IValidationStorage validationStorage)
    {
        _logger = logger;
        _partyRepository = partyRepository;
        _openMatchTicketService = openMatchTicketService;
        _gameRegimeRepository = gameRegimeRepository;
        _validationStorage = validationStorage;
    }

    #region Actions

    public async Task<string> DoBeginGameSearch(Guid leaderUserId, List<Guid> userIds, string gameModeType, CancellationToken ct)
    {
        _logger.LogInformation("Creating a new ticket for game mode: {gameMode}", gameModeType);
        Ticket ticket = await _openMatchTicketService.Create(gameModeType, userIds);
        _logger.LogInformation("Created ticket id: {assign}", ticket.Id);

        try
        {
            //TODO add party confirmation stage
            await _partyRepository.CreateParty(ticket.Id, leaderUserId, userIds, ct);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while updating database");
            await _openMatchTicketService.RemoveByTicketId(ticket.Id);
            throw;
        }
        return ticket.Id;
    }

    public async Task DoCancelGameSearch(Guid partyId, CancellationToken ct)
    {
        Party? party = await _partyRepository.GetById(partyId, ct);
        await _partyRepository.RemovePartyById(partyId, ct);
        await _openMatchTicketService.RemoveByTicketId(party!.TicketId);
        _logger.LogInformation("Party successfully removed: {partyId}", partyId.ToString());
    }

    public async Task<bool> CancelGameSearchByTicketIds(List<string> ticketIds, CancellationToken ct)
    {
        _logger.LogInformation("Cancelling game search for tickets {ticketIds}", string.Join(", ", ticketIds));
        // No validation for ticketIds on purpose:
        // if at the time of the request from director there is no party - so be it for the greater good
        List<Party> parties = await _partyRepository.GetByTicketIds(ticketIds, ct);
        await _partyRepository.CancelManyGameSearches(parties, ct);
        List<Task> tasks = parties.Select(party => _openMatchTicketService.RemoveByTicketId(party.TicketId)).ToList();
        _logger.LogInformation($"Game search successfully cancelled for tickets: {string.Join(", ", ticketIds)}");
        return true;
    }

    public async Task<UserPartyData?> GetActivePartyByUserId(Guid userId, CancellationToken ct)
    {
        return await _partyRepository.GetActivePartyByUserId(userId, ct);
    }

    public async Task<List<Party>> GetPartiesWithoutGames(CancellationToken ct)
    {
        return await _partyRepository.GetPartiesWithoutGames(ct);
    }

    public async Task<List<Guid>> GetUserIdsByPartyId(Guid partyId, CancellationToken ct)
    {
        return await _partyRepository.GetUserIdsByPartyId(partyId, ct);
    }

    public async Task CheckActiveParty(Guid userId, CancellationToken ct)
    {
        Guid? activePartyId = await _partyRepository.GetActivePartyIdByUserId(userId, ct);
        if (activePartyId == null)
        {
            _validationStorage.AddError(ErrorCode.UserHasNoActiveParty, $"User with userId {userId} has no active party");
        };
    }

    public async Task<bool> CheckBeginGameSearch(List<User> users, List<Guid> userIds, Guid leaderUserId, string gameModeKind, CancellationToken ct)
    {
        List<Guid> userIdsWithExistingParties = await _partyRepository.FilterUsersWithExistingParties(userIds, ct);
        if (userIdsWithExistingParties.Count > 0)
        {
            _validationStorage.AddError(ErrorCode.SomeUsersHaveActiveParties, "Cannot begin game search: some users have active parties: " +
                CollectionsUtil.FormatList(userIdsWithExistingParties));
        }
        // Check game mode
        GameMode? gameMode = await _gameRegimeRepository.GetGameModeById(gameModeKind, ct);
        if (gameMode == null)
        {
            _validationStorage.AddError(ErrorCode.GameModeNotFound, $"Cannot begin game search: game mode {gameModeKind} not found!");
        }
        return _validationStorage.IsValid;
    }

    public async Task RemoveTickets(IEnumerable<string> ticketIds, CancellationToken ct)
    {
        await _partyRepository.RemovePartiesByTicketIds(ticketIds, ct);
        _logger.LogInformation("Removing open match tickets {tickets}", ticketIds);
        await Task.WhenAll(ticketIds.Select(_openMatchTicketService.RemoveByTicketId));
    }

    public async Task<Guid?> GetActivePartyIdByUserId(Guid userId, CancellationToken ct)
    {
        return await _partyRepository.GetActivePartyIdByUserId(userId, ct);
    }

    #endregion
    
    #region Validation

    public async Task ValidateStartGame(StartGameRequest request, CancellationToken ct)
    {
        IEnumerable<Guid> userIds = request.UserToTeam.Keys;
        List<Party> parties = await _partyRepository.GetByTicketIds(request.TicketIds, ct);
        if (parties.Count != request.TicketIds.Count())
        {
            _validationStorage.AddError(ErrorCode.IncorrectCountTickets,
                string.Format("Expected {0} valid tickets, got {1}", request.TicketIds.Count(), parties.Count));
        }

        int playersCount = parties.Select(x => x.Size).Sum();
        if (playersCount != userIds.Count())
        {
            _validationStorage.AddError(ErrorCode.IncorrectCountPlayersInTeam, string.Format(
                "PlayerCount = {0} according to UserToTeam, but it = {1} according to TicketIds!",
                userIds.Count(), playersCount));
        }
    }

    #endregion
}