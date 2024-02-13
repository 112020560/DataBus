using DataBus.Application;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DataBus.Infrastructure.Shared;

public class CacheService: ICacheService
{
    private readonly IDistributedCache _distributedCache;
    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<(bool exist, T? information)>  GetCacheInformation<T>(string key, CancellationToken cancellationToken)
    {
         var _cache = await _distributedCache.GetStringAsync(key, cancellationToken);
         if(string.IsNullOrEmpty(_cache))
            return (false, default);
        return (true, JsonConvert.DeserializeObject<T>(_cache));
    }
    public async Task SetCacheInformation<T>(string key, T information, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));//(_backEndSetting.MinutesInRedisCache));
                await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(information), options, cancellationToken);
    }
}
