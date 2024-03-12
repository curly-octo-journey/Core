using Core6.Infra.Base.Auth.JwtSecurity.Middleware;
using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Core6.Infra.Base.Auth.JwtSecurity.Extensions
{
    public static class JwtServerAppBuilderExtensions
    {
        #region ctor
        const string XFormUrlEncoded = "application/x-www-form-urlencoded";
        #endregion

        #region UseJwtServer
        public static IApplicationBuilder UseJwtServer(this IApplicationBuilder app, Action<JwtServerOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var jwtServerOptions = new JwtServerOptions();
            configureOptions(jwtServerOptions);

            app.MapWhen(context => IsValidJwtMiddlewareRequest(context, jwtServerOptions.TokenEndpointPath),
                     appBuilder => appBuilder.UseMiddleware<JwtServerMiddleware>(jwtServerOptions));

            return app;
        }
        #endregion

        #region IsValidJwtMiddlewareRequest
        private static bool IsValidJwtMiddlewareRequest(HttpContext context, string tokenPath)
        {
            return context.Request.Method == HttpMethods.Post &&
                   context.Request.ContentType == XFormUrlEncoded &&
                   context.Request.Path == tokenPath;
        }
        #endregion
    }
}