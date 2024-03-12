using Core6.Infra.Base.Auth.JwtSecurity.Context;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts
{
    public interface IAuthorizationServerProvider
    {
        Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context);
        Task GrantRefreshToken(GrantRefreshTokenContext context);
        Task GrantClientCredentials(GrantClientCredentialsContext context);
    }
}
