using UserProfile.Models.Achievement;
using UserProfile.Models.db;

namespace UserProfile.Repository.Interface;

public interface IAchievementRepository
{
    Task<List<AchievementDTO>> GetAchievementsByUserId(Guid userId, CancellationToken ct);
    Task<bool> HasUserAchieved(Guid userId, Guid achievementId, CancellationToken ct);
    void AddUserAchievementNoSave(UserAchievement userAchievement);
    Task<IEnumerable<Achievement>> GetAllAchievements(CancellationToken ct);
    Task<IEnumerable<Guid>> GetUserAchievementIds(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}