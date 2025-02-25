using Steam.Models.SteamCommunity;
using Steam.Models.SteamUserAuth;

namespace IdentityServer.Services.Steam
{
    public interface ISteamService
    {
        public Task<SteamUserAuthResponse> ValidateSteamToken(string SteamToken);

        public Task<PlayerSummaryModel> FetchUserInformation(ulong SteamId);
    }
}
