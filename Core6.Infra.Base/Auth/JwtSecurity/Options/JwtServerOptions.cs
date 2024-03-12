using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;

namespace Core6.Infra.Base.Auth.JwtSecurity.Options
{
    public class JwtServerOptions : JwtSecurityOptions
    {
        public IAuthorizationServerProvider AuthorizationServerProvider { get; set; }
        public TimeSpan AccessTokenExpireTimeSpan { get; set; }
        public string TokenEndpointPath { get; set; }
        public RefreshTokenOptions RefreshTokenOptions { get; set; }
    }
}
