using UserProfile.Models.db;
using UserProfile.Models.Items;

namespace UserProfile.Repository.Interface;

public interface IItemRepository
{
    Task<List<Item>> GetAllItems(CancellationToken ct);
    Task CreateItem(Item item, CancellationToken ct);
    Task UpdateItem(ItemUpdateRequest updateItemRequest, CancellationToken ct);
    Task DeleteItem(Guid itemId, CancellationToken ct);
    Task<Item?> GetItemByUnrealId(string unrealId, CancellationToken ct);
    Task<Item?> GetItemById(Guid itemId, CancellationToken ct);
    Task<bool> IsItemOwned(Guid itemId, CancellationToken ct);
}