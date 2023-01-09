using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [ApiController]
    [Route("redis")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisContext _redis;

        public RedisController(IRedisContext redis)
        {
            _redis = redis;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RedisAddDto request)
        {
            await _redis.AddAsync(request.Key, request.Content, 1);
            return StatusCode(201);
        }
    }
}
