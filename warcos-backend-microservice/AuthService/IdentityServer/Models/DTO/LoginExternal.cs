using IdentityServer4.Extensions;

namespace IdentityServer.Models.DTO {
    public class LoginExternalRequest {
        public required string ClientId { get; set; }
        public required string Provider { get; set; }
        public required string Token { get; set; }

        public bool IsValid() => !ClientId.IsNullOrEmpty() && !Provider.IsNullOrEmpty() && !Token.IsNullOrEmpty();
    }

    public class LoginExternalResponse {
        public required string AccessToken { get; set; }
        public required string IdToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
