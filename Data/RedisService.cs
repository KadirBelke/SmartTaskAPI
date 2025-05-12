using StackExchange.Redis;

namespace SmartTaskAPI.Data
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConfiguration config)
        {
            _redis = ConnectionMultiplexer.Connect(config["Redis:Connection"]);
            _db = _redis.GetDatabase();
        }

        public async Task SetRefreshTokenAsync(int userId, string token, TimeSpan expiry)
        {
            await _db.StringSetAsync($"refresh:user:{userId}", token, expiry);
        }

        public async Task<string?> GetRefreshTokenAsync(int userId)
        {
            var value = await _db.StringGetAsync($"refresh:user:{userId}");
            return value.HasValue ? value.ToString() : null;
        }
    }
}
