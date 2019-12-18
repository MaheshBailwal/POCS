using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    public class BaseController : Controller
    {
        static bool loaded;
        IDataStore _dataStore;
        IDataStorebyPoint _dataStorebyPoint;

        protected string SearchCordinates(IDataStore dataStore)
        {            
            long totalFetchTime = 0;
            int totalFetchSites = 100;
            for (var count = 1; count < totalFetchSites; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                long fetchTime;
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
                    if (found)
                    {
                        Console.WriteLine("Inside the polygon");
                    }
                }                
            }

            return Environment.NewLine + $"Total aggregate fetch time from {dataStore.GetType().Name} in milliseconds " + ((decimal)totalFetchTime / totalFetchSites);            
        }

        protected string SearchCodrinatesfromDB(IDataStorebyPoint dataStorebypoint)
        {
            long totalFetchTime = 0;            
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                long fetchTime;
                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;
                var site = dataStorebypoint.Get<Site>(count.ToString(),x,y, out fetchTime);
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

            return Environment.NewLine + $"Total aggregate fetch time from {dataStorebypoint.GetType().Name} in milliseconds " + ((decimal)totalFetchTime / 10);
        }

        protected void LoadData(IDataStore dataStore)
        {
            var sites = Util.CreateSites();

            foreach (var key in sites.Keys)
            {
                dataStore.Put(key.ToString(), sites[key]);
            }

        }
    }
}
