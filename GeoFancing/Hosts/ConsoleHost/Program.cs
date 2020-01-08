using PerformanceTestLibrary;
using PerformanceTestLibrary.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                Dictionary<string, string> parameters = GetParameters();

                var testExecuter = new TestExecuter(ProgressNotifiactionHandler,
                 int.Parse(ConfigurationManager.AppSettings["NumberOfSites"]),
                  int.Parse(ConfigurationManager.AppSettings["NumberOfZones"]),
                  int.Parse(ConfigurationManager.AppSettings["NumberOfIteration"]), parameters);

                var response = testExecuter.ExecuteTest(ConfigurationManager.AppSettings["TestToRun"]);

                SendEmail(parameters, testExecuter, response);
                PrintResult(response);
                Console.WriteLine("Done");

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            Console.ReadLine();
        }

        private static Dictionary<string, string> GetParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["RedisCacheConfig"] = ConfigurationManager.AppSettings["RedisCacheConfig"];
            parameters["CosmoDatabaseName"] = ConfigurationManager.AppSettings["CosmoDatabaseName"];
            parameters["CosmoCollectionName"] = ConfigurationManager.AppSettings["CosmoCollectionName"];
            parameters["CosmoEndpointUrl"] = ConfigurationManager.AppSettings["CosmoEndpointUrl"];
            parameters["CosmoPrimaryKey"] = ConfigurationManager.AppSettings["CosmoPrimaryKey"];
            parameters["AzureDBConnectionString"] = ConfigurationManager.AppSettings["AzureDBConnectionString"];
            parameters["ContainerName"] = ConfigurationManager.AppSettings["ContainerName"];
            parameters["StorageConnectionstring"] = ConfigurationManager.AppSettings["StorageConnectionstring"];
            parameters["ToEmails"] = ConfigurationManager.AppSettings["ToEmails"];

            // Data store flags
            parameters["DoNotPushDataToStores"] = ConfigurationManager.AppSettings["DoNotPushDataToStores"];
       
            //Ping response check flag and keys
            parameters["PingToRedisWithPort"] = ConfigurationManager.AppSettings["PingToRedisWithPort"];
            parameters["PingToCosmoWithPort"] = ConfigurationManager.AppSettings["PingToCosmoWithPort"];
            parameters["PingToSqlWithPort"] = ConfigurationManager.AppSettings["PingToSqlWithPort"];
            parameters["PingToStorageWithPort"] = ConfigurationManager.AppSettings["PingToStorageWithPort"];
            parameters["IsPingNeedToCheck"] = ConfigurationManager.AppSettings["IsPingNeedToCheck"];
            parameters["FileSystemDataFolder"] = "";
            return parameters;
        }

        private static void SendEmail(Dictionary<string, string> parameters, TestExecuter testExecuter, Dictionary<DataStoreType, Dictionary<MetricsType, double>> response)
        {
            Email email = new Email();
            var statsInfo = $"<div><b>Data Information</b></div><div>NumberOfSites : {ConfigurationManager.AppSettings["NumberOfSites"]}" +
                $" NumberOfZones: {ConfigurationManager.AppSettings["NumberOfZones"]}" +
                $" NumberOfFetchIteration :{ConfigurationManager.AppSettings["NumberOfIteration"]} </div>";

            email.SendEmailWithMetricsAsync(response,
                "<br></br><br><b><I>Performace Test Excuted  on Azure VM With Below Configuration App </b></I>" + SystemConfiguration.FetchSystemConfiguration() +
                testExecuter.GetDataInfo(), parameters["ToEmails"]);
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
