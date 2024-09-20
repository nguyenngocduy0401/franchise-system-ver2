using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RedisTestController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public RedisTestController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestRedis()
        {
            // Lưu một giá trị vào Redis
            string cacheKey = "TestKey";
            string valueToCache = "Hello from Redis!";
            var encodedValue = Encoding.UTF8.GetBytes(valueToCache);

            // Cấu hình cho Redis cache (có thể hết hạn sau 10 phút)
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));

            // Set giá trị vào cache
            await _cache.SetAsync(cacheKey, encodedValue, options);

            // Lấy giá trị từ Redis
            var cachedValue = await _cache.GetAsync(cacheKey);
            if (cachedValue != null)
            {
                var decodedValue = Encoding.UTF8.GetString(cachedValue);
                return Ok(new { Message = "Redis connected successfully!", CachedValue = decodedValue });
            }
            else
            {
                return StatusCode(500, "Redis not responding.");
            }
        }
    }
}