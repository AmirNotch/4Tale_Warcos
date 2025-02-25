namespace UserProfile.Models.db;

public class Achievement
{
    public Guid AchievementId { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    
    public RequirementType RequirementType { get; set; }
    public int RequirementValue { get; set; }
    public RewardType RewardType { get; set; }
    
    // Ссылка на предмет, связанный с достижением
    public Guid RewardItemId { get; set; }
    public virtual Item RewardItem { get; set; }
    
    // Пользователи, получившие это достижение
    public ICollection<UserAchievement> UserAchievements { get; set; }
}