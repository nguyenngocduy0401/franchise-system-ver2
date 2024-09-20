using FranchiseProject.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class RedisService  : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> StoreJwtTokenAsync(string userName, string jwtToken)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) 
            };

            await RemoveUserIfExistsAsync(userName);
            await _cache.SetStringAsync(userName, jwtToken, options);


            await _cache.SetStringAsync($"jwt:{jwtToken}", userName, options);

            return true;
        }

        public async Task<bool> CheckUserExistAsync(string userName)
        {
            var token = await _cache.GetStringAsync(userName);
            return token != null;
        }

        public async Task<bool> CheckJwtTokenExistsAsync(string jwtToken)
        {
            var userName = await _cache.GetStringAsync($"jwt:{jwtToken}");
            return userName != null;
        }

        public async Task RemoveUserIfExistsAsync(string userName)
        {
            var jwtToken = await _cache.GetStringAsync(userName);

            if (jwtToken != null)
            {
  
                await _cache.RemoveAsync(userName);

                await _cache.RemoveAsync($"jwt:{jwtToken}");
            }
        }

    }
}