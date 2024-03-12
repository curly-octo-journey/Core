namespace Core6.Infra.Base.Auth.JwtSecurity.Options
{
    public class RefreshTokenOptions : JwtSecurityOptions
    {
        public TimeSpan RefreshTokenExpireTimeSpan { get; set; }
    }
}