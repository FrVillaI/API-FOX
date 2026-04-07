using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using EnvioBackend.DTOs;

namespace EnvioBackend.Cache;

// Simple cache tail service: loads tail on first request, caches for 5 minutes, supports explicit invalidate.
public class CacheTailService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan TailCacheDuration = TimeSpan.FromMinutes(5);
    private const string CacheKeyTail = "EnviosTail";

    public CacheTailService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    // Gets cached tail; if not present, uses the provided loader to fetch data and caches it.
    public async Task<List<EnvioTailDto>> GetTailAsync(Func<Task<List<EnvioTailDto>>> loader)
    {
        if (_memoryCache.TryGetValue(CacheKeyTail, out List<EnvioTailDto>? cached))
        {
            return cached!;
        }

        var tail = await loader();
        // Store with 5 minutes absolute expiration as backup
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TailCacheDuration
        };
        _memoryCache.Set(CacheKeyTail, tail, options);
        return tail;
    }

    // Explicitly invalidate the cache (invoked by UI Recargar button)
    public void InvalidarCache()
    {
        _memoryCache.Remove(CacheKeyTail);
    }
}
