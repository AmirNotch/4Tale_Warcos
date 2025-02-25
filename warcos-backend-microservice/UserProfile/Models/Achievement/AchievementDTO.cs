namespace UserProfile.Models.Achievement;

public class AchievementDTO
{
    public Guid AchievementId { get; set; }
    public string Name { get; set; }
    public DateTimeOffset AchievedAt { get; set; } = DateTimeOffset.UtcNow;
}