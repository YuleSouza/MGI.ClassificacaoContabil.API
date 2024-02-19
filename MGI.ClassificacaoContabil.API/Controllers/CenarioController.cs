using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CenarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
