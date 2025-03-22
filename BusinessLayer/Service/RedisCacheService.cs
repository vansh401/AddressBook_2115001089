using BusinessLayer.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheLayer.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _cacheDb;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _cacheDb = redis.GetDatabase();
        }

        public async Task<T?> GetCachedData<T>(string key)
        {
            var cachedData = await _cacheDb.StringGetAsync(key);
            return cachedData.HasValue ? JsonSerializer.Deserialize<T>(cachedData) : default;
        }

        public async Task SetCachedData<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(value);
                var result = await _cacheDb.StringSetAsync(key, jsonData, expiration);
                if (result)
                {
                    Console.WriteLine($"✅ Data cached successfully with key: {key}");
                }
                else
                {
                    Console.WriteLine($"❗ Failed to cache data for key: {key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Error caching data for key {key}: {ex.Message}");
            }
        }



        public async Task<bool> RemoveCachedData(string key)
        {
            return await _cacheDb.KeyDeleteAsync(key);
        }
    }
}