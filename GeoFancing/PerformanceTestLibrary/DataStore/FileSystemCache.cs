using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace PerformanceTestLibrary
{
    public class FileSystemCache : INonQueryableDataStore
    {
        static string rootFolder = @"F:\SiteData";

        public FileSystemCache()
        {
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        public T Get<T>(string key, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cacheEntry = File.ReadAllText(Path.Combine(rootFolder, key + ".json"));
            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;            
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            File.WriteAllText(Path.Combine(rootFolder, key + ".json"), json);
        }
    }
}
