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
                    case DataStoreType.FileSystem:
                        IDataStore fileSystemDataStore = new FileSystemCache();
                        metrics[DataStoreType.FileSystem] = RunTest(fileSystemDataStore);
                        break;

                    case DataStoreType.Cosmo:
                        IDataStore cosmoDB = new CosmoDS(parameters["CosmoDatabaseName"],
                            parameters["CosmoCollectionName"],
                            parameters["CosmoEndpointUrl"],
                            parameters["CosmoPrimaryKey"]);

                        metrics[DataStoreType.Cosmo] = RunTest(cosmoDB);

                        break;
                }
            }

            return metrics;
        }

        private Dictionary<MetricsType, double> RunTest(IDataStore dataStore)
        {
            _progressNotifiaction($"Storing data for {dataStore.ToString()} ");

            var count = 0;

            foreach (var key in _sites.Keys)
            {
                _progressNotifiaction($"Storing progress count {++count}");
                dataStore.Put(key.ToString(), _sites[key]);
            }

            //Parallel.ForEach(_sites.Keys, (key) =>
            //    {
            //        _progressNotifiaction($"Storing progress count {++count}");
            //        dataStore.Put(key.ToString(), _sites[key]);
            //    });


            return FetchDataFromDataStore(dataStore);
        }


        private  Dictionary<MetricsType, double> FetchDataFromDataStore(IDataStore dataStore)
        {
            double totalFetchTime = 0;
            double totalOverallTime = 0;
            int totalFetchSites = _numberOfIteration;
            var metrices = new Dictionary<MetricsType, double>();

            for (var count = 1; count < _numberOfIteration; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                double fetchTime;
                var site = dataStore.Get<Site>(count.ToString(), out fetchTime);
                totalFetchTime += fetchTime;

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault(z => z.Rectangle.Contains(x, y));

                //if yes then then find whether corodiante exist in polygon
                if (zone != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                }

                stopwatch.Stop();

                totalOverallTime += stopwatch.Elapsed.TotalMilliseconds;

            }

            metrices[MetricsType.AvgDataFetchTime] = (double)(totalFetchTime / _numberOfIteration);

            metrices[MetricsType.AvgTotalTime] = (double)(totalOverallTime / _numberOfIteration);

            return metrices;
        }

        private double FetchDataFromAzureDB(IDataStorebyPoint dataStorebypoint)
        {
            double totalFetchTime = 0;
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                double fetchTime;
                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;
                var site = dataStorebypoint.Get<Site>(count.ToString(), x, y, out fetchTime);
                totalFetchTime += fetchTime;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault();

                //if yes then then find whether corodiante exist in polygon
                if (zone != null && zone.PolyGon != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                    if (found)
                    {
                        Console.WriteLine("Inside the polygon");
                    }
                }
            }

            Console.WriteLine($"Total aggregate fetch time from {dataStorebypoint.GetType().Name} in milliseconds " + ((decimal)totalFetchTime / 10));
            return totalFetchTime;
        }
    }
}
