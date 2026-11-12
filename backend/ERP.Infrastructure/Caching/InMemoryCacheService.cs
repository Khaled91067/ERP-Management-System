namespace ERP.Infrastructure.Caching;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions.Caching;

public sealed class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (object? Value, DateTime? ExpiresAt)> _cache = new(StringComparer.Ordinal);

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt is null || entry.ExpiresAt > DateTime.UtcNow)
            {
                return Task.FromResult(entry.Value is T typed ? typed : default);
            }

            _cache.TryRemove(key, out _);
        }

        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        DateTime? expiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null;
        _cache[key] = (value, expiresAt);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        var matchingKeys = _cache.Keys.Where(k => k.StartsWith(prefixKey, StringComparison.Ordinal)).ToList();
        foreach (var key in matchingKeys)
        {
            _cache.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        bool cacheNullValue = false,
        CancellationToken cancellationToken = default)
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
