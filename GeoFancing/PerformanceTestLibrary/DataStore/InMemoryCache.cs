using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PerformanceTestLibrary
{
    public class InMemoryCache : INonQueryableDataStore
    {
        static Dictionary<string, object> _sites;

        public InMemoryCache()
        {
            if (_sites == null)
            {
                _sites = new Dictionary<string, object>();
            }
        }

        public T Get<T>(string key, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = _sites[key];
            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;            
            return (T)Convert.ChangeType(site, typeof(T));
         
        }

        public void Put<T>(string key, T instance)
        {
            _sites[key] = instance;
        }
    }
}
