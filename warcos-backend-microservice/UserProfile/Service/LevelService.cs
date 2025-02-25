using AutoMapper;
using UserProfile.Models.db;
using UserProfile.Models.Levels;
using UserProfile.Repository.Interface;
using UserProfile.Validation;

namespace UserProfile.Service;

public class LevelService
{
    private readonly ILogger<LevelService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly ILevelRepository _levelRepository;
    private readonly IMapper _mapper;

    public LevelService(ILogger<LevelService> logger, IValidationStorage validationStorage, 
        ILevelRepository levelRepository, IMapper mapper)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _levelRepository = levelRepository;
        _mapper = mapper;
    }
    
    #region Action

    public async Task<IEnumerable<LevelDTO>> GetLevels(CancellationToken ct)
    {
        IEnumerable<Level> levels = await _levelRepository.GetAllLevels(ct);
        var levelsDtos = _mapper.Map<List<LevelDTO>>(levels);
        return levelsDtos;
    }
    public async Task<LevelDTO?> CreateLevel(LevelRequest levelRequest, CancellationToken ct)
    {
        bool isValid = await ValidateCreateLevel(levelRequest, ct);
        if (!isValid)
        {
            return null;
        }
        
        var level = new Level()
        {
            LevelId = levelRequest.LevelId,
            ExperiencePoints = levelRequest.ExperiencePoints,
        };
        await _levelRepository.CreateLevel(level, ct);
        return new LevelDTO { LevelId = levelRequest.LevelId, ExperiencePoints = levelRequest.ExperiencePoints };
    }
    
    public async Task<bool> UpdateLevel(LevelRequest levelRequest, CancellationToken ct)
    {
        bool isValid = await ValidateUpdateLevel(levelRequest, ct);
        if (!isValid)
        {
            return false;
        }
        
        await _levelRepository.UpdateLevel(levelRequest, ct);
        return true;
    }
    
    public async Task<bool> DeleteLevel(int levelId, CancellationToken ct)
    {
        bool isValid = await ValidateDeleteLevel(levelId, ct);
        if (!isValid)
        {
            return false;
        }
        
        await _levelRepository.DeleteLevel(levelId, ct);
        return true;
    }

    public async Task<bool> GrantLevelReward(UpdateLevelRewardRequest request, CancellationToken ct)
    {
        // Получаем все награды для указанного уровня
        var levelRewards = await _levelRepository.GetLevelRewardsByLevelId(request.LevelId, ct);

        // Получаем список RewardId наград, которые пользователь уже получил
        var receivedRewardIds = await _levelRepository.GetUserReceivedRewardIds(request.UserId, request.LevelId, ct);

        // Фильтруем награды, чтобы оставить только те, которые пользователь еще не получил
        var newRewards = levelRewards.Where(reward => !receivedRewardIds.Contains(reward.RewardId));

        // Добавляем новые награды для пользователя
        foreach (var reward in newRewards)
        {
            var userLevelReward = new UserLevelReward
            {
                UserId = request.UserId,
                RewardId = reward.RewardId,
                ReceivedAt = DateTimeOffset.UtcNow
            };

            _levelRepository.AddUserLevelRewardNoSave(userLevelReward);
            
            // TODO: Если награда представляет собой айтем, добавить его в инвентарь пользователя
            // Это можно сделать, когда будет реализован функционал инвентаря.
        }

        await _levelRepository.SaveChangesAsync(ct);
        return true;
    }

    #endregion

    #region Validation

    private async Task<bool> ValidateCreateLevel(LevelRequest levelRequest, CancellationToken ct)
    {
        Level? levelById = await _levelRepository.GetLevelById(levelRequest.LevelId, ct);
        if (levelById != null)
        {
            _validationStorage.AddError(ErrorCode.LevelAlreadyExists, $"Level with id {levelRequest.LevelId} already exists");
        }
        
        Level? level = await _levelRepository.GetGreatestLevel(ct);
        if (level != null && level.ExperiencePoints >= levelRequest.ExperiencePoints)
        {
            _validationStorage.AddError(ErrorCode.UnknownLevel, $"Level id {levelRequest.LevelId} must be greater than {level.ExperiencePoints}");
            return false;
        }
        return _validationStorage.IsValid;
    }
    
    private async Task<bool> ValidateUpdateLevel(LevelRequest levelRequest, CancellationToken ct)
    {
        Level? levelById = await _levelRepository.GetLevelById(levelRequest.LevelId, ct);
        if (levelById == null)
        {
            _validationStorage.AddError(ErrorCode.UnknownLevel, $"Level with id {levelRequest.LevelId} does not exist");
        }
        return _validationStorage.IsValid;
    }

    private async Task<bool> ValidateDeleteLevel(int levelId, CancellationToken ct)
    {
        Level? level = await _levelRepository.GetLevelById(levelId, ct);
        if (level == null)
        {
            _validationStorage.AddError(ErrorCode.UnknownLevel, $"Level with id {levelId} does not exist");
            return false;
        }
        bool hasOwned = await _levelRepository.IsLevelOwned(levelId, ct);
        if (hasOwned)
        {
            _validationStorage.AddError(ErrorCode.CannotDeleteOwnedLevel, $"Level with id {levelId} has been owned and cannot be deleted");
        }
        return _validationStorage.IsValid;
    }
    
    #endregion
}