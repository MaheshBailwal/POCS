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

namespace FunctionAppHost
{
    public static class TestEexecutionFunction
    {
        static bool running;
        static StringBuilder sb = new StringBuilder();

        [FunctionName("RunTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (sb.Length < 1)
            {
                running = true;
                sb.AppendLine("Processing Started.Keep on refersing page to see progress..");
                Task.Run(() => StartTest(context));
            }

            if (running)
            {
                return (ActionResult)new OkObjectResult(sb.ToString());
            }

            return new ContentResult { Content = sb.ToString(), ContentType = "text/html" };
        }

        [FunctionName("Clear")]
        public static async Task<IActionResult> Clear(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
           ILogger log, ExecutionContext context)
        {
            sb.Clear();
            return (ActionResult)new OkObjectResult("cleared");
        }

        private static Dictionary<string, string> GetParameters(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(context.FunctionAppDirectory)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["RedisCacheConfig"] = config["RedisCacheConfig"];
            parameters["CosmoDatabaseName"] = config["CosmoDatabaseName"];
            parameters["CosmoCollectionName"] = config["CosmoCollectionName"];
            parameters["CosmoEndpointUrl"] = config["CosmoEndpointUrl"];
            parameters["CosmoPrimaryKey"] = config["CosmoPrimaryKey"];
            parameters["AzureDBConnectionString"] = config["AzureDBConnectionString"];
            parameters["ContainerName"] = config["ContainerName"];
            parameters["StorageConnectionstring"] = config["StorageConnectionstring"];
            parameters["ToEmails"] = config["ToEmails"];

            parameters["NumberOfSites"] = config["NumberOfSites"];
            parameters["NumberOfZones"] = config["NumberOfZones"];
            parameters["NumberOfIteration"] = config["NumberOfIteration"];
            parameters["TestToRun"] = config["TestToRun"];

            parameters["DoNotPushDataToStores"] = config["DoNotPushDataToStores"];

            return parameters;
        }

        private static void StartTest(ExecutionContext context)
        {
            try
            {
                var parameters = GetParameters(context);
                var testExecuter = new TestExecuter(ProgressNotifiactionHandler, int.Parse(parameters["NumberOfSites"]),
                  int.Parse(parameters["NumberOfZones"]),
                  int.Parse(parameters["NumberOfIteration"]), parameters);

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
    }
}
