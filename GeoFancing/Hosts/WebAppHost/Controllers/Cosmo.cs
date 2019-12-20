using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("cosmo")]
    public class Cosmo : BaseController
    {
        INonQueryableDataStore _dataStore;

        static bool loaded;        

        public Cosmo(CosmoDS cosmoDS)
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
            var result = SearchCordinates(_dataStore);
            return Ok(result);            
        }        
    }
}
