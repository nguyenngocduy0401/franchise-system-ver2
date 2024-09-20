using FranchiseProject.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FranchiseProject.API.Middlewares
{
    public class RedisAuthenticationMiddleware : IMiddleware
    {
        private readonly IRedisService _redisService;

        public RedisAuthenticationMiddleware(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
           
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (await _redisService.CheckJwtTokenExistsAsync(token))
                {
                   
                    await next(context);
                    return;
                }
                else {context.Response.StatusCode = StatusCodes.Status401Unauthorized; return; }

            }
            await next(context);
            return;

        }
    }
}