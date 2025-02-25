using Microsoft.EntityFrameworkCore;
using UserProfile.Models;
using UserProfile.Models.Achievement;
using UserProfile.Models.db;
using UserProfile.Repository.Interface;

namespace UserProfile.Repository;

public class AchievementRepository : IAchievementRepository
{
    private readonly ILogger<UserProfileRepository> _logger;
    private readonly WarcosUserProfileDbContext _dbContext;

    public AchievementRepository(ILogger<UserProfileRepository> logger, WarcosUserProfileDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<List<AchievementDTO>> GetAchievementsByUserId(Guid userId, CancellationToken ct)
    {
        var achievements = await (from userAchievement in _dbContext.UserAchievements
            where userAchievement.UserId == userId
            join achievement in _dbContext.Achievements
                on userAchievement.AchievementId equals achievement.AchievementId
            select new AchievementDTO
            {
                AchievementId = achievement.AchievementId,
                Name = achievement.Name,
                AchievedAt = userAchievement.AchievedAt
            }).ToListAsync(ct);

        return achievements;
    }
    
    public async Task<IEnumerable<Achievement>> GetAllAchievements(CancellationToken ct)
    {
        return await _dbContext.Achievements.ToListAsync(ct);
    }
    
    public async Task<bool> HasUserAchieved(Guid userId, Guid achievementId, CancellationToken ct)
    {
        return await _dbContext.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId, ct);
    }

    // Добавление нового достижения для пользователя
    public void AddUserAchievementNoSave(UserAchievement userAchievement)
    {
         _dbContext.UserAchievements.Add(userAchievement);
    }
    
    public async Task<IEnumerable<Guid>> GetUserAchievementIds(Guid userId, CancellationToken ct)
    {
        return await _dbContext.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync(ct);
    }

    // Сохранение изменений
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}