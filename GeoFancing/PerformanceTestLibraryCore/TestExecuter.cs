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
        Dictionary<string, string> _parameters;

        public TestExecuter(Action<string> progressNotifiaction, int sitesCount, int zonesCount, int numberOfIteration, Dictionary<string, string> parameters)
        {
            _progressNotifiaction = progressNotifiaction;
            NumberOfIteration = numberOfIteration;
            _sites = Util.CreateSites(sitesCount, zonesCount);
            SitesCount = sitesCount;
            ZonesCount = zonesCount;
            _parameters = parameters;
        }

        public int SitesCount { get; private set; }
        public int ZonesCount { get; private set; }
        public int NumberOfIteration { get; private set; }

        public Dictionary<DataStoreType, Dictionary<MetricsType, double>> ExecuteTest(IEnumerable<DataStoreType> dataStoreTypes)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var metrics = new Dictionary<DataStoreType, Dictionary<MetricsType, double>>();

            foreach (var dataStoreType in dataStoreTypes)
            {
                switch (dataStoreType)
                {

                    case DataStoreType.InMemory:
                        metrics[DataStoreType.InMemory] = RunTest(new InMemoryCache());
                        break;
                    case DataStoreType.RedisCache:
                        metrics[DataStoreType.RedisCache] = RunTest(new RedisCache(new RedisConnector(_parameters["RedisCacheConfig"])));
                        break;
                    case DataStoreType.AzureSql:
                        metrics[DataStoreType.AzureSql] = RunTest(new AzureSql(_parameters["AzureDBConnectionString"]));
                        break;
                    case DataStoreType.FileSystem:
                        metrics[DataStoreType.FileSystem] = RunTest(new FileSystemCache(_parameters["FileSystemDataFolder"]));
                        break;
                    case DataStoreType.Cosmo:
                        IQueryableDataStore cosmoDB = new CosmoDS(_parameters["CosmoDatabaseName"],
                            _parameters["CosmoCollectionName"],
                            _parameters["CosmoEndpointUrl"],
                            _parameters["CosmoPrimaryKey"]);

                        metrics[DataStoreType.Cosmo] = RunTest(cosmoDB);
                        break;
                    case DataStoreType.BlobStorage:
                        metrics[DataStoreType.BlobStorage] = RunTest(new BlobDS(_parameters["ContainerName"], _parameters["StorageConnectionstring"]));
                        break;
                }
            }
            stopwatch.Stop();
            TotalTimeInSeconds = stopwatch.Elapsed.TotalMinutes;
            return metrics;
        }

        public Dictionary<DataStoreType, Dictionary<MetricsType, double>> ExecuteTest(string dataStoreTypes)
        {
            return ExecuteTest(StringToDataStoreType(dataStoreTypes));
        }

        private IEnumerable<DataStoreType> StringToDataStoreType(string commaSeperatedDataStoreType)
        {
            var arr = commaSeperatedDataStoreType.Split(',');
            var dataStoreTypes = new List<DataStoreType>();

            foreach (var item in arr)
            {
                dataStoreTypes.Add((DataStoreType)Enum.Parse(typeof(DataStoreType), item));
            }

            return dataStoreTypes;
        }

        private Dictionary<MetricsType, double> RunTest(IDataStore dataStore )
        {
            bool doNotPushDataToStore = StringToDataStoreType(_parameters["DoNotPushDataToStores"]).Contains(dataStore.DataStoreType);


            if (!doNotPushDataToStore)
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
