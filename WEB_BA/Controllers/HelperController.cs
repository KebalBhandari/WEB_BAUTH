using Google.Api.Gax.Grpc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static WEB_BA.Models.FirebaseErrorResponse;

namespace WEB_BA.Controllers
{
    public class HelperController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> ClearSession()
        {
            try
            {
                string? TokenNo = HttpContext.Session.GetString("TokenNo");
                if (TokenNo == null || TokenNo == "")
                {
                    return RedirectToAction("Index", "SessionExpired");
                }
                else
                {
                    await HttpContext.SignOutAsync();
                    HttpContext.Session.Remove("TokenNo");
                    HttpContext.Session.Clear();
                    TokenNo = HttpContext.Session.GetString("TokenNo");
                    if (TokenNo == null)
                    {
                        return Ok(1);
                    }
                    else
                    {
                        return Ok(0);
                    }
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
