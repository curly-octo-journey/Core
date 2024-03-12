using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;
using Invio.Extensions.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Core6.Infra.Base.Auth.JwtSecurity.Options;

namespace Core6.Infra.Base.Auth
{
    public static class AutorizacaoCore
    {
        #region AddJwtBearerAuthenticationCore
        public static string Issuer = "https://core.com.br";
        public static string IssuerSigningKey = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";
        public static TokenHandler TokenHandler;

        public static AuthenticationBuilder AddJwtBearerAuthenticationCore(this IServiceCollection services, string issuer = null, string issuerSigningKey = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            Issuer = issuer ?? Issuer;
            IssuerSigningKey = issuerSigningKey ?? IssuerSigningKey;
            var jwtSecurityOptions = new JwtSecurityOptions
            {
                Issuer = Issuer,
                IssuerSigningKey = IssuerSigningKey
            };
            TokenHandler = new TokenHandler(jwtSecurityOptions);
            return services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = TokenHandler.TokenValidationParameters;
                    options.AddQueryStringAuthentication("BearerToken");
                });
        }
        #endregion
    }
}