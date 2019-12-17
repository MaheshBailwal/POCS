using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class RedisCache : IDataStore
    {
        IDistributedCache _distributedCache;

        public RedisCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T Get<T>(string key, ref string res)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _distributedCache.Refresh(key);

            var cacheEntry = _distributedCache.GetString(key);
            res += Environment.NewLine + "Time took to fetch from internal Reids  in milliseconds " + stopwatch.ElapsedMilliseconds;
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public  void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            _distributedCache.SetString(key, json);

        }
    }
}
