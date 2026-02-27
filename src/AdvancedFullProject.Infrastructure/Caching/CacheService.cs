using System.Text.Json;
using AdvancedFullProject.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace AdvancedFullProject.Infrastructure.Caching;

public sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public CacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return value is null ? default : JsonSerializer.Deserialize<T>(value);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken) => _cache.RemoveAsync(key, cancellationToken);

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken)
        => _cache.SetStringAsync(key, JsonSerializer.Serialize(value), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, cancellationToken);
}
