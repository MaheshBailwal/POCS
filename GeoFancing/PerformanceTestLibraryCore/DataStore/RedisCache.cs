﻿using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Diagnostics;

namespace PerformanceTestLibrary
{
    public class RedisCache : INonQueryableDataStore
    {        
        private readonly RedisConnector _redisConnector;

        public RedisCache(RedisConnector redisConnector)
        {   
           _redisConnector = redisConnector;
        }
        public DataStoreType DataStoreType => DataStoreType.RedisCache;

        public T Get<T>(string key, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            IDatabase redisCache = _redisConnector.Connection;
            var cacheEntry = redisCache.StringGet(key);
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;            
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
