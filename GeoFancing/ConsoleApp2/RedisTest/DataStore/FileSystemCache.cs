using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class FileSystemCache : IDataStore
    {
        static string rootFolder = @"F:\SiteData";

        public FileSystemCache()
        {
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        public T Get<T>(string key, ref string res)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cacheEntry = File.ReadAllText(Path.Combine(rootFolder, key + ".json"));
            stopwatch.Stop();
            res += Environment.NewLine + "Time took to fetch from file system VM in milliseconds " + stopwatch.ElapsedMilliseconds;
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            File.WriteAllText(Path.Combine(rootFolder, key + ".json"), json);
        }
    }
}
