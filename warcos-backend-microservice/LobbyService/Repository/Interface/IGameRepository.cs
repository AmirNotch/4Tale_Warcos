using Lobby.Models.db;
using Lobby.Models.Games;

namespace Lobby.Repository.Interface;

public interface IGameRepository
{
    Task<Game?> GetById(Guid gameId, CancellationToken ct);
    Task<GameToUser?> GetGameToUserById(Guid userId, Guid gameId, CancellationToken ct);
    Task<List<Guid>> FilterUsersWithExistingGames(List<Guid> userIds, CancellationToken ct);
    Task<List<Guid>> GetUsersByGameId(Guid gameId, CancellationToken ct);
    Task<bool> CheckEverybodyConfirmedGameStart(Guid gameId, CancellationToken ct);
    void CreateNoSave(Guid gameRegimeEntryId, string gameServerUrl, Dictionary<Guid, int> userIdToTeam, Dictionary<Guid, int> userIdToSquad);
    Task UpdateGameStatus(Guid gameId, GameStatus newStatus, CancellationToken ct);
    Task<UserGameData?> GetActiveGameByUserId(Guid userId, CancellationToken ct);
}
