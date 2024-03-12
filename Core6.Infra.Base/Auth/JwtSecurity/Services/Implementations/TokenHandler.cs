using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations
{
    public class TokenHandler : ITokenHandler
    {
        #region ctor
        readonly JwtSecurityOptions _options;
        readonly SymmetricSecurityKey _symmetricSecurityKey;

        public TokenHandler(JwtSecurityOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.IssuerSigningKey));
        }
        #endregion

        #region GenerateToken
        public virtual string GenerateToken(IEnumerable<Claim> claims, DateTime? notBefore, DateTime? expires)
        {
            var jwt = new JwtSecurityToken(
              _options.Issuer,
              claims: claims,
              notBefore: notBefore,
              expires: expires,
              signingCredentials: new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        #endregion

        #region ValidateToken
        public virtual ClaimsPrincipal ValidateToken(string token)
        {
            return new JwtSecurityTokenHandler().ValidateToken(token, TokenValidationParameters, out _);
        }
        #endregion

        #region TokenValidationParameters
        public virtual TokenValidationParameters TokenValidationParameters =>
            new TokenValidationParameters
            {
                ValidIssuer = _options.Issuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey
            };
        #endregion
    }
}