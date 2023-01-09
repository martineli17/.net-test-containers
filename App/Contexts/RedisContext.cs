using StackExchange.Redis;
using System.Text.Json;

namespace App.Controllers
{
    public class RedisContext : IRedisContext
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _dataBase;
        public RedisContext(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _dataBase = _connection.GetDatabase();
        }

        public async Task AddAsync<TContent>(string key, TContent content, int minutes)
        {
            var redisContent = new RedisValue(JsonSerializer.Serialize(content));
            await _dataBase.StringSetAsync(key, redisContent, TimeSpan.FromMinutes(minutes));
        }

        public async Task<TContent?> GetAsync<TContent>(string key)
        {
            var content = await _dataBase.StringGetAsync(key);
            return JsonSerializer.Deserialize<TContent>(content.ToString());
        }
    }
}
