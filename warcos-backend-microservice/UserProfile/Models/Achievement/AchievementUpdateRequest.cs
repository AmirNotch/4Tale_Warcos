using Common.Validation.Attributes;

namespace UserProfile.Models.Achievement;

public class AchievementUpdateRequest
{
    [ValidGuid]
    public Guid UserId { get; set; }
}