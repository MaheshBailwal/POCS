using PerformanceTestLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating sites");
            var sites = Util.CreateSites();

            // Create redis connector
            RedisConnector redisConnector = new RedisConnector("127.0.0.1:6379");

            // File system data store
            IDataStore fileSystemDataStore = new FileSystemCache();
            IDataStore inMemoryCache = new InMemoryCache();
            IDataStore redisCache = new RedisCache(redisConnector);

            Console.WriteLine("Putting sites in all data stores");
            foreach (var key in sites.Keys)
            {
                fileSystemDataStore.Put(key.ToString(), sites[key]);
                inMemoryCache.Put(key.ToString(), sites[key]);
                redisCache.Put(key.ToString(), sites[key]);
            }
           
            FetchDataFromDataStore(fileSystemDataStore);          
            FetchDataFromDataStore(inMemoryCache);         
            FetchDataFromDataStore(redisCache);
            Console.ReadLine();
        }

        public static long FetchDataFromDataStore(IDataStore dataStore)
        {
            long totalFetchTime = 0;
            int totalFetchSites = 100;
            for (var count = 1; count < totalFetchSites; count++)
            {                
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                long fetchTime;
                var site = dataStore.Get<Site>(count.ToString(), out fetchTime);
                totalFetchTime += fetchTime;

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault(z => z.Rectangle.Contains(x, y));

                //if yes then then find whether corodiante exist in polygon
                if (zone != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                    //if (found)
                    //{
                    //    Console.WriteLine("Inside the polygon");
                    //}
                }                
            }

            Console.WriteLine($"Total aggregate fetch time to serach from {dataStore.GetType().Name} in  milliseconds " + ((decimal)totalFetchTime / totalFetchSites));
            return totalFetchTime;
        }
    }
}
