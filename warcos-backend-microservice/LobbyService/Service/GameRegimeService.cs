using Lobby.Models.Constant;
using Lobby.Models.db;
using Lobby.Models.Matchmaking;
using Lobby.Repository.Interface;
using Lobby.Validation;

namespace Lobby.Service;

public class GameRegimeService
{
    private readonly ILogger<GameRegimeService> _logger;
    private readonly IGameRegimeRepository _gameRegimeRepository;
    private readonly IValidationStorage _validationStorage;

    public GameRegimeService(ILogger<GameRegimeService> logger, IValidationStorage validationStorage, IGameRegimeRepository gameRegimeRepository)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _gameRegimeRepository = gameRegimeRepository;
    }

    #region Actions

    public async Task<EnrichedGameRegimeDto?> GetCurrentGameRegime(string gameModeKind, CancellationToken ct)
    {
        DateTimeOffset moment = DateTimeOffset.UtcNow;
        EnrichedGameRegimeDto? scheduledGameRegime = await _gameRegimeRepository.GetCurrentScheduledGameRegime(gameModeKind, moment, ct);
        if (!ValidateGetCurrentGameRegime(scheduledGameRegime, gameModeKind, moment))
        {
            return null;
        }
        _logger.LogInformation("Getting game regime for game mode {gameMode} at time = {t}", gameModeKind, moment);
        return scheduledGameRegime;
    }

    public async Task<List<GameMode>> GetAllGameModes(CancellationToken ct)
    {
        _logger.LogInformation("Getting all game regimes");
        List<GameMode> gameModes = await _gameRegimeRepository.GetAllGameModes(ct);
        if (!ValidateGetAllGameModes(gameModes))
        {
            return null!;
        }
        return gameModes;
    }

    public async Task<List<EnrichedGameRegimeDto>> GetUpcomingGameRegimesByGameMode(string gameModeKind, DateTimeOffset moment, CancellationToken ct)
    {
        _logger.LogInformation("Getting upcoming game regimes for gameMode {gameMode}", gameModeKind);
        GameMode? gameMode = await _gameRegimeRepository.GetGameModeById(gameModeKind, ct);
        if (gameMode == null)
        {
            _logger.LogWarning("Cannot find game mode {0}", gameModeKind);
            return [];
        }
        return await _gameRegimeRepository.GetUpcomingGameRegimesByMode(gameModeKind, moment,
            LobbyConstants.GameModeFillIntervalInMinutes, LobbyConstants.GameModeFillTimeFrameInMinutes, ct);
    }

    public async Task<Dictionary<string, List<ScheduledGameRegime>>> getAllUpcomingGameRegimes(DateTimeOffset moment, int intervalMinutes, int timeFrameMinutes, CancellationToken ct)
    {
        _logger.LogInformation("Getting Scheduled Game Regimes for nearest future at time = {t}", moment);
        List<ScheduledGameRegime> rawSgr = await _gameRegimeRepository.GetAllUpcomingGameRegimes(moment, intervalMinutes, timeFrameMinutes, ct);
        Dictionary<string, List<ScheduledGameRegime>> result = rawSgr.GroupBy(sgr => sgr.GameMode, sgr => sgr)
            .ToDictionary(keying => keying.Key, valuing => valuing.OrderBy(sgr => sgr.IntervalStart).ToList());
        return result;
    }

    public async Task<Dictionary<string, Dictionary<string, List<GameRegimeEntry>>>> GetGameRegimeEntriesByModeByRegime(CancellationToken ct)
    {
        List<GameRegimeEntry> rawEntries = await _gameRegimeRepository.GetAllGameRegimeEntries(ct);
        return rawEntries
            .GroupBy(entry => entry.GameMode, entry => entry)
            .ToDictionary(keying => keying.Key, valuing => valuing.ToList()
                .GroupBy(entry => entry.GameRegime, entry => entry)
                .ToDictionary(keying => keying.Key, valuing => valuing.ToList())
            );
    }

    public async Task CreateScheduledGameRegimes(List<ScheduledGameRegime> newSgrs, CancellationToken ct)
    {
        await _gameRegimeRepository.CreateScheduledGameRegimes(newSgrs, ct);
    }

    public async Task<GameMode?> GetGameModeById(string gameMode, CancellationToken ct)
    {
        return await _gameRegimeRepository.GetGameModeById(gameMode, ct);
    }

    #endregion

    #region Validation

    private bool ValidateGetAllGameModes(List<GameMode> allGameModes)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (allGameModes.Count == 0)
        {
            _validationStorage.AddError(ErrorCode.GameModeNotFound, $"Game modes not found at {now}!");
        }
        return _validationStorage.IsValid;
    }

    private bool ValidateGetCurrentGameRegime(EnrichedGameRegimeDto? scheduledGameRegime, string gameModeKind, DateTimeOffset moment)
    {
        if (scheduledGameRegime == null)
        {
            string errorText = string.Format("Cannot find current game regime for game mode {0} at {1}!", gameModeKind, moment);
            _validationStorage.AddError(ErrorCode.GameModeNotFound, errorText);
            _logger.LogError(errorText);
            return false;
        }
        return _validationStorage.IsValid;
    }

    #endregion
}
