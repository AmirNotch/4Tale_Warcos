using UserProfile.Models.db;
using UserProfile.Models.UserProfile;
using UserProfile.Repository.Interface;
using UserProfile.Util;
using UserProfile.Validation;

namespace UserProfile.Service;

public class UserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly IUserProfileRepository _userProfileRepository;
    
    public UserProfileService(ILogger<UserProfileService> logger, IValidationStorage validationStorage,
        IUserProfileRepository userProfileRepository)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _userProfileRepository = userProfileRepository;
    }

    #region Action

    public async Task<UserProfileDTO?> GetUserProfile(Guid userId, CancellationToken ct)
    {
        bool isRequestValid = await GetUserProfileValidation(userId, ct);
        if (!isRequestValid)
        {
            return null;
        }
        Models.db.UserProfile? userProfile = await _userProfileRepository.GetUserProfileByUserId(userId, ct);
        if (userProfile == null)
        {
            return null;
        }
        return new UserProfileDTO {
            UserId = userProfile.UserId,
            LevelId = userProfile.LevelId,
            Kills = userProfile.Kills,
            Deaths = userProfile.Deaths,
            Assists = userProfile.Assists,
            Wins = userProfile.Wins,
            Losses = userProfile.Losses,
            MatchesPlayed = userProfile.MatchesPlayed,
            PlayTime = userProfile.PlayTime,
            HeadshotKills = userProfile.HeadshotKills,
            MeleeKills = userProfile.MeleeKills,
            KillStreak = userProfile.KillStreak,
        };
    }

    public async Task<bool> CreateUserStatistics(UserProfileRequest userProfileRequest, CancellationToken ct)
    {
        bool isRequestValid = await CreateUserStatisticsValidation(userProfileRequest.UserId, ct);
        if (!isRequestValid)
        {
            return false;
        }
        Level level = await _userProfileRepository.GetLowestLevel(ct);
        var userProfile = new Models.db.UserProfile()
        {
            UserId = userProfileRequest.UserId,
            LevelId = level!.LevelId,
            ExperiencePoints = level.ExperiencePoints
        };

        await _userProfileRepository.CreateUserProfile(userProfile, ct);
        return true;
    }

    public async Task<bool> UpdateUserStatistics(UserStatisticsRequest request, CancellationToken ct)
    {
        bool isRequestValid = await UpdateUserStatisticsValidation(request, ct);
        if (!isRequestValid)
        {
            return false;
        }
        await _userProfileRepository.UpdateUserProfilesStatsHistory(request.UserStatistics, ct);
        await _userProfileRepository.UpdateUserProfiles(request.UserStatistics, ct);
        return true;
    }

    #endregion

    #region Validation
    
    public async Task<bool> ValidateUser(Guid userId, CancellationToken ct)
    {
        if (!await _userProfileRepository.UserExists(userId, ct))
        {
            ValidationUtils.AddUnknownUserError(_validationStorage, userId);
        }
        return _validationStorage.IsValid;
    }
    
    public async Task<bool> ValidateUserAlreadyExists(Guid userId, CancellationToken ct)
    {
        if (!await _userProfileRepository.UserUserAlreadyExists(userId, ct))
        {
            ValidationUtils.AddUserAlreadyExistsError(_validationStorage, userId);
        }
        return _validationStorage.IsValid;
    }

    private async Task<bool> UpdateUserStatisticsValidation(UserStatisticsRequest request, CancellationToken ct)
    {
        foreach (var userStat in request.UserStatistics)
        {
            var userIsValid = await ValidateUser(userStat.UserId, ct);
            if (!userIsValid)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private async Task<bool> CreateUserStatisticsValidation(Guid userId, CancellationToken ct)
    {
        var userIsValid = await ValidateUserAlreadyExists(userId, ct);
        if (!userIsValid)
        {
            return false;
        }
        
        Level? level = await _userProfileRepository.GetLowestLevel(ct);
        if (level == null)
        {
            _validationStorage.AddError(ErrorCode.UnknownLevel, "Levels don't exists, got");
            return false;
        }
        
        return true;
    }
    
    private async Task<bool> GetUserProfileValidation(Guid userId, CancellationToken ct)
    {
        var userIsValid = await ValidateUser(userId, ct);
        if (!userIsValid)
        {
            return false;
        }

        return true;
    }

    #endregion
}