using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Implementations;

namespace Core6.Infra.Base.Auth.JwtSecurity.Extensions
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, Action<JwtSecurityOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var jwtSecurityOptions = new JwtSecurityOptions();
            configureOptions(jwtSecurityOptions);

            var tokenHandler = new TokenHandler(jwtSecurityOptions);

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenHandler.TokenValidationParameters;
                });

            return services;
        }
    }
}
