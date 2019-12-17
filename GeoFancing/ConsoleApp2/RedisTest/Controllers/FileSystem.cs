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
    public class FileSystem : BaseController
    {
        static bool loaded;
        IDataStore _dataStore;

        public FileSystem(FileSystemCache fileSystemCache)
        {
            _dataStore = fileSystemCache;

            if(!loaded)
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
