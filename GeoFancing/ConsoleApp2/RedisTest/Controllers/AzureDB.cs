using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConsoleApp2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RouteGeoFence;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("azuredb")]
    public class azuredb : Controller
    {
        AzureDatabaseAccessLayer siteDB = new AzureDatabaseAccessLayer();
        public azuredb()
        {

        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            string result = "";
            for (var count = 1; count < 10; count++)
            {
                var x = (count * 10 * 100) * (count - 1) + 10 * count;
                var y = (count * 10 * 100) * (count - 1) + 10 * count;

                DateTime dtstart = DateTime.Now;
                Site _site = LoadFromAzureDatabase(count, x, y);
                double timeSpan = DateTime.Now.Subtract(dtstart).TotalMilliseconds;
                result += Environment.NewLine + "Time to search in  milliseconds " + timeSpan + " [Site Id : " + _site.SiteID + "]";
            }

            return Ok(result);
        }

        private Site LoadFromAzureDatabase(int siteID, int X, int Y)
        {
            return siteDB.GetSites(siteID, X, Y);
        }
    }
}
