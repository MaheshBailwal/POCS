using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("filesys")]
    public class FileSystem : Controller
    {
        static bool loaded;
        FileSystemCache _fileSystemCache;

        public FileSystem(FileSystemCache fileSystemCache)
        {
            _fileSystemCache = fileSystemCache;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!loaded)
            {
                LoadToFileSystem();
            }


            string result = "";
            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();


                var site = _fileSystemCache.Get<Site>(count.ToString(), ref result);

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

            return Ok(result);
        }

        private void LoadToFileSystem()
        {
            var sites = Util.CreateSites();

            foreach (var key in sites.Keys)
            {
                _fileSystemCache.Put(key.ToString(), sites[key]);
            }

            loaded = true;
        }
    }
}
