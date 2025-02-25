using Common.Validation.Attributes;
using Microsoft.AspNetCore.Mvc;
using UserProfile.Filters;
using UserProfile.Models.Items;
using UserProfile.Service;
using UserProfile.Validation;

namespace UserProfile.Controllers;

[ApiController]
[Route("api/private")]
public class ItemController : BaseController
{
    private readonly ItemService _itemService;
    
    public ItemController(IValidationStorage validationStorage, ItemService itemService) : base(validationStorage)
    {
        _itemService = itemService;
    }
    
    [ServerAuthorized]
    [HttpGet("getItems")]
    public async Task<IActionResult> GetUserStatistics(CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _itemService.GetItems(token), ct);
    }
    
    [ServerAuthorized]
    [HttpPost("createItem")]
    public async Task<IActionResult> CreateItem(ItemRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _itemService.CreateItem(request, token), ct);
    }
    
    [ServerAuthorized]
    [HttpPut("updateItem")]
    public async Task<IActionResult> UpdateItem(ItemUpdateRequest request ,CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _itemService.UpdateItem(request, token), ct);
    }
    
    [ServerAuthorized]
    [HttpDelete("deleteItem")]
    public async Task<IActionResult> DeleteItem([FromQuery, ValidGuid] Guid itemId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _itemService.DeleteItem(itemId, token), ct);
    }
}