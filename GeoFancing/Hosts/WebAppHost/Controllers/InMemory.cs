using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("mem")]
    public class InMemory : BaseController
    {
        static bool loaded;
        INonQueryableDataStore _dataStore;
       


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
            var result = SearchCordinates(_dataStore);

            return Ok(result);
        }
    }
}
