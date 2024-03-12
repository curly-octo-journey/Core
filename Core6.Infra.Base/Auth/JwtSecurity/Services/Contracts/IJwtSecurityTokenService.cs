using Core6.Infra.Base.Auth.JwtSecurity.Context;
using Core6.Infra.Base.Auth.JwtSecurity.Models;
using Core6.Infra.Base.Auth.JwtSecurity.Options;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts
{
    public interface IJwtSecurityTokenService
    {
        Task<JwtToken> CreateAsync(BaseValidatingContext baseValidatingContext, JwtServerOptions jwtServerOptions);
    }
}
