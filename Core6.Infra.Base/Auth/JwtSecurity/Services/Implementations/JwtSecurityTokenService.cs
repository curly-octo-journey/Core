using Core6.Infra.Base.Auth.JwtSecurity.Context;
using Core6.Infra.Base.Auth.JwtSecurity.Models;
using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations
{
    public class JwtSecurityTokenService : IJwtSecurityTokenService
    {
        #region CreateAsync
        public virtual async Task<JwtToken> CreateAsync(BaseValidatingContext baseValidatingContext, JwtServerOptions jwtServerOptions)
        {
            Validate();

            var tokenHandler = new TokenHandler(jwtServerOptions);
            var startingDate = DateTime.UtcNow;
            var expiresDate = DateTime.UtcNow.Add(jwtServerOptions.AccessTokenExpireTimeSpan);

            var token = tokenHandler.GenerateToken(baseValidatingContext.Claims, startingDate, expiresDate);

            var result = new JwtToken
            {
                AccessToken = token,
                ExpiresIn = GetTokenExpiral(startingDate, expiresDate),
                TokenType = JwtBearerDefaults.AuthenticationScheme
            };

            if (jwtServerOptions.RefreshTokenOptions != null)
            {
                var tokenHandlerRefreshToken = new TokenHandler(jwtServerOptions.RefreshTokenOptions);
                var startingDateRefreshToken = DateTime.UtcNow;
                var expiresDateRefreshToken = DateTime.UtcNow.Add(jwtServerOptions.RefreshTokenOptions.RefreshTokenExpireTimeSpan);
                var refreshToken = tokenHandlerRefreshToken.GenerateToken(baseValidatingContext.Claims, startingDateRefreshToken, expiresDateRefreshToken);
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    result.RefreshToken = refreshToken;
                }
            }

            void Validate()
            {
                if (baseValidatingContext == null)
                {
                    throw new ArgumentNullException(nameof(baseValidatingContext));
                }
                if (jwtServerOptions == null)
                {
                    throw new ArgumentNullException(nameof(jwtServerOptions));
                }
            }

            return await Task.FromResult(result);
        }
        #endregion

        #region GetTokenExpiral
        protected virtual int GetTokenExpiral(DateTime startingDate, DateTime expiryDate) => Convert.ToInt32((expiryDate.ToUniversalTime() - startingDate.ToUniversalTime()).TotalSeconds);
        #endregion
    }
}
