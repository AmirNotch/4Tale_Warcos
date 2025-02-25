using Common.Validation.Attributes;

namespace UserProfile.Models.UserProfile;

public class UserStatisticsUpdate
{
    [ValidGuid]
    public Guid UserId { get; set; }
    public UserProfileDTO UserProfile { get; set; } = null!;
}