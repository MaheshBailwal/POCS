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
    [Route("cosmo")]
    public class Cosmo : Controller
    {

        IDistributedCache _distributedCache;
        RedisCache _redis;
        static bool loaded;
        DocumentClient documentClient;

        string DatabaseName = "CosmoData";
        string CollectionName = "Collection_CosmoData";

        public Cosmo()
        {
            string EndpointUrl = "https://wencocosmodb1.documents.azure.com:443/";
            string PrimaryKey = "ShpsvUeObdx5SXvRKpUSVfnFXkeHJM5R9yJQRG5JK1ZYClJYG66JOdSQkN700Mai51MqBIR2dnod1TKQHHbgFw==";

            documentClient = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);


        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!loaded)
            {
                
                await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
                await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });
                await LoadToCosmo();
            }

            //var stopwatch = new Stopwatch();
            //stopwatch.Restart();
            string result = "";
            for (var count = 1; count < 10; count++)
            {
                DateTime dtstart = DateTime.Now;
                var site = documentClient.CreateDocumentQuery<CosmoSite>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName))
                           .Where(r => r.SiteID == count)
                           .AsEnumerable().FirstOrDefault();
                double timeSpan = DateTime.Now.Subtract(dtstart).TotalMilliseconds;


                CosmoSite _site = (dynamic)site;
                result += Environment.NewLine + "Time to search in  milliseconds " + timeSpan + " [Site Id : " + _site.SiteID + "]"; 

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

        private async Task LoadToCosmo()

        {
            var sites = CreateSites(11, 11);

            foreach (var key in sites.Keys)
            {
                CosmoSite site = sites[key];
                var json = JsonConvert.SerializeObject(site);
                //_redis.Put(key.ToString(), site);
                await CreateSiteDocumentIfNotExists(DatabaseName, CollectionName, site);
            }
            loaded = true;
        }

        private async Task CreateSiteDocumentIfNotExists(string databaseName, string collectionName, CosmoSite site)
        {
            try
            {
                await documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, site.SiteID.ToString()));

            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), site);

                }
                else
                {
                    throw;
                }
            }
        }

        public static Dictionary<int, CosmoSite> CreateSites(int sitesCount, int zonesCount)
        {
            Console.WriteLine($"Creating {sitesCount} sites with {zonesCount} in each");
            var siteCache = new Dictionary<int, CosmoSite>();

            for (var site = 1; site < sitesCount; site++)
            {
                var zones = new List<Zone>();

                for (var zone = 1; zone < zonesCount; zone++)
                {
                    var rectangle = new Rectangle(site * zone * 10, site * zone * 10, 100, 100);
                    var polyGon = new PolyGon(Util.DrawPolygon(rectangle));
                    zones.Add(new Zone() { Rectangle = rectangle, PolyGon = polyGon });
                }

                siteCache[site] = new CosmoSite() { SiteID = site, Zones = zones }; ;
            }

            return siteCache;
        }

      
    }
}
