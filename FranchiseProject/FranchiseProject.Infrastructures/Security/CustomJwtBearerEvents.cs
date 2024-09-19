/*using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FranchiseProject.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    private readonly IRedisService _redisService;

    public CustomJwtBearerEvents(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var token = context.SecurityToken as JwtSecurityToken;
        if (token != null)
        {
            var tokenString = token.RawData;
            Console.WriteLine($"Checking token: {tokenString}");

            if (!await _redisService.CheckJwtTokenExistsAsync(tokenString))
            {
                Console.WriteLine("Token no longer valid.");
                context.Fail("Token is no longer valid.");
            }
        }

        await base.TokenValidated(context);
    }
}*/