using Lobby.Models.db;

namespace Lobby.Repository.Interface;

public interface IUserRepository
{
    Task<User?> GetById(Guid userId, CancellationToken ct);
    Task<List<User>> GetByIds(IEnumerable<Guid> userIds, CancellationToken ct);
    Task Create(Guid userId, CancellationToken ct);
    Task<bool> UserExists(Guid userId, CancellationToken ct);
}
