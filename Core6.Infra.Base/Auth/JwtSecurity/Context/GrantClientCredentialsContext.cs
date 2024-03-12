using Core6.Infra.Base.Auth.JwtSecurity.Constants;
using Microsoft.AspNetCore.Http;

namespace Core6.Infra.Base.Auth.JwtSecurity.Context
{
    public class GrantClientCredentialsContext : BaseValidatingContext
    {
        #region ctor
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        public GrantClientCredentialsContext(HttpContext context, string clientId, string clientSecret)
            : base(context)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
        #endregion

        #region Create
        public static GrantClientCredentialsContext Create(HttpContext context)
        {
            var requestForm = context.Request.Form;
            if (!requestForm.ContainsKey(Parameters.GrantType))
            {
                return null;
            }
            var grandTypeValue = requestForm[Parameters.GrantType].FirstOrDefault();
            if (grandTypeValue != Parameters.ClientCredentials)
            {
                return null;
            }
            var clientId = requestForm[Parameters.ClientId].FirstOrDefault();
            var clientSecret = requestForm[Parameters.ClientSecret].FirstOrDefault();
            return new GrantClientCredentialsContext(context, clientId, clientSecret);
        }
        #endregion
    }
}