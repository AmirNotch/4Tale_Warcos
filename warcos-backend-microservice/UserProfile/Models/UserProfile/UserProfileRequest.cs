using UserProfile.Validation.Attributes;

namespace UserProfile.Models.UserProfile;

public class UserProfileRequest
{
    [ValidGuid]
    public Guid UserId { get; set; }
}