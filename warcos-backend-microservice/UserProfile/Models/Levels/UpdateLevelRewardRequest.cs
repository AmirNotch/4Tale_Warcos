using Common.Validation.Attributes;

namespace UserProfile.Models.Levels;

public class UpdateLevelRewardRequest
{
    [ValidGuid]
    public Guid UserId { get; set; }
    
    [ValidNotNegativeInteger]
    public int LevelId { get; set; }
}