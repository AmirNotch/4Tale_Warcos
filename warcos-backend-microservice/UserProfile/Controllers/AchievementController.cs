using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Filters;
using UserProfile.Models.Achievement;
using UserProfile.Models.Levels;
using UserProfile.Service;
using UserProfile.Validation;
using UserProfile.Validation.Attributes;

namespace UserProfile.Controllers;

[ApiController]
[Route("api/public")]
public class AchievementController : BaseController
{
    private readonly AchievementService _achievementService;

    public AchievementController(IValidationStorage validationStorage, AchievementService achievementService) 
        : base(validationStorage)
    {
        _achievementService = achievementService;
    }
    
    [Authorize]
    [HttpGet("getUserAchievements")]
    public async Task<IActionResult> GetUserAchievements([FromQuery, ValidGuid] Guid userId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _achievementService.GetAchievements(userId, token), ct);
    }
    
    [ServerAuthorized]
    [HttpPost("updateUserAchievements")]
    public async Task<IActionResult> CheckAndUpdateUserAchievements(AchievementUpdateRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _achievementService.CheckAndUpdateUserAchievements(request, token), ct);
    }
}