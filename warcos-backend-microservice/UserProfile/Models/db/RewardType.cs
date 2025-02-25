using System.ComponentModel.DataAnnotations;

namespace UserProfile.Models.db;

public enum RewardType
{
    [Display(Name = "Skin")]
    Skin = 0,
    
    [Display(Name = "Weapon")]
    Weapon = 1,
    
    [Display(Name = "Item")]
    Item = 2
}