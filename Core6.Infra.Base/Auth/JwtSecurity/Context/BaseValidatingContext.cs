using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core6.Infra.Base.Auth.JwtSecurity.Context
{
    public class BaseValidatingContext
    {
        public HttpContext Context { get; }
        public IEnumerable<Claim> Claims { get; private set; }
        public object Properties { get; private set; }

        public BaseValidatingContext(HttpContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public HttpRequest Request => Context.Request;

        public HttpResponse Response => Context.Response;

        /// <summary>
        /// True if application code has called any of the Validate methods on this context.
        /// </summary>
        public bool IsValidated { get; private set; }

        /// <summary>
        /// True if application code has called the SetError methods on this context.
        /// </summary>
        public bool HasError { get; private set; }

        /// <summary>
        /// The error argument provided when SetError was called on this context.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Marks this context as validated by the application. IsValidated becomes true and HasError becomes false as a result of calling.
        /// </summary>
        /// <returns>True if the validation has taken effect.</returns>
        public bool Validated(IEnumerable<Claim> claims, object properties = null)
        {
            Claims = claims;
            IsValidated = true;
            HasError = false;
            Properties = properties;
            return true;
        }

        public void SetError(string error)
        {
            Error = error;
            IsValidated = false;
            HasError = true;
        }
    }
}
