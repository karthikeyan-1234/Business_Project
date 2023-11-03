using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonLibrary.Caching
{
    public class CacheManager : ICacheManager
    {
        IDistributedCache cache;
        DistributedCacheEntryOptions options;

        public CacheManager(IDistributedCache cache)
        {
            this.cache = cache;
            options = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) };
        }

        public void SetTimeOut(TimeSpan timeOut)
        {
            options.AbsoluteExpirationRelativeToNow = timeOut;
        }

        public async Task<bool> TrySetAsync<T>(T obj, string key)
        {
            var json = JsonSerializer.Serialize(obj);
            await cache.SetStringAsync(key, json,options);
            return true;
        }

        public async Task<T?> TryGetAsync<T>(string key)
        {
            var json = await cache.GetStringAsync(key);
            if (json != null)
                return JsonSerializer.Deserialize<T>(json);

            return default(T);
        }
    }
}
