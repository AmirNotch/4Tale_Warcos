using Lobby.Models;
using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;

using Lobby.Repository.Interface;

namespace Lobby.Repository;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly WarcosLobbyDbContext _dbContext;

    public UserRepository(ILogger<UserRepository> logger, WarcosLobbyDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<User?> GetById(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching user by id: {userId}");
        return await _dbContext.Users
            .Where(x => x.UserId.Equals(userId) && !x.Removed.HasValue)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<User>> GetByIds(IEnumerable<Guid> userIds, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching users by ids: {string.Join(", ", userIds)}");
        return await _dbContext.Users
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync(ct);
    }

    public async Task Create(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation($"Creating user with id: {userId}");
        _dbContext.Users.Add(new User { UserId = userId });
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UserExists(Guid userId, CancellationToken ct)
    {
        User? user = await GetById(userId, ct);
        return user != null;
    }
}
