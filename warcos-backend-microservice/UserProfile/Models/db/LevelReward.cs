namespace UserProfile.Models.db;

public class LevelReward
{
    public Guid RewardId { get; set; } = Guid.NewGuid();
    public int LevelId { get; set; }
    public virtual Level Level { get; set; }
    
    public RewardType RewardType { get; set; }
    public string RewardName { get; set; }
    
    public Guid ItemId { get; set; }
    public virtual Item Item { get; set; }
    
    public ICollection<UserLevelReward> UserLevelRewards { get; set; }
}