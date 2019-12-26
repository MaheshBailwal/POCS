using PerformanceTestLibrary;
using PerformanceTestLibrary.Services;
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
            try
            {
                FetchSystemConfiguration();

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RedisCacheConfig"] = ConfigurationManager.AppSettings["RedisCacheConfig"];
                parameters["CosmoDatabaseName"] = ConfigurationManager.AppSettings["CosmoDatabaseName"];
                parameters["CosmoCollectionName"] = ConfigurationManager.AppSettings["CosmoCollectionName"];
                parameters["CosmoEndpointUrl"] = ConfigurationManager.AppSettings["CosmoEndpointUrl"];
                parameters["CosmoPrimaryKey"] = ConfigurationManager.AppSettings["CosmoPrimaryKey"];
                parameters["AzureDBConnectionString"] = ConfigurationManager.AppSettings["AzureDBConnectionString"];
                parameters["ContainerName"] = ConfigurationManager.AppSettings["ContainerName"];
                parameters["StorageConnectionstring"] = ConfigurationManager.AppSettings["StorageConnectionstring"];

                var testExecuter = new TestExecuter(ProgressNotifiactionHandler,
                   int.Parse(ConfigurationManager.AppSettings["NumberOfSites"]),
                    int.Parse(ConfigurationManager.AppSettings["NumberOfZones"]),
                    int.Parse(ConfigurationManager.AppSettings["NumberOfIteration"]));

                var response = testExecuter.ExecuteTest(parameters, new[] { DataStoreType.InMemory,
                                                                DataStoreType.Cosmo,
                                                                DataStoreType.FileSystem,
                                                                DataStoreType.AzureSql,
                                                                DataStoreType.RedisCache,
            DataStoreType.BlobStorage});

                Email email = new Email();
                email.SendEmailWithMetricsAsync(response);
                PrintResult(response);
                Console.WriteLine("Done");

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
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

        static void FetchSystemConfiguration()
        {
            
            int lPathindex = System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf("\\");

            string batchfilepath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Executepowershell.bat");
            string ss = "powershell.exe " + Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\PerformTest.ps1") + " " +
                Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Systemconfig.txt");

            StreamWriter streamWriter = new StreamWriter(batchfilepath);
            streamWriter.Write(ss);
            streamWriter.Close();
            System.Diagnostics.Process.Start(batchfilepath).WaitForExit();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("********************************************************************");
            Console.WriteLine("Performance Test executed on below system configurations: ");
            Console.WriteLine(File.ReadAllText(Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Systemconfig.txt")).ToString()); ;
            Console.WriteLine("********************************************************************");
            Console.ResetColor();
        }
    }
}
