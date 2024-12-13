using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using WEB_BA.DataProvider;
using WEB_BA.Models;

namespace WEB_BA.Controllers
{
    public class LoginController : Controller
    {

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            TempData["msgtype"] = null;
            TempData["message"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msgtype"] = "error";
                TempData["message"] = "Please correct the errors and try again.";
                return View(model);
            }

            try
            {
                if (Request.HttpContext.Connection.RemoteIpAddress?.ToString() == null || Request.Headers["User-Agent"].ToString() == null)
                {
                    TempData["msgtype"] = "ERROR";
                    TempData["message"] = "Invalid Request Setting, Contact Admin";
                    return View(model);
                }
                else
                {
                    var url = "api/Auth/Login";
                    var payload = new
                    {
                        Email = model.Email,
                        Password = model.Password,
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserAgent = Request.Headers["User-Agent"].ToString()
                    };

                    string response = await ApiCall.ApiCallWithObject(url, payload, "Post");
                    if (response != null && response != "Null")
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);
                        if (jsonResponse != null)
                        {
                            if (jsonResponse.status == "SUCCESS")
                            {
                                HttpContext.Session.SetString("UserEmail", model.Email);
                                if (!string.IsNullOrEmpty((string)jsonResponse.refreshToken))
                                    HttpContext.Session.SetString("TokenNo", (string)jsonResponse.refreshToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtToken))
                                    HttpContext.Session.SetString("JWToken", (string)jsonResponse.jwtToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtRefreshToken))
                                    HttpContext.Session.SetString("JWTRefreshToken", (string)jsonResponse.jwtRefreshToken);

                                TempData["msgtype"] = "LoginSuccess";
                                TempData["message"] = (string)jsonResponse.Message;
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else
                            {
                                TempData["msgtype"] = "info";
                                TempData["message"] = (string)jsonResponse.Message;
                                return View(model);
                            }

                        }
                        else
                        {
                            TempData["msgtype"] = "info";
                            TempData["message"] = "Error connecting to the authentication service.";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["msgtype"] = "info";
                        TempData["message"] = "Error connecting to the authentication service.";
                        return View(model);
                    }
                }
            }
            catch (SqlException ex)
            {
                TempData["msgtype"] = "info";
                TempData["message"] = $"Database error: {ex.Message}";
                return View(model);
            }
            catch (System.Exception ex)
            {
                TempData["msgtype"] = "info";
                TempData["message"] = ex.ToString();
                return View(model);
            }
        }

        public bool IsTokenExpiringSoon(string token)
        {
            try
            {
                // Decode the JWT payload
                var parts = token.Split('.');
                if (parts.Length < 2) return true;

                var payload = parts[1];
                var jsonBytes = Convert.FromBase64String(payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '='));
                var jsonString = Encoding.UTF8.GetString(jsonBytes);
                var jwtPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                // Extract the expiration time
                if (jwtPayload != null && jwtPayload.ContainsKey("exp"))
                {
                    var exp = Convert.ToDouble(jwtPayload["exp"]);
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)exp).UtcDateTime;
                    return (expirationTime - DateTime.UtcNow).TotalMinutes < 5; // Less than 5 minutes remaining
                }
            }
            catch
            {
                // If decoding fails, assume the token is invalid or expiring
                return true;
            }

            return true;
        }
    }
}



