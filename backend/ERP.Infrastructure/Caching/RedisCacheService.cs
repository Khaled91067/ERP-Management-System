
namespace ERP.Infrastructure.Caching;

using System.Text.Json;

using ERP.Application.Abstractions.Caching;

using Microsoft.Extensions.Logging;

using StackExchange.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        _jsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }

    private IDatabase Database => _connectionMultiplexer.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedValue = await Database.StringGetAsync(key);
            if (cachedValue.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache miss for key: {CacheKey}", key);
                return default;
            }

            _logger.LogInformation("Cache hit for key: {CacheKey}", key);
            return JsonSerializer.Deserialize<T>(cachedValue.ToString(), _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache failed for key: {CacheKey}. Falling back to database.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            await Database.StringSetAsync(key, serializedValue, expiration);
            _logger.LogInformation("Cache set for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache failed to set key: {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await Database.KeyDeleteAsync(key);
            _logger.LogInformation("Cache removed for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache failed to remove key: {CacheKey}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            if (endpoints.Length == 0) return;

            var server = _connectionMultiplexer.GetServer(endpoints.First());
            
            // Using KEYS command pattern as requested for simplicity without advanced Redis search features
            var keys = server.Keys(pattern: $"{prefixKey}*").ToArray();
            if (keys.Any())
            {
                await Database.KeyDeleteAsync(keys);
                _logger.LogInformation("Cache removed {Count} keys with prefix: {PrefixKey}", keys.Length, prefixKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache failed to remove keys by prefix: {PrefixKey}", prefixKey);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, bool cacheNullValue = false, CancellationToken cancellationToken = default)
    {
        var cachedResult = await GetAsync<T>(key, cancellationToken);
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var result = await factory(cancellationToken);
        
        if (result is not null || cacheNullValue)
        {
            await SetAsync(key, result, expiration, cancellationToken);
        }

        return result;
    }
}
