using System.ComponentModel.DataAnnotations;

namespace UserProfile.Models.db;

public enum RequirementType
{
    [Display(Name = "HeadshotKills")]
    HeadshotKills = 0,
    
    [Display(Name = "MeleeKills")]
    MeleeKills = 1,
    
    [Display(Name = "Wins")]
    Wins = 2,
    
    [Display(Name = "Kills")]
    Kills = 3,
    
    [Display(Name = "KillStreak")]
    KillStreak = 4,
    
    [Display(Name = "PlayTime")]
    PlayTime = 5,
    
    [Display(Name = "MatchesPlayed")]
    MatchesPlayed = 6
}