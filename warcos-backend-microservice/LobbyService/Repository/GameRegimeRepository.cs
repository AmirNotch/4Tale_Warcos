using Lobby.Models;
using Lobby.Models.Matchmaking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Lobby.Models.db;
using Lobby.Repository.Interface;

namespace Lobby.Repository;

public class GameRegimeRepository : IGameRegimeRepository
{
    private readonly ILogger<GameRegimeRepository> _logger;
    private readonly WarcosLobbyDbContext _dbContext;

    public GameRegimeRepository(ILogger<GameRegimeRepository> logger, WarcosLobbyDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<GameRegimeEntry?> GetGameRegimeEntryById(Guid gameRegimeEntryId, CancellationToken ct)
    {
        return await _dbContext.GameRegimeEntries
            .FirstOrDefaultAsync(gre => gre.GameRegimeEntryId == gameRegimeEntryId, ct);
    }

    public async Task<GameMode?> GetGameModeById(string gameMode, CancellationToken ct)
    {
        return await _dbContext.GameModes
            .FirstOrDefaultAsync(gm => gm.GameModeKind == gameMode, ct);
    }

    public async Task<List<GameMode>> GetAllGameModes(CancellationToken ct)
    {
        return await _dbContext.GameModes.ToListAsync(ct);
    }

    public async Task<List<EnrichedGameRegimeDto>> GetUpcomingGameRegimesByMode(
        string gameMode, DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes, CancellationToken ct)
    {
        var query = GetEnrichedGameRegimeEntities()
            .Where(GetIntervalCondition(gameMode, moment, intervalMinutes, timeFrameMinutes))
            .OrderBy(entry => entry.Sgr.IntervalStart);

        return await TransformToDto(query).ToListAsync(ct);
    }

    public async Task<EnrichedGameRegimeDto?> GetCurrentScheduledGameRegime(
        string gameMode, DateTimeOffset moment, CancellationToken ct)
    {
        var query = GetEnrichedGameRegimeEntities()
            .Where(GetMomentCondition(gameMode, moment));

        return await TransformToDto(query).FirstOrDefaultAsync(ct);
    }

    public async Task<List<ScheduledGameRegime>> GetAllUpcomingGameRegimes(
        DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes, CancellationToken ct)
    {
        return await _dbContext.ScheduledGameRegimes
            .Where(sgr => sgr.IntervalStart >= moment.AddMinutes(-intervalMinutes) && sgr.IntervalStart < moment.AddMinutes(timeFrameMinutes))
            .OrderBy(sgr => sgr.IntervalStart)
            .ToListAsync(ct);
    }

    public async Task<List<GameRegimeEntry>> GetAllGameRegimeEntries(CancellationToken ct)
    {
        return await _dbContext.GameRegimeEntries.ToListAsync(ct);
    }

    public async Task CreateScheduledGameRegimes(List<ScheduledGameRegime> newSgrs, CancellationToken ct)
    {
        _dbContext.ScheduledGameRegimes.AddRange(newSgrs);
        await _dbContext.SaveChangesAsync(ct);
    }

    private IQueryable<EnrichedGameRegimeEntities> GetEnrichedGameRegimeEntities()
    {
        return _dbContext.ScheduledGameRegimes
            .Join(_dbContext.GameRegimeEntries, sgr => sgr.GameRegimeEntryId, gre => gre.GameRegimeEntryId, 
                (sgr, gre) => new { sgr, gre })
            .Join(_dbContext.GameModes, entry => entry.sgr.GameMode, gm => gm.GameModeKind, 
                (entry, gm) => new EnrichedGameRegimeEntities { Sgr = entry.sgr, Gre = entry.gre, Gm = gm });
    }

    private static IQueryable<EnrichedGameRegimeDto> TransformToDto(IQueryable<EnrichedGameRegimeEntities> query)
    {
        return query.Select(entry => new EnrichedGameRegimeDto
        {
            GameMode = entry.Sgr.GameMode,
            GameRegime = entry.Gre.GameRegime,
            MapKind = entry.Gre.MapKind,
            GameRegimeEntryId = entry.Gre.GameRegimeEntryId,
            NumberOfPlayers = entry.Gm.NumberOfPlayers,
            IsTeam = entry.Gm.IsTeam,
            IntervalStart = entry.Sgr.IntervalStart,
            IntervalEnd = entry.Sgr.IntervalEnd,
        });
    }

    private static Expression<Func<EnrichedGameRegimeEntities, bool>> GetIntervalCondition(
        string gameMode, DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes)
    {
        return entry => entry.Sgr.GameMode == gameMode &&
                        entry.Sgr.IntervalStart >= moment.AddMinutes(-intervalMinutes) &&
                        entry.Sgr.IntervalStart < moment.AddMinutes(timeFrameMinutes);
    }

    private static Expression<Func<EnrichedGameRegimeEntities, bool>> GetMomentCondition(string gameMode, DateTimeOffset moment)
    {
        return entry => entry.Sgr.GameMode == gameMode &&
                        entry.Sgr.IntervalStart <= moment &&
                        entry.Sgr.IntervalEnd > moment;
    }

    private class EnrichedGameRegimeEntities
    {
        public ScheduledGameRegime Sgr { get; set; }
        public GameRegimeEntry Gre { get; set; }
        public GameMode Gm { get; set; }
    }
}
