using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PerformanceTestLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisTest.Controllers
{
    [Route("filesys")]
    public class FileSystem : BaseController
    {
        static bool loaded;
        INonQueryableDataStore _dataStore;

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
            var result = SearchCordinates(_dataStore);
            return Ok(result);
        }
      
    }
}
