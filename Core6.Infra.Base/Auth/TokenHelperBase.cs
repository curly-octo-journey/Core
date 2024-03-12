using Core6.Infra.Base.Criptografia;
using System.Security.Claims;
using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;

namespace Core6.Infra.Base.Auth
{
    public class TokenHelperBase
    {
        #region GerarToken Padrao
        public static string GerarToken(string stringConexao, int codigoUsuario, int codigoTenant)
        {
            var dados = new Dictionary<string, object>
            {
                { ClaimTypes.Name, codigoUsuario.ToString() },
                { "SC", CriptografiaSB2.Criptografar(stringConexao,21) },
                { "TENANT", codigoTenant.ToString() }
            };

            var startingDate = DateTime.UtcNow;
            var expiresDate = DateTime.UtcNow.Add(TimeSpan.FromHours(2));
            var listaClaim = new List<Claim>();
            foreach (var dadoClaim in dados)
            {
                listaClaim.Add(new Claim(dadoClaim.Key, dadoClaim.Value.ToString()));
            }
            var clains = new ClaimsIdentity(listaClaim);
            var jwtSecurityOptions = new JwtSecurityOptions
            {
                Issuer = AutorizacaoCore.Issuer,
                IssuerSigningKey = AutorizacaoCore.IssuerSigningKey
            };
            var tokenHandler = new TokenHandler(jwtSecurityOptions);
            var token = tokenHandler.GenerateToken(clains.Claims, startingDate, expiresDate);

            return token;
        }
        #endregion
    }
}
