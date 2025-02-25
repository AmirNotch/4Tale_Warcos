using Steam.Models.SteamCommunity;
using Steam.Models.SteamUserAuth;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace IdentityServer.Services.Steam {
    public class SteamService : ISteamService {
        private readonly ILogger _logger;
        private readonly ISteamWebInterfaceFactory _steamFactory;

        public SteamService(ILogger<SteamService> logger) {
            _logger = logger;

            string steamWebApiKey = Environment.GetEnvironmentVariable("STEAM_WEB_API_KEY")!;
            _steamFactory = new SteamWebInterfaceFactory(steamWebApiKey);
        }

        public async Task<SteamUserAuthResponse> ValidateSteamToken(string SteamToken) {
            string steamAppIdString = Environment.GetEnvironmentVariable("STEAM_APP_ID")!;
            uint steamAppId = uint.Parse(steamAppIdString);

            SteamUserAuth steamAuthInterface = _steamFactory.CreateSteamWebInterface<SteamUserAuth>();
            var responseData = await steamAuthInterface.AuthenticateUserTicket(steamAppId, SteamToken);

            return responseData.Data.Response;
        }

        public async Task<PlayerSummaryModel> FetchUserInformation(ulong SteamId) {
            SteamUser steamAuthInterface = _steamFactory.CreateSteamWebInterface<SteamUser>();
            var responseData = await steamAuthInterface.GetPlayerSummaryAsync(SteamId);

            return responseData.Data;
        }
    }
}
