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
        static bool loaded;
        AzureDatabaseAccessLayer siteDB = new AzureDatabaseAccessLayer();

        public azuredb()
        {            
            
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!loaded)
            {

                //await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
                //await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });
                //await LoadFromAzureDatabase();
            }

            //var stopwatch = new Stopwatch();
            //stopwatch.Restart();
            string result = "";
            for (var count = 1; count < 10; count++)
            {
                DateTime dtstart = DateTime.Now;
                //var site = documentClient.CreateDocumentQuery<CosmoSite>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName))
                           //.Where(r => r.SiteID == count)
                          // .AsEnumerable().FirstOrDefault();
                double timeSpan = DateTime.Now.Subtract(dtstart).TotalMilliseconds;

                Site site = new Site();

                Site _site = (dynamic)site;
                result += Environment.NewLine + "Time to search in  milliseconds " + timeSpan + " [Site Id : " + _site.ID + "]";

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;
                Console.WriteLine(x + ":" + y);
                //check corodinates exist in rectangle
                var zone = _site.Zones.FirstOrDefault(z => z.Rectangle.Contains(x, y));

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

            return Ok(result);
        }

        private List<Site> LoadFromAzureDatabase(int siteID)

        {
            return siteDB.GetSites();
        }        

        //public static Dictionary<int, CosmoSite> FillPolygon(List<Site> lsites)
        //{
        //    //Console.WriteLine($"Creating {sitesCount} sites with {zonesCount} in each");
        //    var siteCache = new Dictionary<int, CosmoSite>();

        //    for (var site = 1; site < lsites.Count; site++)
        //    {
        //        var zones = new List<Zone>();

        //        for (var zone = 1; zone < lsites[site].Zones.Count; zone++)
        //        {
        //            var rectangle = new Rectangle(site * zone * 10, site * zone * 10, 100, 100);
        //            var polyGon = new PolyGon(Util.DrawPolygon(rectangle));
        //            zones.Add(new Zone() { Rectangle = rectangle, PolyGon = polyGon });
        //        }

        //        siteCache[site] = new CosmoSite() { SiteID = site, Zones = zones }; ;
        //    }

        //    return siteCache;
        //}


    }
}
