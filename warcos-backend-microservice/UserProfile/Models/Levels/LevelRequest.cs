using Common.Validation.Attributes;

namespace UserProfile.Models.Levels;

public class LevelRequest
{
    [ValidNotNegativeInteger]
    public int LevelId { get; set; }
    
    [ValidNotNegativeInteger]
    public int ExperiencePoints { get; set; }
}