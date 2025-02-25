namespace UserProfile.Models.db;

public class UserProfileStatsHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public virtual UserProfile UserProfile { get; set; }

    public int LevelId { get; set; }
    public virtual Level Level { get; set; }
    
    public int ExperiencePoints { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int MatchesPlayed { get; set; }
    public TimeSpan PlayTime { get; set; }
    public int HeadshotKills { get; set; }
    public int MeleeKills { get; set; }
    public int KillStreak { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
