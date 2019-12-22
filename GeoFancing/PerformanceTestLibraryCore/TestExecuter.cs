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
        int _numberOfIteration = 0;

        public TestExecuter(Action<string> progressNotifiaction, int sitesCount, int zonesCount, int numberOfIteration)
        {
            _progressNotifiaction = progressNotifiaction;
            _numberOfIteration = numberOfIteration;
            _sites = Util.CreateSites(sitesCount, zonesCount);
        }

        public Dictionary<DataStoreType, Dictionary<MetricsType, double>> ExecuteTest(Dictionary<string, string> parameters,
                                                                          IEnumerable<DataStoreType> dataStoreTypes)
        {
            var metrics = new Dictionary<DataStoreType, Dictionary<MetricsType, double>>();

            foreach (var dataStoreType in dataStoreTypes)
            {
                switch (dataStoreType)
                {

                    case DataStoreType.InMemory:
                        metrics[DataStoreType.InMemory] = RunTest(new InMemoryCache());
                        break;
                    case DataStoreType.RedisCache:
                        metrics[DataStoreType.RedisCache] = RunTest(new RedisCache(new RedisConnector(parameters["RedisCacheConfig"])));
                        break;
                    case DataStoreType.AzureSql:
                        metrics[DataStoreType.AzureSql] = RunTest(new AzureSql(new SiteDBLayer(parameters["AzureDBConnectionString"])));
                        break;
                    case DataStoreType.FileSystem:
                        metrics[DataStoreType.FileSystem] = RunTest(new FileSystemCache());
                        break;
                    case DataStoreType.Cosmo:
                        IQueryableDataStore cosmoDB = new CosmoDS(parameters["CosmoDatabaseName"],
                            parameters["CosmoCollectionName"],
                            parameters["CosmoEndpointUrl"],
                            parameters["CosmoPrimaryKey"]);

                        metrics[DataStoreType.Cosmo] = RunTest(cosmoDB);
                        break;
                }
            }

            return metrics;
        }

        private Dictionary<MetricsType, double> RunTest(INonQueryableDataStore dataStore)
        {
            _progressNotifiaction($"Storing data for {dataStore.ToString()} ");

            var count = 0;

            foreach (var key in _sites.Keys)
            {
                _progressNotifiaction($"Storing progress count {++count}");
                dataStore.Put(key.ToString(), _sites[key]);
            }

            return FetchDataFromDataStore(dataStore);
        }

        private Dictionary<MetricsType, double> RunTest(IQueryableDataStore dataStore)
        {
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

            for (var count = 1; count < _numberOfIteration; count++)
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
                    zones = queryableDataStore.Get<List<Zone>>(count.ToString(), x, y,width, height, out fetchTime);
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

            metrices[MetricsType.AvgDataFetchTime] = (double)(totalFetchTime / _numberOfIteration);
            metrices[MetricsType.AvgTotalTime] = (double)(totalOverallTime / _numberOfIteration);

            return metrices;
        }
      
    }
}
