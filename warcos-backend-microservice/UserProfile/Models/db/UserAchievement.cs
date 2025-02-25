namespace UserProfile.Models.db;

public class UserAchievement
{
    public Guid UserId { get; set; }
    public virtual UserProfile UserProfile { get; set; }
    
    public Guid AchievementId { get; set; }
    public virtual Achievement Achievement { get; set; }
    
    public DateTimeOffset AchievedAt { get; set; } = DateTimeOffset.UtcNow;
}