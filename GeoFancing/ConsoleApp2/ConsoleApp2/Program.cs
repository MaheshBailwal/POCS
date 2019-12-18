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

            Console.WriteLine("Fetching sites from File System data store");
            FetchDataFromDataStore(fileSystemDataStore);
            Console.WriteLine("Fetching sites from Memory Cache data store");
            FetchDataFromDataStore(inMemoryCache);
            Console.WriteLine("Fetching sites from Redis Cache data store");
            FetchDataFromDataStore(redisCache);
            Console.ReadLine();
        } 
        
        public static void FetchDataFromDataStore(IDataStore dataStore)
        {
            Console.WriteLine("Fetching sites from data stores");

            for (var count = 1; count < 10; count++)
            {
                string result = "";
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var site = dataStore.Get<Site>(count.ToString(), ref result);

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault(z => z.Rectangle.Contains(x, y));

                //if yes then then find whether corodiante exist in polygon
                if (zone != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                    if (found)
                    {
                        Console.WriteLine("Inside the polygon");
                    }
                }

                Console.WriteLine("Time to serach in  milliseconds " + stopwatch.ElapsedMilliseconds);
            }            
        }
    }
}
