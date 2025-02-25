namespace UserProfile.Models.db;

public class UserProfile
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    
    // Связь с уровнем
    public int LevelId { get; set; }
    public virtual Level Level { get; set; }
    public int ExperiencePoints { get; set; } = 0;
    
    // Статистика игрока
    public int Kills { get; set; } = 0;
    public int Deaths { get; set; } = 0;
    public int Assists { get; set; } = 0;
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public int MatchesPlayed { get; set; } = 0;
    public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
    public int HeadshotKills { get; set; } = 0;
    public int MeleeKills { get; set; } = 0;
    public int KillStreak { get; set; } = 0;
    
    // Список достижений
    public ICollection<UserAchievement> UserAchievements { get; set; }
    public ICollection<UserLevelReward> UserLevelRewards { get; set; }
    public ICollection<UserProfileStatsHistory> UserProfileStatsHistories { get; set; }
}