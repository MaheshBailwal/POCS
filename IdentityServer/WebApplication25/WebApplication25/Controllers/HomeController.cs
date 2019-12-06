using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication25.Controllers
{
    [Route("home")]
   // [Authorize(Roles = "admin,guest")]

    //[ClaimRequirement("dsfdfdfdf", "CanReadResource")]
    [CustomAuthorize("admin")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return Ok( "sdffdfsf");
        }
    }
}
