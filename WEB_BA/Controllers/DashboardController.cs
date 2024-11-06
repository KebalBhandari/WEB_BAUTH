using Microsoft.AspNetCore.Mvc;

namespace WEB_BA.Controllers
{
    public class DashboardController : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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
                    var UserName = HttpContext.Session.GetString("UserName");
                    if (UserName=="")
                    {
                        TempData["UserName"] = "Welcome";
                    }
                    else
                    {
                        TempData["UserName"] = UserName;
                    }
                    
                    TempData["msgtype"] = "";
                    TempData["message"] = "";
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
