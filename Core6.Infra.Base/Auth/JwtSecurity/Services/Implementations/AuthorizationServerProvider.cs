using Core6.Infra.Base.Auth.JwtSecurity.Context;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations
{
    public class AuthorizationServerProvider : IAuthorizationServerProvider
    {
        public Func<GrantResourceOwnerCredentialsContext, Task> OnGrantResourceOwnerCredentialsAsync { get; set; }
        public Func<GrantRefreshTokenContext, Task> OnGrantRefreshToken { get; set; }
        public Func<GrantClientCredentialsContext, Task> OnGrantClientCredentials { get; set; }

        public AuthorizationServerProvider()
        {
            OnGrantResourceOwnerCredentialsAsync = context => Task.FromResult<object>(null);
            OnGrantRefreshToken = context => Task.FromResult<object>(null);
        }

        public virtual Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context)
        {
            return OnGrantResourceOwnerCredentialsAsync.Invoke(context);
        }

        public virtual Task GrantRefreshToken(GrantRefreshTokenContext context)
        {
            return OnGrantRefreshToken.Invoke(context);
        }

        public virtual Task GrantClientCredentials(GrantClientCredentialsContext context)
        {
            return OnGrantClientCredentials.Invoke(context);
        }
    }
}
