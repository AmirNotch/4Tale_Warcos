using Common.Validation.Attributes;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Filters;
using UserProfile.Models.Levels;
using UserProfile.Service;
using UserProfile.Validation;

namespace UserProfile.Controllers;

[ApiController]
[Route("api/private")]
public class LevelController : BaseController
{
    private readonly LevelService _levelService;
    
    public LevelController(IValidationStorage validationStorage, LevelService levelService) : base(validationStorage)
    {
        _levelService = levelService;
    }
    
    [ServerAuthorized]
    [HttpGet("getLevels")]
    public async Task<IActionResult> GetLevels(CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _levelService.GetLevels(token), ct);
    }
    
    [ServerAuthorized]
    [HttpPost("createLevel")]
    public async Task<IActionResult> CreateLevel(LevelRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _levelService.CreateLevel(request, token), ct);
    }
    
    [ServerAuthorized]
    [HttpPut("updateLevel")]
    public async Task<IActionResult> UpdateLevel(LevelRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _levelService.UpdateLevel(request, token), ct);
    }
    
    [ServerAuthorized]
    [HttpDelete("deleteLevel")]
    public async Task<IActionResult> DeleteLevel([FromQuery, ValidNotNegativeInteger] int levelId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _levelService.DeleteLevel(levelId, token), ct);
    }
    
    
    [ServerAuthorized]
    [HttpPost("updateLevelReward")]
    public async Task<IActionResult> UpdateLevelReward(UpdateLevelRewardRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _levelService.GrantLevelReward(request, token), ct);
    }
}