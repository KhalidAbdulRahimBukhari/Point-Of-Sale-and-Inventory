using Microsoft.AspNetCore.Mvc;

namespace POSsystem.Api.Controllers
{
    public class ExpensesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
