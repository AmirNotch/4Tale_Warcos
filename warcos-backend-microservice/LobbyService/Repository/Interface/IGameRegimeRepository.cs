using Lobby.Models.db;
using Lobby.Models.Matchmaking;

namespace Lobby.Repository.Interface;

public interface IGameRegimeRepository
{
    Task<GameRegimeEntry?> GetGameRegimeEntryById(Guid gameRegimeEntryId, CancellationToken ct);
    Task<GameMode?> GetGameModeById(string gameMode, CancellationToken ct);
    Task<List<GameMode>> GetAllGameModes(CancellationToken ct);
    Task<List<EnrichedGameRegimeDto>> GetUpcomingGameRegimesByMode(string gameMode, DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes, CancellationToken ct);
    Task<EnrichedGameRegimeDto?> GetCurrentScheduledGameRegime(string gameMode, DateTimeOffset moment, CancellationToken ct);
    Task<List<ScheduledGameRegime>> GetAllUpcomingGameRegimes(DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes, CancellationToken ct);
    Task<List<GameRegimeEntry>> GetAllGameRegimeEntries(CancellationToken ct);
    Task CreateScheduledGameRegimes(List<ScheduledGameRegime> newSgrs, CancellationToken ct);
}
