using UserProfile.Models.db;
using UserProfile.Models.UserProfile;

namespace UserProfile.Repository.Interface;

public interface IUserProfileRepository
{
    Task<Models.db.UserProfile?> GetUserProfileByUserId(Guid userId, CancellationToken ct);
    Task<Level?> GetLowestLevel(CancellationToken ct);
    Task CreateUserProfile(Models.db.UserProfile userProfile, CancellationToken ct);
    Task UpdateUserProfiles(List<UserStatisticsUpdate> userStatistics, CancellationToken ct);
    Task UpdateUserProfilesStatsHistory(List<UserStatisticsUpdate> userStatisticsList, CancellationToken ct);
    Task<bool> UserExists(Guid userId, CancellationToken ct);
    Task<bool> UserUserAlreadyExists(Guid userId, CancellationToken ct);
    Task<Models.db.UserProfile?> GetById(Guid userId, CancellationToken ct);
}