using Lobby.Models.db;
using Lobby.Models.Constant;
using Lobby.Service;

namespace Lobby.Background;

public class GameModeFillerJob(IServiceScopeFactory serviceScopeFactory, ILogger<GameModeFillerJob> logger) : BackgroundService {
    private static readonly int gameModeFillerWaitingTimeInMinutes = 5;

    private const string className = nameof(GameModeFillerJob);
    private readonly ILogger<GameModeFillerJob> _logger = logger;
    private readonly Random _random = new Random();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("{name} is running.", className);
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(gameModeFillerWaitingTimeInMinutes));
        await DoWorkAsync(stoppingToken);
        while (await timer.WaitForNextTickAsync(stoppingToken)) {
            await DoWorkAsync(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("{Name} is stopping.", className);
        await base.StopAsync(stoppingToken);
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken) {
        try {
            using (IServiceScope scope = serviceScopeFactory.CreateScope()) {
                GameRegimeService gameRegimeService = scope.ServiceProvider.GetRequiredService<GameRegimeService>();
                await DoWorkAsync(gameRegimeService, stoppingToken);
            }
        } catch (OperationCanceledException) {
            _logger.LogInformation("{name} is stopping.", className);
            Environment.Exit(1);
        } catch (Exception ex) {
            _logger.LogError(ex, "{name} encountered error", className);
        }
    }

    private async Task DoWorkAsync(GameRegimeService gameRegimeService, CancellationToken ct) {
        Dictionary<string, List<ScheduledGameRegime>> gameModeToSgrs = await gameRegimeService.getAllUpcomingGameRegimes(
            DateTimeOffset.UtcNow, LobbyConstants.GameModeFillIntervalInMinutes, LobbyConstants.GameModeFillTimeFrameInMinutes, ct);
        var modeToRegimeToEntries = await gameRegimeService.GetGameRegimeEntriesByModeByRegime(ct);
        List<ScheduledGameRegime> newSgrs = [];
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset intervalStartLimit = now.AddMinutes(LobbyConstants.GameModeFillTimeFrameInMinutes);
        GameRegimeEntry? previousEntry = null;
        foreach (string gameMode in modeToRegimeToEntries.Keys) {
            if (!gameModeToSgrs.ContainsKey(gameMode)) {
                // No scheduled regimes, so we need to create them from scratch
                newSgrs.AddRange(scheduleMoreRegimes(now, intervalStartLimit, null, modeToRegimeToEntries, gameMode));
            } else {
                List<ScheduledGameRegime> existingSgrs = gameModeToSgrs[gameMode];
                ScheduledGameRegime lastScheduled = existingSgrs[^1];
                previousEntry = lastScheduled.GameRegimeEntryNavigation;
                DateTimeOffset intervalStart = lastScheduled.IntervalStart.AddMinutes(LobbyConstants.GameModeFillIntervalInMinutes);
                newSgrs.AddRange(scheduleMoreRegimes(intervalStart, intervalStartLimit, previousEntry, modeToRegimeToEntries, gameMode));
            }
        }
        if (newSgrs.Count > 0) {
            _logger.LogInformation("Adding {number} new scheduled game regimes.", newSgrs.Count);
            await gameRegimeService.CreateScheduledGameRegimes(newSgrs, ct);
        }
    }

    private GameRegimeEntry getRandomEntryForGameMode(string gameMode, Dictionary<string, Dictionary<string, List<GameRegimeEntry>>> modeToRegimeToEntries,
                                                      GameRegimeEntry? previousEntry) {
        Dictionary<string, List<GameRegimeEntry>> gameRegimeToEntries = modeToRegimeToEntries[gameMode];
        if (gameRegimeToEntries.Count == 1) {
            return getRandomEntryForGameRegime(gameRegimeToEntries.First().Key, gameRegimeToEntries, previousEntry);
        } else {
            string chosenGameRegime;
            do {
                chosenGameRegime = gameRegimeToEntries.ElementAt(_random.Next(0, gameRegimeToEntries.Count)).Key;
            } while (previousEntry != null && chosenGameRegime == previousEntry.GameRegime);
            return getRandomEntryForGameRegime(chosenGameRegime, gameRegimeToEntries, previousEntry);
        }
    }

    private GameRegimeEntry getRandomEntryForGameRegime(string gameRegime, Dictionary<string, List<GameRegimeEntry>> gameRegimeToEntries,
                                                        GameRegimeEntry? previousEntry) {
        List<GameRegimeEntry> entries = gameRegimeToEntries[gameRegime];
        if (entries.Count == 1) {
            return entries[0];
        } else {
            GameRegimeEntry chosenEntry;
            do {
                chosenEntry = entries[_random.Next(entries.Count)];
            } while (previousEntry != null && chosenEntry.GameRegime == previousEntry.GameRegime && chosenEntry.MapKind == previousEntry.MapKind);
            return chosenEntry;
        }
    }

    private List<ScheduledGameRegime> scheduleMoreRegimes(
        DateTimeOffset intervalStart, DateTimeOffset intervalStartLimit, GameRegimeEntry? previousEntry,
        Dictionary<string, Dictionary<string, List<GameRegimeEntry>>> modeToRegimeToEntries, string gameMode
    ) {
        List<ScheduledGameRegime> newSgrs = [];
        DateTimeOffset intervalEnd;
        while (intervalStart <= intervalStartLimit) {
            intervalEnd = intervalStart.AddMinutes(LobbyConstants.GameModeFillIntervalInMinutes);
            GameRegimeEntry newEntry = getRandomEntryForGameMode(gameMode, modeToRegimeToEntries, previousEntry);
            newSgrs.Add(entryToSgr(newEntry, intervalStart, intervalEnd));
            previousEntry = newEntry;

            intervalStart = intervalEnd;
        }
        return newSgrs;
    }

    private ScheduledGameRegime entryToSgr(GameRegimeEntry entry, DateTimeOffset intervalStart, DateTimeOffset intervalEnd) {
        return new ScheduledGameRegime {
            GameRegimeEntryId = entry.GameRegimeEntryId,
            GameMode = entry.GameMode,
            GameRegime = entry.GameRegime,
            MapKind = entry.MapKind,
            IntervalStart = intervalStart,
            IntervalEnd = intervalEnd,
        };
    }
}