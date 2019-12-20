﻿using PerformanceTestLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
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

            string redisHost = ConfigurationManager.AppSettings["RedisCacheConfig"];
            string cosmoDatabaseName = ConfigurationManager.AppSettings["CosmoDatabaseName"];
            string cosmoCollectionName = ConfigurationManager.AppSettings["CosmoCollectionName"];
            string cosmoEndpointUrl = ConfigurationManager.AppSettings["CosmoEndpointUrl"];
            string cosmoPrimaryKey = ConfigurationManager.AppSettings["CosmoPrimaryKey"];
            string azureDBConnectionString = ConfigurationManager.AppSettings["AzureDBConnectionString"];


            // Create redis connector
            RedisConnector redisConnector = new RedisConnector(redisHost);
            SiteDBLayer azureDBLayer = new SiteDBLayer(azureDBConnectionString);

            // File system data store
            IDataStore fileSystemDataStore = new FileSystemCache();
            IDataStore inMemoryCache = new InMemoryCache();
            IDataStore redisCache = new RedisCache(redisConnector);
            IDataStorebyPoint azureDB = new AzureSql(azureDBLayer);
            IDataStore cosmoDB = new CosmoDS(cosmoDatabaseName, cosmoCollectionName, cosmoEndpointUrl, cosmoPrimaryKey);



            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic["RedisCacheConfig"] = ConfigurationManager.AppSettings["RedisCacheConfig"];
            dic["CosmoDatabaseName"] = ConfigurationManager.AppSettings["CosmoDatabaseName"];
            dic["CosmoCollectionName"] = ConfigurationManager.AppSettings["CosmoCollectionName"];
            dic["CosmoEndpointUrl"] = ConfigurationManager.AppSettings["CosmoEndpointUrl"];
            dic["CosmoPrimaryKey"] = ConfigurationManager.AppSettings["CosmoPrimaryKey"];
            dic["AzureDBConnectionString"] = ConfigurationManager.AppSettings["AzureDBConnectionString"];


            TestExecuter testExecuter = new TestExecuter(ProgressNotifiactionHandler, 10, 10, 5);


            var response = testExecuter.ExecuteTest(dic, new[] { DataStoreType.FileSystem, DataStoreType.Cosmo });

            foreach (var dataStoreType in response.Keys)
            {
                var metreics = response[dataStoreType];

                Console.WriteLine($"Metrics for datastor {dataStoreType.ToString()}");

                foreach (var metricsType in metreics.Keys)
                {
                    Console.WriteLine($" {metricsType.ToString()} : time in ms { metreics[metricsType]} ");
                }
            }


            Console.WriteLine("Done");

            Console.ReadLine();
            return;

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
            FetchDataFromAzureDB(azureDB);
            FetchDataFromDataStore(cosmoDB);
            Console.ReadLine();
        }

        static void ProgressNotifiactionHandler(string message)
        {
            Console.WriteLine(message);
        }

        public static double FetchDataFromDataStore(IDataStore dataStore)
        {
            double totalFetchTime = 0;
            int totalFetchSites = 100;
            for (var count = 1; count < totalFetchSites; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                double fetchTime;
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
                }
            }

            Console.WriteLine($"Total aggregate fetch time to serach from {dataStore.GetType().Name} in  milliseconds " + ((decimal)totalFetchTime / totalFetchSites));
            return totalFetchTime;
        }

        public static double FetchDataFromAzureDB(IDataStorebyPoint dataStorebypoint)
        {
            double totalFetchTime = 0;
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                double fetchTime;
                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;
                var site = dataStorebypoint.Get<Site>(count.ToString(), x, y, out fetchTime);
                totalFetchTime += fetchTime;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault();

                //if yes then then find whether corodiante exist in polygon
                if (zone != null && zone.PolyGon != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                    if (found)
                    {
                        Console.WriteLine("Inside the polygon");
                    }
                }
            }

            Console.WriteLine($"Total aggregate fetch time from {dataStorebypoint.GetType().Name} in milliseconds " + ((decimal)totalFetchTime / 10));
            return totalFetchTime;
        }
    }
}
