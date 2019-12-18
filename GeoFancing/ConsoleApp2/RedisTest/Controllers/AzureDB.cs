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
    public class azuredb : BaseController
    {
        //AzureDatabaseAccessLayer siteDB = new AzureDatabaseAccessLayer();

        IDataStorebyPoint _dataStore;
        public azuredb(AzuredatabaseDS azuredatabaseDS)
        {
            _dataStore = azuredatabaseDS;
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = SearchCodrinatesfromDB(_dataStore);
            return Ok(result);                       
        }
        
    }
}
