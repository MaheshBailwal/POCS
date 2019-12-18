using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp2;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("mem")]
    public class InMemory : BaseController
    {
        static bool loaded;
        IDataStore _dataStore;

        public InMemory(InMemoryCache inMemoryCache)
        {
            _dataStore = inMemoryCache;

            if (!loaded)
            {
                LoadData(_dataStore);
            }
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = SearchCodrinates(_dataStore);

            return Ok(result);
        }
    }
}
