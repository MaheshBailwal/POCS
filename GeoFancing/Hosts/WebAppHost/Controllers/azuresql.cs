using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("azuresql")]
    public class AzureSQL : BaseController
    {
        IDataStorebyPoint _dataStore;

        public AzureSQL(AzureSql azuredatabaseDS)
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
