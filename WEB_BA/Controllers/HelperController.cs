using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WEB_BA.DataProvider;
using WEB_BA.Models;

namespace WEB_BA.Controllers
{
    public class HelperController : Controller
    {
        private readonly LoginController _loginController;

        public HelperController(LoginController loginController)
        {
            _loginController = loginController;
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return Ok(1);
        }

        [HttpPost]
        public async Task<IActionResult> ClearSession()
        {
            try
            {
                string TokenNo = HttpContext.Session.GetString("TokenNo");
                string JWToken = HttpContext.Session.GetString("JWToken");
                string JWTRefreshToken = HttpContext.Session.GetString("JWTRefreshToken");

                // Get the access token from session
                if (string.IsNullOrEmpty(JWToken)) throw new Exception("No access token available");

                // Check if token is expiring
                if (_loginController.IsTokenExpiringSoon(JWToken))
                {
                    var payload = new { Token = JWTRefreshToken, Username = TokenNo };
                    string response = await ApiCall.ApiCallWithObject("/api/Auth/RefreshToken", payload, "POST");
                    dynamic jsonResponse = JsonConvert.DeserializeObject(response);

                    if (jsonResponse != null)
                    {
                        if (jsonResponse.status == "SUCCESS")
                        {
                            if (!string.IsNullOrEmpty((string)jsonResponse.jwtRefreshToken))
                                HttpContext.Session.SetString("JWTRefreshToken", (string)jsonResponse.jwtRefreshToken);
                            if (!string.IsNullOrEmpty((string)jsonResponse.jwtToken))
                                HttpContext.Session.SetString("JWToken", (string)jsonResponse.jwtToken);
                        }
                    }
                }

                if (string.IsNullOrEmpty(TokenNo))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                    return RedirectToAction("Index", "SessionExpired");
                }
                else
                {
                    string JWTRefreshTokenNew = HttpContext.Session.GetString("JWTRefreshToken");
                    string UserEmail = HttpContext.Session.GetString("UserEmail");

                    var url = "api/Auth/InvalidateSession";
                    var payload = new { RefreshToken = TokenNo, Token = JWTRefreshTokenNew, Email = UserEmail };

                    string response = await ApiCall.JWTApiCallWithObject(url, payload, "Post", JWToken);

                    if (!string.IsNullOrEmpty(response) && response != "Null")
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);
                        if (jsonResponse != null)
                        {
                            if (jsonResponse.status == "SUCCESS")
                            {
                                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                                HttpContext.Session.Clear();
                                return Ok(1);
                            }
                            else
                            {
                                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                                HttpContext.Session.Clear();
                                TempData["msgtype"] = "ERROR";
                                TempData["message"] = "Force Redirecting to Login";
                                return Ok(1);
                            }
                        }
                        else
                        {
                            TempData["msgtype"] = "ERROR";
                            HttpContext.Session.Clear();
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                            TempData["message"] = "Error invalidating session.";
                            return Ok(1);
                        }
                    }
                    else
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                        HttpContext.Session.Clear();
                        TempData["msgtype"] = "ERROR";
                        TempData["message"] = "Error invalidating session.";
                        return Ok(1);
                    }
                }
            }
            catch (Exception ex)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                string exception = ex.ToString();
                TempData["Exception"] = exception;
                return RedirectToAction("Index", "UnexpectedError");
            }
        }
    }
}
