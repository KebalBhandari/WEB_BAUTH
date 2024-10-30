using Microsoft.AspNetCore.Mvc;

namespace WEB_BA.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
