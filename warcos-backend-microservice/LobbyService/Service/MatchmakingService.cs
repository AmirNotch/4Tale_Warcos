using Lobby.Models.db;
using Lobby.Models.Games;
using Lobby.Models.Matchmaking;
using Lobby.Models.Parties;
using Lobby.Models.WsMessage;
using Lobby.Service.OpenMatch;
using Lobby.Util;
using Lobby.Validation;
using Newtonsoft.Json;
using System.Net.WebSockets;

using static Lobby.Util.WebSocketUtils;

namespace Lobby.Service;

public class MatchmakingService : AbstractWebSocketService<MatchmakingEndpointKind>
{
    private readonly IOpenMatchTicketService _openMatchTicketService;
    private readonly GameService _gameService;
    private readonly GameRegimeService _gameRegimeService;
    private readonly PartyService _partyService;
    private readonly UserService _userService;

    public MatchmakingService(ILogger<MatchmakingService> logger,
        IValidationStorage validationStorage,
        IOpenMatchTicketService openMatchTicketService,
        GameRegimeService gameRegimeService,
        GameService gameService,
        PartyService partyService,
        UserService userService,
        JsonSerializerSettings jsonSerializerSettings) :
        base(EndpointCategory.Matchmaking, validationStorage, logger, jsonSerializerSettings)
    {
        _openMatchTicketService = openMatchTicketService;
        _gameRegimeService = gameRegimeService;
        _gameService = gameService;
        _partyService = partyService;
        _userService = userService;
    }

    protected override async Task<IOutputMessageData?> ProcessWebSocketMessage(Guid userId, MatchmakingEndpointKind endpointKind, string? data, CancellationToken ct)
    {
        IOutputMessageData? outputMessage = null;
        switch (endpointKind)
        {
            case MatchmakingEndpointKind.GetMyParty:
                outputMessage = await GetMyParty(userId, ct);
                break;
            case MatchmakingEndpointKind.GetUpcomingGameRegimes:
                outputMessage = await GetUpcomingGameRegimes(JsonConvert.DeserializeObject<GetUpcomingGameRegimesRequest>(data!), ct);
                break;
            case MatchmakingEndpointKind.BeginGameSearch:
                outputMessage = await BeginGameSearch(userId, JsonConvert.DeserializeObject<BeginGameSearchRequest>(data!), ct);
                break;
            case MatchmakingEndpointKind.CancelGameSearch:
                await CancelGameSearch(userId, ct);
                break;
            case MatchmakingEndpointKind.ResolveGameStart:
                await ResolveGameStart(userId, JsonConvert.DeserializeObject<ResolveGameStartRequest>(data!), ct);
                break;
            default:
                _validationStorage.AddError(ErrorCode.WrongEndpointKind, $"Unknown event type for category \"{_endpointCategory}\": \"{data}\"");
                break;
        }
        return outputMessage;
    }

    #region Actions

    private async Task<GetUpcomingGameRegimesResponse> GetUpcomingGameRegimes(GetUpcomingGameRegimesRequest? request, CancellationToken ct)
    {
        bool isValid = await ValidateGetUpcomingGameRegimes(request, ct);
        if (!isValid)
        {
            return null!;
        }
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<EnrichedGameRegimeDto> upcomingGameRegimes = await _gameRegimeService.GetUpcomingGameRegimesByGameMode(request!.GameMode, now, ct);
        return new GetUpcomingGameRegimesResponse
        {
            UpcomingGameRegimes = upcomingGameRegimes
        };
    }

    private async Task<PartyInfo> GetMyParty(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Getting party'n'game info for userId {userId}", userId);
        UserPartyData? activeParty = await _partyService.GetActivePartyByUserId(userId, ct);
        UserGameData? activeGame = await _gameService.GetActiveGameByUserId(userId, ct);
        PartyInfo partyInfo = new()
        {
            PartyId = activeParty?.PartyId,
            IsPartyConfirmed = activeParty?.IsPartyConfirmed ?? false,
            GameId = activeGame?.GameId,
            GameServerUrl = activeGame?.GameServerUrl,
            IsGameConfirmed = activeGame?.IsGameConfirmed ?? false,
        };
        return partyInfo;
    }

    private async Task<BeginGameSearchResponse> BeginGameSearch(Guid leaderUserId, BeginGameSearchRequest? request, CancellationToken ct)
    {
        bool isValid = await ValidateBeginGameSearch(leaderUserId, request, ct);
        if (!isValid)
        {
            return null!;
        }

        _logger.LogInformation($"Beginning game search for userId {leaderUserId}, game mode {request!.GameMode} " +
            $"and users {CollectionsUtil.FormatList(request.UserIds)}");
        string ticketId = await _partyService.DoBeginGameSearch(leaderUserId, request.UserIds, request.GameMode, ct);

        List<Guid> userIdsToSendUpdates = new(request.UserIds);
        userIdsToSendUpdates.Remove(leaderUserId);
        if (userIdsToSendUpdates.Count > 0)
        {
            await SendGetMyPartyUpdates(userIdsToSendUpdates, ct);
        }
        return new BeginGameSearchResponse
        {
            TicketId = ticketId,
        };
    }

