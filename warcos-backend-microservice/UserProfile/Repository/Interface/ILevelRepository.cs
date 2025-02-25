using UserProfile.Models.db;
using UserProfile.Models.Levels;

namespace UserProfile.Repository.Interface;

public interface ILevelRepository
{
    Task<List<Level>> GetAllLevels(CancellationToken ct);
    Task CreateLevel(Level level, CancellationToken ct);
    Task UpdateLevel(LevelRequest levelRequest, CancellationToken ct);
    Task DeleteLevel(int levelId, CancellationToken ct);
    Task<Level?> GetLevelById(int levelId, CancellationToken ct);
    Task<bool> IsLevelOwned(int levelId, CancellationToken ct);
    Task<Level?> GetGreatestLevel(CancellationToken ct);
    Task<IEnumerable<LevelReward>> GetLevelRewardsByLevelId(int levelId, CancellationToken ct);
    Task<bool> HasUserReceivedReward(Guid userId, Guid rewardId, CancellationToken ct);
    void AddUserLevelRewardNoSave(UserLevelReward userLevelReward);
    Task SaveChangesAsync(CancellationToken ct);
    Task<IEnumerable<Guid>> GetUserReceivedRewardIds(Guid userId, int levelId, CancellationToken ct);
}