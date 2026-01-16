using Microsoft.AspNetCore.Mvc;

namespace POSsystem.Api.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
