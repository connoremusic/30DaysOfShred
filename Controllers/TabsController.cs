using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace _30DaysOfShred.Controllers
{
    public class TabsController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Search()
        {
            return View();
        }
    }
}
