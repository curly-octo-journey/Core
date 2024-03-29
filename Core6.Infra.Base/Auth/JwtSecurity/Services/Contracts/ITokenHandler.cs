﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Core6.Infra.Base.Auth.JwtSecurity.Services.Contracts
{
    public interface ITokenHandler
    {
        string GenerateToken(IEnumerable<Claim> claims, DateTime? notBefore, DateTime? expires);
        ClaimsPrincipal ValidateToken(string token);
        TokenValidationParameters TokenValidationParameters { get; }
    }
}