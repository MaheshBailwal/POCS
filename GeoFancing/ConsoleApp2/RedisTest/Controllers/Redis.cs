﻿using System;
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
    public class Redis : BaseController
    {

        static bool loaded;
        IDataStore _dataStore;

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
            var result = SerachCodrinates(_dataStore);
            return Ok(result);
            
        }
    }
}
