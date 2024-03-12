using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Core6.Infra.Base.Auth.JwtSecurity.Extensions
{
    public static class JwtServerServiceCollectionExtensions
    {
        #region AddJwtServer
        public static IServiceCollection AddJwtServer(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IAuthorizationServerProvider, AuthorizationServerProvider>();
            services.AddSingleton<IJwtSecurityTokenService, JwtSecurityTokenService>();

            return services;
        }
        #endregion
    }
}