using Core6.Infra.Base.Auth.JwtSecurity.Constants;
using Microsoft.AspNetCore.Http;

namespace Core6.Infra.Base.Auth.JwtSecurity.Context
{
    public class GrantResourceOwnerCredentialsContext : BaseValidatingContext
    {
        public GrantResourceOwnerCredentialsContext(HttpContext context, string userName, string password) : base(context)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }

        public static GrantResourceOwnerCredentialsContext Create(HttpContext context)
        {
            var result = default(GrantResourceOwnerCredentialsContext);
            var requestForm = context.Request.Form;
            if (requestForm.ContainsKey(Parameters.GrantType))
            {
                var grandTypeValue = requestForm[Parameters.GrantType].FirstOrDefault();
                if (grandTypeValue == Parameters.Password)
                {
                    var userName = requestForm[Parameters.Username].FirstOrDefault();
                    var password = requestForm[Parameters.Password].FirstOrDefault();

                    result = new GrantResourceOwnerCredentialsContext(context, userName, password);
                }
            }
            return result;
        }
    }
}