    public async Task CancelGameSearch(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation($"Cancelling game search for userId {userId}");
        bool isValid = await ValidateCancelGameSearch(userId, ct);
        if (!isValid)
        {
            return;
        }

        await DoCancelGameSearch(userId, ct);
    }

    private async Task DoCancelGameSearch(Guid userId, CancellationToken ct)
    {
        Guid partyId = (await _partyService.GetActivePartyIdByUserId(userId, ct)).Value;
        List<Guid> userIdsToSendUpdates = await _partyService.GetUserIdsByPartyId(partyId, ct);
        userIdsToSendUpdates.Remove(userId);
        await _partyService.DoCancelGameSearch(partyId, ct);
        if (userIdsToSendUpdates.Count > 0)
        {
            await SendGetMyPartyUpdates(userIdsToSendUpdates, ct);
        }
    }

    private async Task<ResolveGameStartResponse> ResolveGameStart(Guid userId, ResolveGameStartRequest? request, CancellationToken ct)
    {
        bool isValid = await ValidateResolveGameStart(userId, request, ct);
        if (!isValid)
        {
            return null!;
        }
        _logger.LogInformation($"Resolving game start for userId = {userId}, gameId = {request!.GameId}, agreed = {request.Agreed}");
        string? gameServerUrl = await _gameService.DoResolveGameStart(userId, request!, ct);

        if (!request.Agreed)
        {
            List<Guid> userIdsToSendUpdates = await _gameService.GetUsersByGameId(request!.GameId, ct);
            userIdsToSendUpdates.Remove(userId);
            await SendGetMyPartyUpdates(userIdsToSendUpdates, ct);
        }
        return new ResolveGameStartResponse
        {
            GameServerUrl = gameServerUrl,
        };
    }

    public async Task SendGetMyPartyUpdates(ICollection<Guid> userIds, CancellationToken ct)
    {
        await Task.WhenAll(userIds.Select(userId => SendGetMyPartyUpdate(userId, ct)));
    }

    public async Task SendGetMyPartyUpdate(Guid userId, CancellationToken ct)
    {
        try
        {
            WebSocket? ws = WebSocketConnectionManager.GetSocketByUserId(userId);
            if (ws == null)
            {
                _logger.LogInformation("Cannot send update to user {userId} cause no connection is held", userId);
                return;
            }

            PartyInfo partyInfo = await GetMyParty(userId, ct);
            await SendMessageToUser(ws, userId, _logger, MatchmakingEndpointKind.GetMyParty.ToString(),
                EndpointCategory.Matchmaking.ToString(), partyInfo, _jsonSerializerSettings, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending getMyParty update to user {userId}", userId);
        }
    }

    #endregion

    #region Validation

    private async Task<bool> ValidateBeginGameSearch(Guid leaderUserId, BeginGameSearchRequest? request, CancellationToken ct)
    {
        bool isValid = ValidationUtils.BasicWsValidation(request, _validationStorage);
        if (!isValid)
        {
            return false;
        }
        if (request!.UserIds == null || request.UserIds.Count == 0)
        {
            _validationStorage.AddError(ErrorCode.EmptyUserIds, "Cannot begin game search: userIds are empty!");
            return false;
        }
        if (!request.UserIds.Contains(leaderUserId))
        {
            _validationStorage.AddError(ErrorCode.UserIdsDoNotIncludeUser, "Cannot begin game search: userIds don't include the user!");
        }
        List<User> users = await _userService.GetByIds(request.UserIds, ct);
        if (users.Count != request.UserIds.Count)
        {
            _validationStorage.AddError(ErrorCode.NotAllUsersAreReal,
                $"Cannot begin game search: expected {request.UserIds.Count} real users, got {users.Count}!");
        }
        await _gameService.ValidateBeginGameSearch(request.UserIds, ct);
        return await _partyService.CheckBeginGameSearch(users, request.UserIds, leaderUserId, request.GameMode, ct);
    }

    private async Task<bool> ValidateCancelGameSearch(Guid userId, CancellationToken ct)
    {
        await _partyService.CheckActiveParty(userId, ct);
        return _validationStorage.IsValid;
    }

    private async Task<bool> ValidateGetUpcomingGameRegimes(GetUpcomingGameRegimesRequest? request, CancellationToken ct)
    {
        bool isValid = ValidationUtils.BasicWsValidation(request, _validationStorage);
        if (!isValid)
        {
            return false;
        }
        GameMode? gameMode = await _gameRegimeService.GetGameModeById(request!.GameMode, ct);
        if (gameMode is null)
        {
            _validationStorage.AddError(ErrorCode.GameModeNotFound, $"Game mode {request.GameMode} not found");
        }
        return _validationStorage.IsValid;
    }

    private async Task<bool> ValidateResolveGameStart(Guid userId, ResolveGameStartRequest? request, CancellationToken ct)
    {
        bool isValid = ValidationUtils.BasicWsValidation(request, _validationStorage);
        if (!isValid)
        {
            return false;
        }
        await _gameService.ValidateResolveGameStart(userId, request!.GameId, ct);
        return _validationStorage.IsValid;
    }

    #endregion
}
