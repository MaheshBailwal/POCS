using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using PerformanceTestLibrary;
using PerformanceTestLibrary.Services;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FunctionAppHost
{
    public static class TestEexecutionFunction
    {
        static bool running;
        static StringBuilder sb = new StringBuilder();
        static ExecutionContext _context;


        [FunctionName("RunTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            _context = context;

            if (sb.Length < 1)
            {
                running = true;
                sb.AppendLine("Processing Started.Keep on refersing page to see progress..");
                Task.Run(() => StartTest());
            }

            if (running)
            {
                return (ActionResult)new OkObjectResult(sb.ToString());
            }

            return new ContentResult { Content = sb.ToString(), ContentType = "text/html" };
        }

        [FunctionName("Clear")]
        public static async Task<IActionResult> Clear(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
           ILogger log, ExecutionContext context)
        {
            sb.Clear();
            return (ActionResult)new OkObjectResult("cleared");
        }

        private static Dictionary<string, string> GetParameters()
        {
            sb.AppendLine("in 1");
            var config = new ConfigurationBuilder()
           .SetBasePath(_context.FunctionAppDirectory)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

            sb.AppendLine("in 2");


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            sb.AppendLine("in 3");
            parameters["RedisCacheConfig"] = GetSetting("RedisCacheConfig");

            sb.AppendLine("Redis:" + parameters["RedisCacheConfig"]);

            sb.AppendLine("in 4");
            parameters["CosmoDatabaseName"] = GetSetting("CosmoDatabaseName");
            parameters["CosmoCollectionName"] = GetSetting("CosmoCollectionName");
            parameters["CosmoEndpointUrl"] = GetSetting("CosmoEndpointUrl");
            sb.AppendLine("in 5");
            parameters["CosmoPrimaryKey"] = GetSetting("CosmoPrimaryKey");
            parameters["AzureDBConnectionString"] = GetSetting("AzureDBConnectionString");
            parameters["ContainerName"] = GetSetting("ContainerName");
            parameters["StorageConnectionstring"] = GetSetting("StorageConnectionstring");
            parameters["ToEmails"] = GetSetting("ToEmails");

            parameters["NumberOfSites"] = GetSetting("NumberOfSites");
            parameters["NumberOfZones"] = GetSetting("NumberOfZones");
            parameters["NumberOfIteration"] = GetSetting("NumberOfIteration");
            parameters["TestToRun"] = GetSetting("TestToRun");
            sb.AppendLine("in 6");
            parameters["DoNotPushDataToStores"] = GetSetting("DoNotPushDataToStores");
            parameters["FileSystemDataFolder"] = _context.FunctionAppDirectory;
            

            sb.AppendLine("in 7");
            return parameters;
        }

        private static void StartTest()
        {
            try
            {

                var parameters = GetParameters();

                sb.AppendLine("in 8");

                var testExecuter = new TestExecuter(ProgressNotifiactionHandler, int.Parse(parameters["NumberOfSites"]),
                  int.Parse(parameters["NumberOfZones"]),
                  int.Parse(parameters["NumberOfIteration"]), parameters);

                sb.AppendLine("in 9");

                var response = testExecuter.ExecuteTest(parameters["TestToRun"]);

                Email email = new Email();
                var html = email.SendEmailWithMetricsAsync(response,
                    "<br><b><I>Performace Test Excuted  on Azure Function App </b></I>"
                    + testExecuter.GetDataInfo() + "", parameters["ToEmails"]);

                sb.Clear();
                sb.Append(html);
            }
            catch (Exception ex)
            {
                sb.Append(ex);
            }
            finally
            {
                running = false;
            }
        }

        private static void ProgressNotifiactionHandler(string message)
        {
            sb.AppendLine(message);
        }

        private static string GetSetting(string settingName)
        {
            var value = Environment.GetEnvironmentVariable(settingName);
        
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var file = Path.Combine(_context.FunctionAppDirectory,"settings.json");

            var content = File.ReadAllText(file);

            var jobject = JObject.Parse(content);

            var configEnrtries = jobject.Children().Values().ToList()[1].ToList();

            foreach (JProperty entry in configEnrtries)
            {
                if (entry.Name == settingName)
                {

                    return entry.Value.ToString();
                }
            }

            return value;
        }

    }
}
