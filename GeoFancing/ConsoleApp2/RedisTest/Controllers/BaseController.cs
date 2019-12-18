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

        protected string SerachCodrinates(IDataStore dataStore)
        {
            string result = "";
            long totalFetchTime = 0;
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                long fetchTime;
                var site = dataStore.Get<Site>(count.ToString(), ref result, out fetchTime);
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

                result += Environment.NewLine + "Total Time in  milliseconds " + stopwatch.ElapsedMilliseconds;
            }

            return Environment.NewLine + $"Total aggregate fetch time from {dataStore.GetType().Name} in milliseconds " + ((decimal)totalFetchTime / 10);            
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
