using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;
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
        static bool loaded;
        AppSettings _appSettings;
        StringBuilder sb = new StringBuilder();

        public RunAll(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
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

            var testExecuter = new TestExecuter(ProgressNotifiactionHandler, 3, 3, 3);

            var response = testExecuter.ExecuteTest(parameters, new[] { DataStoreType.InMemory,
                                                                DataStoreType.Cosmo,
                                                                DataStoreType.FileSystem,
                                                                DataStoreType.AzureSql,
                                                                DataStoreType.RedisCache });

            PrintResult(response);
            return Ok(sb.ToString());
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
