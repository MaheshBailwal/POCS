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
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["RedisCacheConfig"] = ConfigurationManager.AppSettings["RedisCacheConfig"];
            parameters["CosmoDatabaseName"] = ConfigurationManager.AppSettings["CosmoDatabaseName"];
            parameters["CosmoCollectionName"] = ConfigurationManager.AppSettings["CosmoCollectionName"];
            parameters["CosmoEndpointUrl"] = ConfigurationManager.AppSettings["CosmoEndpointUrl"];
            parameters["CosmoPrimaryKey"] = ConfigurationManager.AppSettings["CosmoPrimaryKey"];
            parameters["AzureDBConnectionString"] = ConfigurationManager.AppSettings["AzureDBConnectionString"];

            var testExecuter = new TestExecuter(ProgressNotifiactionHandler, 3, 3, 3);

            var response = testExecuter.ExecuteTest(parameters, new[] { DataStoreType.InMemory,
                                                                DataStoreType.Cosmo,
                                                                DataStoreType.FileSystem,
                                                                DataStoreType.AzureSql,
                                                                DataStoreType.RedisCache });

            PrintResult(response);
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void ProgressNotifiactionHandler(string message)
        {
            Console.WriteLine(message);
        }

        static void PrintResult(Dictionary<DataStoreType, Dictionary<MetricsType, double>> response)
        {
            foreach (var dataStoreType in response.Keys)
            {
                var metreics = response[dataStoreType];

                Console.WriteLine($"{Environment.NewLine}Metrics for datastor {dataStoreType.ToString()}");

                foreach (var metricsType in metreics.Keys)
                {
                    Console.WriteLine($" {metricsType.ToString()} : time in ms { metreics[metricsType]} ");
                }
            }

        }
    }
}