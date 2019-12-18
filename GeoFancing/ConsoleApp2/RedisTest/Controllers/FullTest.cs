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
        static bool loaded;

        public FullTest(FileSystemCache fileSystemCache, InMemoryCache inMemoryCache, RedisCache redis)
        {
            _fileSystemCache = fileSystemCache;
            _inMemoryCache = inMemoryCache;
            _redis = redis;

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
            var result = SerachCodrinates(_fileSystemCache);
            result += SerachCodrinates(_inMemoryCache);
            result += SerachCodrinates(_redis);

            return Ok(result);
        }
    }
}
