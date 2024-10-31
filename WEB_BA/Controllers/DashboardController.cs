using Microsoft.AspNetCore.Mvc;

namespace WEB_BA.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                string? session = HttpContext.Session.GetString("TokenNo");
                if (session == null || session == "")
                {
                    return RedirectToAction("Index", "SessionExpired");
                }
                else
                {
                    ViewBag.msgtype = "";
                    ViewBag.message = "";
                    return View();
                }
            }
            catch (Exception ex)
            {
                string Exception = ex.ToString();
                TempData["Exception"] = Exception;
                return RedirectToAction("Index", "UnexpectedError");
            }
        }
    }
}
