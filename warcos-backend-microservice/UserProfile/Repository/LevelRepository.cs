using Microsoft.EntityFrameworkCore;
using UserProfile.Models;
using UserProfile.Models.db;
using UserProfile.Models.Levels;
using UserProfile.Repository.Interface;

namespace UserProfile.Repository;

public class LevelRepository : ILevelRepository
{
    private readonly ILogger<LevelRepository> _logger;
    private readonly WarcosUserProfileDbContext _dbContext;

    public LevelRepository(ILogger<LevelRepository> logger, WarcosUserProfileDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<Level>> GetAllLevels(CancellationToken ct)
    {
        return await _dbContext.Levels.OrderBy(x => x.LevelId).ToListAsync(ct);
    }

    public async Task CreateLevel(Level level, CancellationToken ct)
    {
        _dbContext.Levels.Add(level);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateLevel(LevelRequest levelRequest, CancellationToken ct)
    {
        await _dbContext.Levels
            .Where(l => l.LevelId == levelRequest.LevelId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(l => l.ExperiencePoints, levelRequest.ExperiencePoints),
                ct);
    }

    public async Task DeleteLevel(int levelId, CancellationToken ct)
    {
        await _dbContext.Levels
            .Where(l => l.LevelId == levelId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<Level?> GetLevelById(int levelId, CancellationToken ct)
    {
        return await _dbContext.Levels
            .Where(l => l.LevelId == levelId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsLevelOwned(int levelId, CancellationToken ct)
    {
        return await _dbContext.UserProfiles
            .Where(e => e.LevelId == levelId)
            .AnyAsync(ct);
    }

    public async Task<Level?> GetGreatestLevel(CancellationToken ct)
    {
        return await _dbContext.Levels
            .OrderByDescending(a => a.LevelId)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task<IEnumerable<LevelReward>> GetLevelRewardsByLevelId(int levelId, CancellationToken ct)
    {
        return await _dbContext.LevelRewards
            .Where(lr => lr.LevelId == levelId)
            .ToListAsync(ct);
    }
    
    public async Task<bool> HasUserReceivedReward(Guid userId, Guid rewardId, CancellationToken ct)
    {
        return await _dbContext.UserLevelRewards
            .AnyAsync(ulr => ulr.UserId == userId && ulr.RewardId == rewardId, ct);
    }

    public void AddUserLevelRewardNoSave(UserLevelReward userLevelReward)
    {
         _dbContext.UserLevelRewards.Add(userLevelReward);
    }
    
    public async Task<IEnumerable<Guid>> GetUserReceivedRewardIds(Guid userId, int levelId, CancellationToken ct)
    {
        return await _dbContext.UserLevelRewards
            .Where(ulr => ulr.UserId == userId && ulr.LevelReward.LevelId == levelId)
            .Select(ulr => ulr.RewardId)
            .ToListAsync(ct);
    }
    
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }   
}