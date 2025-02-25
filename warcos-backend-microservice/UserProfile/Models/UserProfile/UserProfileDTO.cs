using Common.Validation.Attributes;

namespace UserProfile.Models.UserProfile;

public class UserProfileDTO
{
    [ValidGuid]
    public Guid UserId { get; set; } = Guid.NewGuid();
    
    [ValidNotNegativeInteger]
    public int LevelId { get; set; }
    
    [ValidNotNegativeInteger]
    public int ExperiencePoints { get; set; } = 0;

    [ValidNotNegativeInteger]
    public int Kills { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int Deaths { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int Assists { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int Wins { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int Losses { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int MatchesPlayed { get; set; } = 0;
    public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
    
    [ValidNotNegativeInteger]
    public int HeadshotKills { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int MeleeKills { get; set; } = 0;
    
    [ValidNotNegativeInteger]
    public int KillStreak { get; set; } = 0;
}