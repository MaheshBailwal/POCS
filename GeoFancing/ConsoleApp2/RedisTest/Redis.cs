using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class RedisCache
    {
        IDistributedCache _distributedCache;

        public RedisCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T Get<T>(string key)
        {
            _distributedCache.Refresh(key);

            var cacheEntry = _distributedCache.GetString(key);
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public  void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);

            //var distributedCacheEntryOptions = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpiration = absoluteExpiration,
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            //};

            _distributedCache.SetString(key, json);

        }
    }
}
