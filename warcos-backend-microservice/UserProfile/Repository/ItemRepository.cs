using Microsoft.EntityFrameworkCore;
using UserProfile.Models;
using UserProfile.Models.db;
using UserProfile.Models.Items;
using UserProfile.Repository.Interface;

namespace UserProfile.Repository;

public class ItemRepository : IItemRepository
{
    private readonly ILogger<ItemRepository> _logger;
    private readonly WarcosUserProfileDbContext _dbContext;

    public ItemRepository(ILogger<ItemRepository> logger, WarcosUserProfileDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<Item>> GetAllItems(CancellationToken ct)
    {
        return await _dbContext.Items.OrderBy(x => x.CreatedAt).ToListAsync(ct);
    }
    
    public async Task CreateItem(Item item, CancellationToken ct)
    {
        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateItem(ItemUpdateRequest updateItemRequest, CancellationToken ct)
    {
        await _dbContext.Items
            .Where(i => i.ItemId == updateItemRequest.ItemId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(i => i.UnrealId, updateItemRequest.UnrealId),
                ct);
    }

    public async Task DeleteItem(Guid itemId, CancellationToken ct)
    {
        await _dbContext.Items
            .Where(i => i.ItemId == itemId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<Item?> GetItemByUnrealId(string unrealId, CancellationToken ct)
    {
        return await _dbContext.Items
            .Where(e => e.UnrealId == unrealId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Item?> GetItemById(Guid itemId, CancellationToken ct)
    {
        return await _dbContext.Items
            .Where(e => e.ItemId == itemId)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task<bool> IsItemOwned(Guid itemId, CancellationToken ct)
    {
        return await _dbContext.Achievements
            .Where(e => e.RewardItemId == itemId)
            .AnyAsync(ct);
    }
}