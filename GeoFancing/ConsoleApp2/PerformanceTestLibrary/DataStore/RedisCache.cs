using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Diagnostics;

namespace PerformanceTestLibrary
{
    public class RedisCache : IDataStore
    {        
        private readonly RedisConnector _redisConnector;

        public RedisCache(RedisConnector redisConnector)
        {   
           _redisConnector = redisConnector;
        }

        public T Get<T>(string key, ref string res)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            IDatabase redisCache = _redisConnector.Connection;
            var cacheEntry = redisCache.StringGet(key);
            res += Environment.NewLine + "Time took to fetch from internal Reids  in milliseconds " + stopwatch.ElapsedMilliseconds;
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public  void Put<T>(string key, T instance)
        {
            IDatabase redisCache = _redisConnector.Connection;            
            var json = JsonConvert.SerializeObject(instance);
            redisCache.StringSet(key, json);

        }
    }
}
