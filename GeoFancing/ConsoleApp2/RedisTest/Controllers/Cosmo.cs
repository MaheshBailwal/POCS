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
    public class Cosmo : BaseController
    {
        IDataStore _dataStore;

        static bool loaded;        

        public Cosmo(CosmoDS cosmoDS )
        {
            _dataStore = cosmoDS;

            if (!loaded)
            {
                //LoadData(_dataStore);
                loaded = true;
            }
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = SerachCodrinates(_dataStore);
            return Ok(result);            
        }        
    }
}
