﻿namespace Core6.Infra.Base.Auth.JwtSecurity.Models
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
    }
}
