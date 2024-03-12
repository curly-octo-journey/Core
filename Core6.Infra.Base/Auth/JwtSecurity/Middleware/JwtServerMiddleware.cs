using Core6.Infra.Base.Auth.JwtSecurity.Constants;
using Core6.Infra.Base.Auth.JwtSecurity.Context;
using Core6.Infra.Base.Auth.JwtSecurity.Options;
using Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core6.Infra.Base.Auth.JwtSecurity.Middleware
{
    public class JwtServerMiddleware
    {
        #region ctor
        readonly JwtServerOptions _jwtServerOptions;

        public JwtServerMiddleware(RequestDelegate next, JwtServerOptions jwtServerOptions)
        {
            _jwtServerOptions = jwtServerOptions ?? throw new ArgumentNullException(nameof(jwtServerOptions));
            if (_jwtServerOptions.RefreshTokenOptions != null)
            {
                _jwtServerOptions.RefreshTokenOptions.Issuer = _jwtServerOptions.RefreshTokenOptions.Issuer ?? $"{_jwtServerOptions.Issuer}_rt";
                _jwtServerOptions.RefreshTokenOptions.IssuerSigningKey = _jwtServerOptions.RefreshTokenOptions.IssuerSigningKey ?? $"{_jwtServerOptions.IssuerSigningKey}_rt";
            }
        }
        #endregion

        #region InvokeAsync
        public async Task InvokeAsync(HttpContext context, IJwtSecurityTokenService iJwtSecurityTokenService)
        {
            var baseValidatingContext = default(BaseValidatingContext);
            var grantType = GetGrantType(context);
            switch (grantType)
            {
                case Parameters.Password:
                    baseValidatingContext = GrantResourceOwnerCredentialsContext.Create(context);
                    if (baseValidatingContext != null)
                    {
                        await _jwtServerOptions.AuthorizationServerProvider.GrantResourceOwnerCredentials((GrantResourceOwnerCredentialsContext)baseValidatingContext);
                    }
                    break;
                case Parameters.RefreshToken:
                    baseValidatingContext = GrantRefreshTokenContext.Create(context, _jwtServerOptions);
                    if (baseValidatingContext != null)
                    {
                        await _jwtServerOptions.AuthorizationServerProvider.GrantRefreshToken((GrantRefreshTokenContext)baseValidatingContext);
                    }
                    break;
                case Parameters.ClientCredentials:
                    baseValidatingContext = GrantClientCredentialsContext.Create(context);
                    if (baseValidatingContext != null)
                    {
                        await _jwtServerOptions.AuthorizationServerProvider.GrantClientCredentials((GrantClientCredentialsContext)baseValidatingContext);
                    }
                    break;
            }
            if (baseValidatingContext != null)
            {
                if (baseValidatingContext.IsValidated)
                {
                    var token = await iJwtSecurityTokenService.CreateAsync(baseValidatingContext, _jwtServerOptions);
                    if (baseValidatingContext.Properties != null)
                    {
                        var novo = new JObject
                        {
                            {"AccessToken", token.AccessToken},
                            {"ExpiresIn", token.ExpiresIn},
                            {"RefreshToken", token.RefreshToken},
                            {"TokenType", token.TokenType}
                        };
                        var type = baseValidatingContext.Properties.GetType();
                        var properties = type.GetProperties();
                        foreach (var propertyInfo in properties)
                        {
                            var value = propertyInfo.GetValue(baseValidatingContext.Properties);
                            novo.Add(propertyInfo.Name, value == null ? null : JToken.FromObject(value));
                        }
                        await WriteResponseAsync(context, JsonConvert.SerializeObject(novo));
                    }
                    else
                    {
                        await WriteResponseAsync(context, JsonConvert.SerializeObject(token));
                    }
                }
                else
                {
                    await WriteResponseError(context, baseValidatingContext.Error);
                }
            }
        }
        #endregion

        #region GetGrantType
        private static string GetGrantType(HttpContext context)
        {
            var result = default(string);
            var requestForm = context.Request.Form;
            if (requestForm.ContainsKey(Parameters.GrantType))
            {
                result = requestForm[Parameters.GrantType].FirstOrDefault();
            }
            return result;
        }
        #endregion

        #region WriteResponseAsync
        private static async Task WriteResponseAsync(HttpContext context, string content)
        {
            const string contentType = "application/json";

            context.Response.Headers[HeaderNames.ContentType] = contentType;
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(content);
        }
        #endregion

        #region WriteResponseError
        private static async Task WriteResponseError(HttpContext context, string error)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(error);
        }
        #endregion
    }
}
