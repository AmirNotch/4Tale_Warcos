using AngleSharp.Io;
using IdentityModel;
using IdentityServer.Models;
using IdentityServer.Services.Steam;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static IdentityServer4.Events.TokenIssuedSuccessEvent;

namespace AuthService.Extensions.ExternalAuth
{
    public class ExternalAuthGrandTypeValidator : IExtensionGrantValidator
    {
        // External providers validators
        private readonly ISteamService _steamService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public string GrantType => "external";

        public ExternalAuthGrandTypeValidator(
            ISteamService steamService, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _steamService = steamService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // Provider
            var provider = context.Request.Raw["external_provider"] ?? string.Empty;
            var providerNormalized = provider.ToLower();

            // Token
            var token = context.Request.Raw["external_token"] ?? string.Empty;

            // Sandbox mode
            bool sandbox;
            var sandboxRaw = context.Request.Raw["sandbox"] ?? "false";
            bool.TryParse(sandboxRaw, out sandbox);

            var foundUser = sandbox
                ? await GetUserBySandbox() 
                : await GetUserByExternalProvider(providerNormalized, token);
            
            if (foundUser == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            context.Result = new GrantValidationResult(foundUser.Id, GrantType);
            return;
        }

        private async Task<string> ValidateExternalToken(string provider, string token)
        {
            // todo(dimasterrr) normalize provider before
            switch (provider)
            {
                case "steam":
                    var response = await _steamService.ValidateSteamToken(token);
                    return response.Success ? response.Params.SteamId : string.Empty;

                default: return string.Empty;
            }
        }

        private async Task<string> FetchExternalUserName(string provider, string providerUserId)
        {
            // todo(dimasterrr) normalize provider before
            switch (provider)
            {
                case "steam":
                    var response = await _steamService.FetchUserInformation(ulong.Parse(providerUserId));
                    return response != null ? response.Nickname: providerUserId.ToString();

                default: return providerUserId.ToString();
            }
        }

        private async Task<ApplicationUser?> GetUserByExternalProvider(string provider, string token)
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(token)) return null;

            var externalUserId = await ValidateExternalToken(provider, token);
            if (string.IsNullOrEmpty(externalUserId)) return null;

            // Find or Create user
            var foundUser = await _userManager.FindByLoginAsync(provider, externalUserId);
            if (foundUser == null)
            {
                // todo(dimasterrr) need generate unique username
                foundUser = new ApplicationUser() { UserName = externalUserId };

                var CreateUserResult = await _userManager.CreateAsync(foundUser);
                if (!CreateUserResult.Succeeded) return null;

                var LinkExternalLoginResult = await _userManager.AddLoginAsync(foundUser, new UserLoginInfo(provider, externalUserId, provider));
                if (!LinkExternalLoginResult.Succeeded) return null;

                // set default user info from linked account
                var externalUserNickname = await FetchExternalUserName(provider, externalUserId);
                var CreateClaimAsync = await _userManager.AddClaimAsync(foundUser, new Claim(JwtClaimTypes.NickName, externalUserNickname));
                if (!CreateClaimAsync.Succeeded) return null;
            }

            return foundUser;
        }

        private async Task<ApplicationUser?> GetUserBySandbox()
        {
            // Find or Create user
            var foundUser = await _userManager.FindByNameAsync("sandbox-user");
            if (foundUser == null)
            {
                foundUser = new ApplicationUser() { UserName = "sandbox-user" };

                var CreateUserResult = await _userManager.CreateAsync(foundUser);
                if (!CreateUserResult.Succeeded) return null;

                var CreateClaimAsync = await _userManager.AddClaimAsync(foundUser, new Claim(JwtClaimTypes.NickName, "Sandbox User"));
                if (!CreateClaimAsync.Succeeded) return null;
            }

            return foundUser;
        }
    }
}
