using Lobby.Repository;
using Lobby.Models.db;
using Lobby.Repository.Interface;
using Lobby.Util;
using Lobby.Validation;

namespace Lobby.Service;

public class UserService(ILogger<GameService> logger, IUserRepository userRepository, IValidationStorage validationStorage)
{
    private readonly ILogger<GameService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IValidationStorage _validationStorage = validationStorage;

    #region Actions
    
    public async Task<User?> GetById(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Getting user by id {0}", userId);
        return await _userRepository.GetById(userId, ct);
    }

    public async Task<List<User>> GetByIds(IEnumerable<Guid> userIds, CancellationToken ct)
    {
        _logger.LogInformation("Getting users by ids {0}", userIds);
        return await _userRepository.GetByIds(userIds, ct);
    }

    public async Task CreateIfAbsent(Guid userId, CancellationToken ct)
    {
        User? user = await GetById(userId, ct);
        if (user == null)
        {
            _logger.LogInformation("Cannot find existing user with userId {userId}. Creating a new user.", userId);
            await _userRepository.Create(userId, ct);
        }
    }
    
    #endregion
    
    #region Validation

    public async Task<bool> ValidateUser(Guid userId, CancellationToken ct)
    {
        if (!await _userRepository.UserExists(userId, ct))
        {
            ValidationUtils.AddUnknownUserError(_validationStorage, userId);
        }
        return _validationStorage.IsValid;
    }
    
    #endregion
}