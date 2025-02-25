using AutoMapper;
using UserProfile.Models.db;
using UserProfile.Models.Items;
using UserProfile.Repository.Interface;
using UserProfile.Validation;

namespace UserProfile.Service;

public class ItemService
{
    private readonly ILogger<ItemService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly IItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public ItemService(ILogger<ItemService> logger, IValidationStorage validationStorage, 
        IItemRepository itemRepository, IMapper mapper)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _itemRepository = itemRepository;
        _mapper = mapper;
    }

    #region Action

    public async Task<IEnumerable<ItemDTO>> GetItems(CancellationToken ct)
    {
        IEnumerable<Item> items = await _itemRepository.GetAllItems(ct);
        var itemDtos = _mapper.Map<List<ItemDTO>>(items);
        return itemDtos;
    }
    public async Task<ItemDTO?> CreateItem(ItemRequest itemRequest, CancellationToken ct)
    {
        bool isValid = await CreateItemValidation(itemRequest.UnrealId, ct);
        if (!isValid)
        {
            return null;
        }
        
        var item = new Item()
        {
            ItemId = new Guid(),
            UnrealId = itemRequest.UnrealId,
            CreatedAt = DateTime.UtcNow
        };
        await _itemRepository.CreateItem(item, ct);
        return new ItemDTO { ItemId = item.ItemId, UnrealId = itemRequest.UnrealId, CreatedAt = item.CreatedAt };
    }
    
    public async Task<bool> UpdateItem(ItemUpdateRequest updateItemRequest, CancellationToken ct)
    {
        bool isValid = await UpdateItemValidation(updateItemRequest, ct);
        if (!isValid)
        {
            return false;
        }
        
        await _itemRepository.UpdateItem(updateItemRequest, ct);
        return true;
    }
    
    public async Task<bool> DeleteItem(Guid itemId, CancellationToken ct)
    {
        bool isValid = await DeleteItemValidate(itemId, ct);
        if (!isValid)
        {
            return false;
        }
        
        await _itemRepository.DeleteItem(itemId, ct);
        return true;
    }

    #endregion

    #region Validation

    private async Task<bool> CreateItemValidation(string unrealId, CancellationToken ct)
    {
        Item? item = await _itemRepository.GetItemByUnrealId(unrealId, ct);
        if (item != null)
        {
            _validationStorage.AddError(ErrorCode.ItemAlreadyExists, $"Item with id {unrealId} already exists");
        }
        return _validationStorage.IsValid;
    }
    
    private async Task<bool> UpdateItemValidation(ItemUpdateRequest updateItemRequest, CancellationToken ct)
    {
        Item? itemById = await _itemRepository.GetItemById(updateItemRequest.ItemId, ct);
        if (itemById == null)
        {
            _validationStorage.AddError(ErrorCode.UnknownItem, $"Item with id {updateItemRequest.ItemId} does not exist");
        }
        Item? item = await _itemRepository.GetItemByUnrealId(updateItemRequest.UnrealId, ct);
        if (item != null && item.ItemId != updateItemRequest.ItemId)
        {
            _validationStorage.AddError(ErrorCode.ItemAlreadyExists, $"Item with id {updateItemRequest.UnrealId} already exists");
        }
        return _validationStorage.IsValid;
    }

    private async Task<bool> DeleteItemValidate(Guid itemId, CancellationToken ct)
    {
        Item? item = await _itemRepository.GetItemById(itemId, ct);
        if (item == null)
        {
            _validationStorage.AddError(ErrorCode.UnknownItem, $"Item with id {itemId} does not exist");
            return false;
        }
        bool hasOwned = await _itemRepository.IsItemOwned(itemId, ct);
        if (hasOwned)
        {
            _validationStorage.AddError(ErrorCode.CannotDeleteOwnedItem, $"Item with id {itemId} has been owned and cannot be deleted");
        }
        return _validationStorage.IsValid;
    }
    
    #endregion
}