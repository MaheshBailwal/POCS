﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("redis")]
    public class Redis : BaseController
    {

        static bool loaded;
        INonQueryableDataStore _dataStore;

        public Redis(RedisCache redis)
        {
            _dataStore = redis;

            if (!loaded)
            {
                LoadData(_dataStore);
            }
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = SearchCordinates(_dataStore);
            return Ok(result);
            
        }
    }
}
