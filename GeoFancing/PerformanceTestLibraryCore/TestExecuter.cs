using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTestLibrary
{
    public class TestExecuter
    {
        Dictionary<int, Site> _sites;
        Action<string> _progressNotifiaction;
        public static double TotalTimeInSeconds;

        public TestExecuter(Action<string> progressNotifiaction, int sitesCount, int zonesCount, int numberOfIteration)
        {
            _progressNotifiaction = progressNotifiaction;
            NumberOfIteration = numberOfIteration;
            _sites = Util.CreateSites(sitesCount, zonesCount);
            SitesCount = sitesCount;
            ZonesCount = zonesCount;
        }

        public int SitesCount { get; private set; }
        public int ZonesCount { get; private set; }
        public int NumberOfIteration { get; private set; }

        public Dictionary<DataStoreType, Dictionary<MetricsType, double>> ExecuteTest(Dictionary<string, string> parameters, IEnumerable<DataStoreType> dataStoreTypes)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var metrics = new Dictionary<DataStoreType, Dictionary<MetricsType, double>>();

            foreach (var dataStoreType in dataStoreTypes)
            {
                switch (dataStoreType)
                {

                    case DataStoreType.InMemory:
                        metrics[DataStoreType.InMemory] = RunTest(new InMemoryCache(), Convert.ToBoolean(parameters["IsDataNeedToStoreInMemory"]));
                        break;
                    case DataStoreType.RedisCache:
                        metrics[DataStoreType.RedisCache] = RunTest(new RedisCache(new RedisConnector(parameters["RedisCacheConfig"])), Convert.ToBoolean(parameters["IsDataNeedToStoreInRedis"]));
                        break;
                    case DataStoreType.AzureSql:
                        metrics[DataStoreType.AzureSql] = RunTest(new AzureSql(parameters["AzureDBConnectionString"]), Convert.ToBoolean(parameters["IsDataNeedToStoreInSQL"]));
                        break;
                    case DataStoreType.FileSystem:
                        metrics[DataStoreType.FileSystem] = RunTest(new FileSystemCache(), Convert.ToBoolean(parameters["IsDataNeedToStoreInFileSystem"]));
                        break;
                    case DataStoreType.Cosmo:
                        IQueryableDataStore cosmoDB = new CosmoDS(parameters["CosmoDatabaseName"],
                            parameters["CosmoCollectionName"],
                            parameters["CosmoEndpointUrl"],
                            parameters["CosmoPrimaryKey"]);

                        metrics[DataStoreType.Cosmo] = RunTest(cosmoDB, Convert.ToBoolean(parameters["IsDataNeedToStoreInCosmos"]));
                        break;
                    case DataStoreType.BlobStorage:
                        metrics[DataStoreType.BlobStorage] = RunTest(new BlobDS(parameters["ContainerName"], parameters["StorageConnectionstring"]), Convert.ToBoolean(parameters["IsDataNeedToStoreInBlob"]));
                        break;
                }
            }
            stopwatch.Stop();
            TotalTimeInSeconds = stopwatch.Elapsed.TotalMinutes;
            return metrics;
        }

        private Dictionary<MetricsType, double> RunTest(IDataStore dataStore, bool isDataNeedToStore)
        {
            if (isDataNeedToStore)
            {
                _progressNotifiaction($"Storing data for {dataStore.ToString()} ");
                var count = 0;

                Parallel.ForEach(_sites.Keys, (key) =>
                {
                    dataStore.Put(key.ToString(), _sites[key]);
                    _progressNotifiaction($"Storing progress count {++count} : {key}");
                });
            }

            return FetchDataFromDataStore(dataStore);
        }

        private Dictionary<MetricsType, double> FetchDataFromDataStore(IDataStore dataStore)
        {
            double totalFetchTime = 0;
            double totalOverallTime = 0;
            var metrices = new Dictionary<MetricsType, double>();

            bool isQueryableDataStore = dataStore is IQueryableDataStore;
            IEnumerable<Zone> zones = null;

            _progressNotifiaction($"Fetching Data From :{dataStore}");

            for (var count = 1; count < NumberOfIteration; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;
                int width = 100;
                int height = 100;

                double fetchTime;

                if (isQueryableDataStore)
                {
                    var queryableDataStore = (IQueryableDataStore)dataStore;
                    zones = queryableDataStore.Get<List<Zone>>(count.ToString(), x, y, width, height, out fetchTime);
                }
                else
                {
                    var queryableDataStore = (INonQueryableDataStore)dataStore;
                    var site = queryableDataStore.Get<Site>(count.ToString(), out fetchTime);
                    zones = site.Zones.FindAll(z => z.Rectangle.Contains(x, y));
                }

                totalFetchTime += fetchTime;
                stopwatch.Stop();
                totalOverallTime += stopwatch.Elapsed.TotalMilliseconds;
            }

            metrices[MetricsType.AvgDataFetchTime] = (double)(totalFetchTime / NumberOfIteration);
            metrices[MetricsType.AvgTotalTime] = (double)(totalOverallTime / NumberOfIteration);

            return metrices;
        }

        public string GetDataInfo()
        {
            return $"<div><u>Data Information</u></div><div><i><u>NumberOfSites</i></u> : {SitesCount}" +
             $"  <i><u>NumberOfZones</i></u>: {ZonesCount}" +
             $"  <i><u>NumberOfFetchIteration</i></u> :{NumberOfIteration} </div>";
        }
    }
}
