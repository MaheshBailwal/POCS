using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("redis")]
    public class Redis : Controller
    {

        IDistributedCache _distributedCache;
        RedisCache _redis;
        static bool loaded;

        public Redis(RedisCache redis)
        {
            _redis = redis;
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(!loaded)
            {
                LoadToRdis();
            }


            string result = "";
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var site = _redis.Get<Site>(count.ToString());

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

                stopwatch.Stop();
                result += Environment.NewLine + "Time to serach in  milliseconds " + stopwatch.ElapsedMilliseconds;
            }

            return Ok(result);
        }

        private void LoadToRdis()
        {
            var sites = Util.CreateSites(100,100);

            foreach (var key in sites.Keys)
            {
                var site = sites[key];
                var json = JsonConvert.SerializeObject(site);
                _redis.Put(key.ToString(), site);
            }
            loaded = true;
        }
    }
}
