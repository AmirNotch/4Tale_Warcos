using Lobby.Models.Matchmaking;
using Lobby.Validation;

namespace Lobby.Service;

public class StartGameService
{
    private readonly ILogger<StartGameService> _logger;
    private readonly GameService _gameService;
    private readonly GameRegimeService _gameRegimeService;
    private readonly MatchmakingService _matchmakingService;
    private readonly PartyService _partyService;
    private readonly IValidationStorage _validationStorage;

    public StartGameService(ILogger<StartGameService> logger, GameService gameService,
        GameRegimeService gameRegimeService, MatchmakingService matchmakingService,
        PartyService partyService, IValidationStorage validationStorage)
    {
        _logger = logger;
        _gameService = gameService;
        _gameRegimeService = gameRegimeService;
        _matchmakingService = matchmakingService;
        _partyService = partyService;
        _validationStorage = validationStorage;
    }

    public async Task<bool> StartGame(StartGameRequest request, CancellationToken ct)
    {
        await _partyService.ValidateStartGame(request, ct);
        await _gameService.ValidateStartGame(request, ct);
        if (!_validationStorage.IsValid)
        {
            return false;
        }

        await _gameService.DoCreateNewGame(request, ct);
        await _partyService.RemoveTickets(request.TicketIds, ct);
        await _matchmakingService.SendGetMyPartyUpdates(request.UserToTeam.Keys, ct);
        return true;
    }
}