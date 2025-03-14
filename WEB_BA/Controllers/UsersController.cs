using Microsoft.AspNetCore.Mvc;
using WEB_BA.DataProvider;
using WEB_BA.Models;

namespace WEB_BA.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                string session = HttpContext.Session.GetString("TokenNo");
                if (session == null || session == "")
                {
                    return RedirectToAction("Index", "SessionExpired");
                }
                else
                {
                    var UserName = HttpContext.Session.GetString("UserName");
                    if (UserName == "" || UserName == null)
                    {
                        TempData["UserName"] = "Welcome";
                    }
                    else
                    {
                        TempData["UserName"] = UserName;
                    }
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

        [HttpGet]
        public async Task<IActionResult> GetUsersResult()
        {
            try
            {
                string TokenNo = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(TokenNo))
                {
                    return RedirectToAction("Index", "Login");
                }

                else
                {
                    string JWToken = HttpContext.Session.GetString("JWToken");
                    var url = "api/Users/ListAllUsers";
                    string response = await ApiCall.JWTApiCallWithString(url, TokenNo, "Post", JWToken);
                    return Ok(response);
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
