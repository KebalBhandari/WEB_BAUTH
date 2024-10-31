using Microsoft.AspNetCore.Mvc;

namespace WEB_BA.Controllers
{
    public class SessionExpiredController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
