using Lobby.Models.db;
using Lobby.Models.Games;
using Lobby.Models.Matchmaking;
using Lobby.Repository.Interface;
using Lobby.Util;
using Lobby.Validation;

namespace Lobby.Service;

public class GameService
{
    private readonly ILogger<GameService> _logger;
    private readonly IGameRepository _gameRepository;
    private readonly IGameRegimeRepository _gameRegimeRepository;
    private readonly UserService _userService;
    private readonly IValidationStorage _validationStorage;

    public GameService(ILogger<GameService> logger,
        IGameRepository gameRepository, IGameRegimeRepository gameRegimeRepository, 
        UserService userService, IValidationStorage validationStorage)
    {
        _logger = logger;
        _gameRepository = gameRepository;
        _gameRegimeRepository = gameRegimeRepository;
        _userService = userService;
        _validationStorage = validationStorage;
    }

    #region Actions

    public async Task<string?> DoResolveGameStart(Guid userId, ResolveGameStartRequest request, CancellationToken ct)
    {
        Game? game = await _gameRepository.GetById(request.GameId, ct);
        GameToUser? gameToUser = await _gameRepository.GetGameToUserById(userId, request.GameId, ct);
        if (request.Agreed)
        {
            return await ConfirmGame(game!, gameToUser!, ct);
        }
        else
        {
            await DeclineGame(game!, gameToUser!, ct);
            return null;
        }
    }

    public async Task DoCreateNewGame(StartGameRequest request, CancellationToken ct)
    {
        GameRegimeEntry? gre = await _gameRegimeRepository.GetGameRegimeEntryById(request.GameRegimeEntryId, ct);
        _logger.LogInformation("Creating new game for users {users}, game mode {mode} and game regime id {greId}", CollectionsUtil.FormatList(request.UserToTeam.Keys), gre.GameMode, gre.GameRegimeEntryId);
        _gameRepository.CreateNoSave(gre.GameRegimeEntryId, request.GameServerUrl, request.UserToTeam, request.UserToSquad);
    }

    public async Task<bool> DoFinishGame(Guid gameId, CancellationToken ct)
    {
        await ValidateFinishGame(gameId, ct);
        if (!_validationStorage.IsValid)
        {
            return false;
        }

        _logger.LogInformation($"Finishing the game with Id {gameId}");
        await _gameRepository.UpdateGameStatus(gameId, GameStatus.FINISHED, ct);
        return true;
    }

    private async Task<string> ConfirmGame(Game game, GameToUser gameToUser, CancellationToken ct)
    {
        gameToUser.IsConfirmedGameStart = true;
        // Check if everybody in the game has done so.
        bool isEverybodyConfirmed = await _gameRepository.CheckEverybodyConfirmedGameStart(game.GameId, ct);
        if (isEverybodyConfirmed)
        {
            await _gameRepository.UpdateGameStatus(game.GameId, GameStatus.IN_PROGRESS, ct);
        }
        return game.GameServerUrl;
    }

    private async Task DeclineGame(Game game, GameToUser gameToUser, CancellationToken ct)
    {
        gameToUser.IsConfirmedGameStart = false;
        // The user declines his partaking. Finalize the game.
        await _gameRepository.UpdateGameStatus(game.GameId, GameStatus.CANCELLED, ct);
    }

    public async Task<List<Guid>> GetUsersByGameId(Guid gameId, CancellationToken ct)
    {
        return await _gameRepository.GetUsersByGameId(gameId, ct);
    }

    public async Task<UserGameData?> GetActiveGameByUserId(Guid userId, CancellationToken ct)
    {
        return await _gameRepository.GetActiveGameByUserId(userId, ct);
    }

    #endregion

    #region Validation

    public async Task ValidateResolveGameStart(Guid userId, Guid gameId, CancellationToken ct)
    {
        Game? game = await _gameRepository.GetById(gameId, ct);
        if (game == null)
        {
            _validationStorage.AddError(ErrorCode.GameNotFound, string.Format("Cannot find game with Id {0}", gameId));
            return;
        }
        if (game!.GameStatus != GameStatus.CONFIRMATION)
        {
            _validationStorage.AddError(ErrorCode.WrongGameStatus, string.Format("Wrong status for game start resolve: expected {0}, got {1}", GameStatus.CONFIRMATION, game.GameStatus));
        }
        GameToUser? gameToUser = await _gameRepository.GetGameToUserById(userId, gameId, ct);
        if (gameToUser == null)
        {
            _validationStorage.AddError(ErrorCode.WrongGameIdForUser, string.Format("User {0} doesn't participate in game {1}", userId, gameId));
        }
    }

    public async Task ValidateBeginGameSearch(List<Guid> userIds, CancellationToken ct)
    {
        List<Guid> userIdsWithExistingGames = await _gameRepository.FilterUsersWithExistingGames(userIds, ct);
        if (userIdsWithExistingGames.Count > 0)
        {
            _validationStorage.AddError(ErrorCode.SomeUsersHaveActiveGames, "Cannot begin game search: some users have active games: " +
                CollectionsUtil.FormatList(userIdsWithExistingGames));
        }
    }

    public async Task ValidateStartGame(StartGameRequest request, CancellationToken ct)
    {
        Dictionary<Guid, int> userToTeam = request.UserToTeam;
        IEnumerable<Guid> userIds = userToTeam.Keys;
        List<User> users = await _userService.GetByIds(userIds, ct);
        if (users.Count != userIds.Count())
        {
            string errorText = string.Format("Expected {0} valid users, got {1}", userIds.Count(), users.Count);
            _validationStorage.AddError(ErrorCode.IncorrectCountValidUsers, errorText);
            _logger.LogError(errorText);
        }
        GameRegimeEntry? gre = await _gameRegimeRepository.GetGameRegimeEntryById(request.GameRegimeEntryId, ct);
        if (gre == null)
        {
            string errorText = string.Format("Couldn't find game regime by id {0}", request.GameRegimeEntryId);
            _validationStorage.AddError(ErrorCode.GameNotFound, errorText);
            _logger.LogError(errorText);
        }

        Dictionary<Guid, int> userToSquad = request.UserToSquad;
        if (!Enumerable.SequenceEqual(userToTeam.Keys.OrderBy(t => t), userToSquad.Keys.OrderBy(t => t)))
        {
            string errorText = string.Format("userToTeam and userToSquad are not synchronized! {0} vs {1}",
                CollectionsUtil.FormatDict(userToTeam, "userId", "team"),
                CollectionsUtil.FormatDict(userToSquad, "userId", "squad"));
            _validationStorage.AddError(ErrorCode.ConflictingInputData, errorText);
            _logger.LogError(errorText);
        }
    }

    private async Task ValidateFinishGame(Guid gameId, CancellationToken ct)
    {
        Game? game = await _gameRepository.GetById(gameId, ct);
        if (game == null)
        {
            string errorText = string.Format("Game with id {0} not found!", gameId);
            _validationStorage.AddError(ErrorCode.GameNotFound, errorText);
            _logger.LogError(errorText);
        }
    }

    #endregion

}
