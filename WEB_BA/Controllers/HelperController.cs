using Google.Api.Gax.Grpc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WEB_BA.DataProvider;
using static WEB_BA.Models.FirebaseErrorResponse;

namespace WEB_BA.Controllers
{
    public class HelperController : Controller
    {
        private readonly IConfiguration _configuration;

        public HelperController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
                    // Invalidate the session in the DB if we have a refresh token
                    if (!string.IsNullOrEmpty(TokenNo))
                    {
                        var authProvider = new AuthDataProvider(_configuration);
                        bool invalidated = await authProvider.InvalidateSessionAsync(TokenNo);
                        if(invalidated == true)
                        {
                            await HttpContext.SignOutAsync();
                            HttpContext.Session.Remove("TokenNo");
                            HttpContext.Session.Clear();

                            // Double check that TokenNo is cleared
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
                        else
                        {
                            TempData["msgtype"] = "ERROR";
                            TempData["message"] = "Force Redirecting to Login";
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    else
                    {
                        await HttpContext.SignOutAsync();
                        HttpContext.Session.Remove("TokenNo");
                        HttpContext.Session.Clear();

                        // Double check that TokenNo is cleared
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
