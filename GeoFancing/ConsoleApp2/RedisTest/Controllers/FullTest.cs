using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;
using System.Threading.Tasks;

namespace RedisTest.Controllers
{
    [Route("full")]
    public class FullTest : BaseController
    {        
        private readonly FileSystemCache _fileSystemCache;
        private readonly InMemoryCache _inMemoryCache;
        private readonly RedisCache _redis;
        private readonly AzuredatabaseDS _azureDB;
        private readonly CosmoDS _cosmoDb;
        static bool loaded;

        public FullTest(FileSystemCache fileSystemCache, InMemoryCache inMemoryCache, RedisCache redis, AzuredatabaseDS azureDB, CosmoDS cosmoDb)
        {
            _fileSystemCache = fileSystemCache;
            _inMemoryCache = inMemoryCache;
            _redis = redis;
            _azureDB = azureDB;
            _cosmoDb = cosmoDb;
            if (!loaded)
            {
                LoadData(_fileSystemCache);
                LoadData(_inMemoryCache);
                LoadData(_redis);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = SearchCordinates(_fileSystemCache);
            result += SearchCordinates(_inMemoryCache);
            result += SearchCordinates(_redis);
            result += SearchCordinates(_cosmoDb);
            return Ok(result);
        }
    }
}
