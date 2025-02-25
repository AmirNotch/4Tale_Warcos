using Lobby.Models;
using Lobby.Models.db;
using Lobby.Models.Games;
using Lobby.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Lobby.Repository;

public class GameRepository : IGameRepository
{
    private readonly ILogger<GameRepository> _logger;
    private readonly WarcosLobbyDbContext _dbContext;
    private readonly static List<GameStatus> terminalGameStatuses = new List<GameStatus> 
    { 
        GameStatus.FINISHED, 
        GameStatus.CANCELLED 
    };

    public GameRepository(ILogger<GameRepository> logger, WarcosLobbyDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Game?> GetById(Guid gameId, CancellationToken ct)
    {
        _logger.LogInformation($"Getting game by id {gameId}");
        return await _dbContext.Games
            .Where(x => x.GameId == gameId && !terminalGameStatuses.Contains(x.GameStatus))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<GameToUser?> GetGameToUserById(Guid userId, Guid gameId, CancellationToken ct)
    {
        _logger.LogInformation($"Getting gameToUser by gameId = {gameId}, userId = {userId}");
        return await _dbContext.GameToUsers
            .Where(x => x.GameId == gameId && x.UserId == userId && !x.Removed.HasValue)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Guid>> FilterUsersWithExistingGames(List<Guid> userIds, CancellationToken ct)
    {
        _logger.LogInformation($"Filtering users with existing games. UserIds = {string.Join(", ", userIds)}");
        return await _dbContext.GameToUsers
            .Where(g2u => userIds.Contains(g2u.UserId) && !g2u.Removed.HasValue)
            .Select(user => user.UserId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<List<Guid>> GetUsersByGameId(Guid gameId, CancellationToken ct)
    {
        _logger.LogInformation($"Getting user ids by gameId = {gameId}");
        return await _dbContext.GameToUsers
            .Where(g2u => g2u.GameId == gameId && !g2u.Removed.HasValue)
            .Select(user => user.UserId)
            .ToListAsync(ct);
    }

    public async Task<bool> CheckEverybodyConfirmedGameStart(Guid gameId, CancellationToken ct)
    {
        var gamesToUsers = await _dbContext.GameToUsers
            .Where(x => x.GameId.Equals(gameId))
            .ToListAsync(ct);
        return gamesToUsers.All(x => x.IsConfirmedGameStart);
    }

    public void CreateNoSave(Guid gameRegimeEntryId, string gameServerUrl, Dictionary<Guid, int> userIdToTeam, Dictionary<Guid, int> userIdToSquad)
    {
        Guid gameId = Guid.NewGuid();
        Game game = new()
        {
            GameId = gameId,
            GameRegimeEntryId = gameRegimeEntryId,
            GameServerUrl = gameServerUrl,
            GameStatus = GameStatus.IN_PROGRESS  // DisableConfirmation: we skip confirmation phase and start at IN_PROGRESS
        };

        List<GameToUser> gameToUsersList = new();
        foreach (Guid userId in userIdToTeam.Keys)
        {
            gameToUsersList.Add(new GameToUser
            {
                GameId = gameId,
                UserId = userId,
                Team = userIdToTeam[userId],
                Squad = userIdToSquad[userId],
                IsConfirmedGameStart = true, // DisableConfirmation: game_to_user is born with confirmation already set
            });
        }

        _dbContext.Games.Add(game);
        _dbContext.GameToUsers.AddRange(gameToUsersList);
    }

    public async Task UpdateGameStatus(Guid gameId, GameStatus newStatus, CancellationToken ct)
    {
        Game? game = await GetById(gameId, ct);

        if (game != null)
        {
            game.GameStatus = newStatus;
            if (terminalGameStatuses.Contains(newStatus))
            {
                var gamesToUsers = await _dbContext.GameToUsers
                    .Where(x => x.GameId.Equals(game.GameId))
                    .ToListAsync(ct);
                gamesToUsers.ForEach(gameToUser => gameToUser.Removed = DateTimeOffset.UtcNow);
            }
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<UserGameData?> GetActiveGameByUserId(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation($"Getting active game data by user id {userId}");
        return await _dbContext.GameToUsers
            .Join(_dbContext.Games,
                gameToUser => gameToUser.GameId,
                game => game.GameId,
                (gameToUser, game) => new { gameToUser, game })
            .Where(v => v.gameToUser.UserId == userId && !terminalGameStatuses.Contains(v.game.GameStatus))
            .Select(v => new UserGameData
            {
                GameId = v.game.GameId,
                IsGameConfirmed = v.gameToUser.IsConfirmedGameStart,
                GameServerUrl = v.game.GameServerUrl,
            })
            .FirstOrDefaultAsync(ct);
    }
}

