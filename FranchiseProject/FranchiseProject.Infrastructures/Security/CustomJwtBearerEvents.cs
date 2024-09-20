using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FranchiseProject.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    private readonly IRedisService _redisService;

    public CustomJwtBearerEvents(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        //Net 8
        var token = context.SecurityToken as JsonWebToken;
        if (token != null)
        {
            var tokenString = token.EncodedToken;

            if (!await _redisService.CheckJwtTokenExistsAsync(tokenString))
            { 
                context.Fail("Token is no longer valid.");
            }
        }

        await base.TokenValidated(context);
    }
}