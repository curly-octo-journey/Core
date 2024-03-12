using Core6.Infra.Base.Auth.JwtSecurity.Constants;
using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;

namespace Core6.Infra.Base.Auth.JwtSecurity.Context
{
    public class GrantRefreshTokenContext : BaseValidatingContext
    {
        #region ctor
        private GrantRefreshTokenContext(HttpContext context, IEnumerable<Claim> claims)
            : base(context)
        {
            Validated(claims);
        }
        #endregion

        #region Create
        public static GrantRefreshTokenContext Create(HttpContext context, JwtServerOptions jwtServerOptions)
        {
            var requestForm = context.Request.Form;
            if (!requestForm.ContainsKey(Parameters.GrantType))
            {
                return null;
            }
            var grandTypeValue = requestForm[Parameters.GrantType].FirstOrDefault();
            if (grandTypeValue != Parameters.RefreshToken)
            {
                return null;
            }
            var token = requestForm[Parameters.RefreshToken].FirstOrDefault();
            var tokenHandler = new TokenHandler(jwtServerOptions.RefreshTokenOptions);
            var claim = tokenHandler.ValidateToken(token);
            if (claim == null
             || !claim.Identity.IsAuthenticated)
            {
                return null;
            }
            return new GrantRefreshTokenContext(context, claim.Claims);
        }
        #endregion
    }
}
