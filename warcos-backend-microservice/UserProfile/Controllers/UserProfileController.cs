using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Filters;
using UserProfile.Models.UserProfile;
using UserProfile.Service;
using UserProfile.Validation;
using UserProfile.Validation.Attributes;

namespace UserProfile.Controllers;

[ApiController]
[Route("api/public")]
public class UserProfileController : BaseController
{
    private readonly UserProfileService _userProfileService;
    
    public UserProfileController(IValidationStorage validationStorage, UserProfileService userProfileService) : base(validationStorage)
    {
        _userProfileService = userProfileService;
    }
    
    [Authorize]
    [HttpGet("getUserStatistics")]
    public async Task<IActionResult> GetUserStatistics([FromQuery, ValidGuid] Guid userId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _userProfileService.GetUserProfile(userId, token), ct);
    }
    
    // createUserStatistics будет вызываться из LOBBY Service
    [ServerAuthorized]
    [HttpPost("createUserStatistics")]
    public async Task<IActionResult> CreateUserStatistics([FromBody] UserProfileRequest userProfileRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _userProfileService.CreateUserStatistics(userProfileRequest, token), ct);
    }
    
    [ServerAuthorized]
    [HttpPut("updateUserStatistics")]
    public async Task<IActionResult> UpdateUserStatistics([FromBody] UserStatisticsRequest request, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _userProfileService.UpdateUserStatistics(request, token), ct);
    }
}