using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace PerformanceTestLibrary
{
    public class FileSystemCache : INonQueryableDataStore
    {
        string _rootFolder;

        public FileSystemCache(string dataRootFolder)
        {
            _rootFolder = Path.Combine(dataRootFolder, "SiteData");

            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public DataStoreType DataStoreType => DataStoreType.FileSystem;

        public T Get<T>(string key, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cacheEntry = File.ReadAllText(Path.Combine(_rootFolder, key + ".json"));
            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;            
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            File.WriteAllText(Path.Combine(_rootFolder, key + ".json"), json);
        }
    }
}
