namespace UserProfile.Models.db;

public class Level
{
    public int LevelId { get; set; }
    public int ExperiencePoints { get; set; } = 0;
    
    // Связанные награды для уровня
    public virtual LevelReward Reward { get; set; }
    
    // Игроки на уровне
    public ICollection<UserProfile> UserProfiles { get; set; }
    public ICollection<UserProfileStatsHistory> UserProfileStatsHistories { get; set; }
    public ICollection<LevelReward> LevelRewards { get; set; }
}