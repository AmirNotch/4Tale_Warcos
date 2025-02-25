using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using UserProfile.Models;
using UserProfile.Models.db;
using UserProfile.Models.UserProfile;
using UserProfile.Repository.Interface;

namespace UserProfile.Repository;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ILogger<UserProfileRepository> _logger;
    private readonly WarcosUserProfileDbContext _dbContext;

    public UserProfileRepository(ILogger<UserProfileRepository> logger, WarcosUserProfileDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Models.db.UserProfile?> GetUserProfileByUserId(Guid userId, CancellationToken ct)
    {
        return (await _dbContext.UserProfiles
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync(ct));
    }

    public async Task<Level?> GetLowestLevel(CancellationToken ct)
    {
        return await _dbContext.Levels
            .OrderBy(a => a.LevelId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateUserProfile(Models.db.UserProfile userProfile, CancellationToken ct)
    {
        _dbContext.UserProfiles.Add(userProfile);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateUserProfiles(List<UserStatisticsUpdate> userStatistics, CancellationToken ct)
    {
        // Get all UserIds from the incoming data
        var userIds = userStatistics.Select(ns => ns.UserId).Distinct().ToList();

        // Fetch the existing statistics from the database for all UserIds
        var userProfiles = await _dbContext.UserProfiles
            .Where(us => userIds.Contains(us.UserId))
            .ToListAsync(ct);

        // Update the statistics for each user
        foreach (var newStat in userStatistics)
        {
            // Find the corresponding existing statistics for the current UserId
            var userProfile = userProfiles.FirstOrDefault(es => es.UserId == newStat.UserId);

            if (userProfile != null)
            {
                // Find current level
                Level? currentLevel = await _dbContext.Levels
                    .Where(p => p.LevelId == userProfile.LevelId)
                    .FirstOrDefaultAsync(ct);

                // Update the total amount of experience points
                userProfile.ExperiencePoints += newStat.UserProfile.ExperiencePoints;

                // Check if the player has moved to a new level
                if (currentLevel == null || currentLevel.ExperiencePoints < userProfile.ExperiencePoints)
                {
                    // Find max level, corresponding to updated ExperiencePoints
                    Level? newLevel = await _dbContext.Levels
                        .Where(p => p.ExperiencePoints <= userProfile.ExperiencePoints)
                        .OrderByDescending(p => p.ExperiencePoints)
                        .FirstOrDefaultAsync(ct);

                    if (newLevel != null && newLevel.LevelId != userProfile.LevelId)
                    {
                        userProfile.LevelId = newLevel.LevelId; // Update user's level
                    }
                }
                
                // Update the existing statistics with new values
                userProfile.Kills += newStat.UserProfile.Kills;
                userProfile.Deaths += newStat.UserProfile.Deaths;
                userProfile.Assists += newStat.UserProfile.Assists;
                userProfile.Wins += newStat.UserProfile.Wins;
                userProfile.Losses += newStat.UserProfile.Losses;
                userProfile.MatchesPlayed += newStat.UserProfile.MatchesPlayed;
                userProfile.PlayTime += newStat.UserProfile.PlayTime; // This assumes PlayTime is a TimeSpan
                userProfile.HeadshotKills += newStat.UserProfile.HeadshotKills;
                userProfile.MeleeKills += newStat.UserProfile.MeleeKills;
                userProfile.KillStreak = Math.Max(userProfile.KillStreak, newStat.UserProfile.KillStreak); // Update according to your rules
            }
        }
        
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateUserProfilesStatsHistory(List<UserStatisticsUpdate> userStatisticsList, CancellationToken ct)
    {
        foreach (var userStat in userStatisticsList)
        {
            var statsHistory = new UserProfileStatsHistory
            {
                UserId = userStat.UserId,
                LevelId = userStat.UserProfile.LevelId,
                ExperiencePoints = userStat.UserProfile.ExperiencePoints,
                Kills = userStat.UserProfile.Kills,
                Deaths = userStat.UserProfile.Deaths,
                Assists = userStat.UserProfile.Assists,
                Wins = userStat.UserProfile.Wins,
                Losses = userStat.UserProfile.Losses,
                MatchesPlayed = userStat.UserProfile.MatchesPlayed,
                PlayTime = userStat.UserProfile.PlayTime,
                HeadshotKills = userStat.UserProfile.HeadshotKills,
                MeleeKills = userStat.UserProfile.MeleeKills,
                KillStreak = userStat.UserProfile.KillStreak,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _dbContext.UserProfileStatsHistories.AddAsync(statsHistory, ct);
        }
        
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UserExists(Guid userId, CancellationToken ct)
    {
        Models.db.UserProfile? user = await GetById(userId, ct);
        return user != null;
    }
    public async Task<bool> UserUserAlreadyExists(Guid userId, CancellationToken ct)
    {
        Models.db.UserProfile? user = await GetById(userId, ct);
        return user == null;
    }

    public async Task<Models.db.UserProfile?> GetById(Guid userId, CancellationToken ct)
    {
        return await _dbContext.UserProfiles
            .Where(x => x.UserId.Equals(userId))
            .SingleOrDefaultAsync(ct);
    }
}