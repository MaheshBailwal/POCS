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
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["RedisCacheConfig"] = GetSetting("RedisCacheConfig");
            parameters["CosmoDatabaseName"] = GetSetting("CosmoDatabaseName");
            parameters["CosmoCollectionName"] = GetSetting("CosmoCollectionName");
            parameters["CosmoEndpointUrl"] = GetSetting("CosmoEndpointUrl");
            parameters["CosmoPrimaryKey"] = GetSetting("CosmoPrimaryKey");
            parameters["AzureDBConnectionString"] = GetSetting("AzureDBConnectionString");
            parameters["ContainerName"] = GetSetting("ContainerName");
            parameters["StorageConnectionstring"] = GetSetting("StorageConnectionstring");
            parameters["ToEmails"] = GetSetting("ToEmails");
            parameters["NumberOfSites"] = GetSetting("NumberOfSites");
            parameters["NumberOfZones"] = GetSetting("NumberOfZones");
            parameters["NumberOfIteration"] = GetSetting("NumberOfIteration");
            parameters["TestToRun"] = GetSetting("TestToRun");
            parameters["DoNotPushDataToStores"] = GetSetting("DoNotPushDataToStores");
            parameters["FileSystemDataFolder"] = _context.FunctionAppDirectory;
            return parameters;
        }

        private static void StartTest()
        {
            try
            {
                var parameters = GetParameters();
                var testExecuter = new TestExecuter(ProgressNotifiactionHandler, int.Parse(parameters["NumberOfSites"]),
                  int.Parse(parameters["NumberOfZones"]),
                  int.Parse(parameters["NumberOfIteration"]), parameters);

                var response = testExecuter.ExecuteTest(parameters["TestToRun"]);

                var email = new Email();
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
