using Microsoft.AspNetCore.Mvc;

namespace WEB_BA.Controllers
{
    public class SessionExpiredController : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
