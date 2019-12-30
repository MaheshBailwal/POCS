using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;
using PerformanceTestLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisTest.Controllers
{
    [Route("all")]
    public class RunAll : BaseController
    {
        private readonly FileSystemCache _fileSystemCache;
        private readonly InMemoryCache _inMemoryCache;
        private readonly RedisCache _redis;
        private readonly AzureSql _azureDB;
        private readonly CosmoDS _cosmoDb;
        static bool running;
        AppSettings _appSettings;
        static StringBuilder sb = new StringBuilder();

        public RunAll(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (sb.Length < 1)
            {
                running = true;
                sb.AppendLine("Processing Started.Keep on refersing page to see progress..");
                Task.Run(() => StartTest());
            }

            if (running)
            {
                return Ok(sb.ToString());
            }

            return Content(sb.ToString(), "text/html", Encoding.UTF8);

        }

        [HttpGet("clear")]
        public async Task<IActionResult> Clear()
        {
            sb.Clear();
            return Ok("cleared");
        }

        private void StartTest()
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RedisCacheConfig"] = _appSettings.RedisCacheConfig;
                parameters["CosmoDatabaseName"] = _appSettings.CosmoDatabaseName;
                parameters["CosmoCollectionName"] = _appSettings.CosmoCollectionName;
                parameters["CosmoEndpointUrl"] = _appSettings.CosmoEndpointUrl;
                parameters["CosmoPrimaryKey"] = _appSettings.CosmoPrimaryKey;
                parameters["AzureDBConnectionString"] = _appSettings.AzureDBConnectionString;
                parameters["StorageConnectionstring"] = _appSettings.StorageConnectionstring;
                parameters["ContainerName"] = _appSettings.ContainerName;
                parameters["ToEmails"] = _appSettings.ToEmails;

                var testExecuter = new TestExecuter(ProgressNotifiactionHandler,int.Parse( _appSettings.NumberOfSites), int.Parse(_appSettings.NumberOfZones), int.Parse(_appSettings.NumberOfIteration));

                var response = testExecuter.ExecuteTest(parameters, new[] { DataStoreType.InMemory,
                                                                DataStoreType.Cosmo,
                                                                DataStoreType.FileSystem,
                                                                DataStoreType.AzureSql,
                                                                DataStoreType.RedisCache,
                DataStoreType.BlobStorage});

                Email email = new Email();
                var html = email.SendEmailWithMetricsAsync(response, "<br><b><I>Performace Test Excuted  on Azure Web App </b></I>" + testExecuter.GetDataInfo() + "", parameters["ToEmails"]);
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


            //  PrintResult(response);

        }

        private void ProgressNotifiactionHandler(string message)
        {
            sb.AppendLine(message);
        }
        private void PrintResult(Dictionary<DataStoreType, Dictionary<MetricsType, double>> response)
        {
            foreach (var dataStoreType in response.Keys)
            {
                var metreics = response[dataStoreType];

                sb.AppendLine($"{Environment.NewLine} Metrics for datastor {dataStoreType.ToString()}");

                foreach (var metricsType in metreics.Keys)
                {
                    sb.AppendLine($" {metricsType.ToString()} : time in ms { metreics[metricsType]} ");
                }
            }

        }
    }
}
