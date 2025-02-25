using UserProfile.Models.Achievement;
using UserProfile.Models.db;
using UserProfile.Models.UserProfile;
using UserProfile.Repository.Interface;
using UserProfile.Validation;

namespace UserProfile.Service;

public class AchievementService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly UserProfileService _userProfileService;

    public AchievementService(ILogger<UserProfileService> logger, IValidationStorage validationStorage, 
        IAchievementRepository achievementRepository, UserProfileService userProfileService,
        IUserProfileRepository userProfileRepository)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _achievementRepository = achievementRepository;
        _userProfileRepository = userProfileRepository;
        _userProfileService = userProfileService;
    }

    #region Action
    
    public async Task<IEnumerable<AchievementDTO>> GetAchievements(Guid userId, CancellationToken ct)
    {
        bool isValid = await _userProfileService.ValidateUser(userId, ct); // Прямой вызов ValidateUser
        if (!isValid)
        {
            return null!;
        }
        return await _achievementRepository.GetAchievementsByUserId(userId, ct);
    }
    
    public async Task<IEnumerable<AchievementDTO>> CheckAndUpdateUserAchievements(AchievementUpdateRequest request, CancellationToken ct)
    {
        bool isValid = await _userProfileService.ValidateUser(request.UserId, ct);
        if (!isValid)
        {
            return null!;
        }

        // Получаем список всех доступных достижений
        var allAchievements = await _achievementRepository.GetAllAchievements(ct);

        // Получаем список AchievementId достижений, которые пользователь уже выполнил
        var userAchievements = await _achievementRepository.GetUserAchievementIds(request.UserId, ct);

        var updatedAchievements = new List<AchievementDTO>();

        // Фильтруем достижения, чтобы оставить только те, которые пользователь еще не выполнил
        var pendingAchievements = allAchievements.Where(achievement => !userAchievements.Contains(achievement.AchievementId));

        // Проверяем условия для каждого достижения, которое пользователь еще не выполнил
        foreach (var achievement in pendingAchievements)
        {
            bool conditionsMet = await CheckAchievementConditions(request.UserId, achievement, ct);

            if (conditionsMet)
            {
                // Обновляем достижение для пользователя
                var newAchievement = new UserAchievement
                {
                    UserId = request.UserId,
                    AchievementId = achievement.AchievementId,
                    AchievedAt = DateTimeOffset.UtcNow
                };

                _achievementRepository.AddUserAchievementNoSave(newAchievement);

                updatedAchievements.Add(new AchievementDTO
                {
                    AchievementId = achievement.AchievementId,
                    Name = achievement.Name,
                    AchievedAt = newAchievement.AchievedAt
                });
            }
        }

        await _achievementRepository.SaveChangesAsync(ct);

        return updatedAchievements;
    }
    private async Task<bool> CheckAchievementConditions(Guid userId, Achievement achievement, CancellationToken ct)
    {
        Models.db.UserProfile? userStats = await _userProfileRepository.GetUserProfileByUserId(userId, ct);

        if (userStats == null) 
            return false;

        // Проверяем условия достижения, используя switch с инлайновой логикой
        return achievement.RequirementType switch
        {
            RequirementType.Kills => userStats.Kills >= achievement.RequirementValue,
            RequirementType.MatchesPlayed => userStats.MatchesPlayed >= achievement.RequirementValue,
            RequirementType.KillStreak => userStats.KillStreak >= achievement.RequirementValue,
            RequirementType.HeadshotKills => userStats.HeadshotKills >= achievement.RequirementValue,
            RequirementType.Wins => userStats.Wins >= achievement.RequirementValue,
            _ => false // Возвращаем false, если нет подходящего типа
        };
    }
    
    #endregion
    
    #region Validation
    
    #endregion
}