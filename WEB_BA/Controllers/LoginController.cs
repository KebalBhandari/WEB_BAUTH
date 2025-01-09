using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using WEB_BA.DataProvider;
using WEB_BA.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System;
using System.Net;

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
                    if (response.StartsWith("<") || response.Contains("html"))
                    {
                        TempData["msgtype"] = "error";
                        TempData["message"] = "Unexpected response from the server. Please contact support.";
                        return View(model);
                    }

                    else if (response != null && response != "Null")
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);
                        if (jsonResponse != null)
                        {
                            if (jsonResponse.status == "SUCCESS")
                            {
                                HttpContext.Session.SetString("UserEmail", model.Email);
                                if (!string.IsNullOrEmpty((string)jsonResponse.userName))
                                    HttpContext.Session.SetString("UserName", (string)jsonResponse.userName);
                                if (!string.IsNullOrEmpty((string)jsonResponse.refreshToken))
                                    HttpContext.Session.SetString("TokenNo", (string)jsonResponse.refreshToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtToken))
                                    HttpContext.Session.SetString("JWToken", (string)jsonResponse.jwtToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtRefreshToken))
                                    HttpContext.Session.SetString("JWTRefreshToken", (string)jsonResponse.jwtRefreshToken);

                                TempData["msgtype"] = "LoginSuccess";
                                TempData["message"] = (string)jsonResponse.message;
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else
                            {
                                TempData["msgtype"] = "info";
                                TempData["message"] = (string)jsonResponse.message;
                                return View(model);
                            }

                        }
                        else
                        {
                            TempData["msgtype"] = "info";
                            TempData["message"] = "Error connecting to the authentication service. Try Again";
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

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var keyCloakAuth = new KeyCloakAuthModel();
            var url = "api/Auth/LoginOAuth";
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authResult?.Succeeded != true)
            {
                TempData["msgtype"] = "error";
                TempData["message"] = "External login failed.";
                return RedirectToAction("Index");
            }
            else
            {
                var accessToken = authResult.Properties.GetTokenValue("access_token");
                var refreshToken = authResult.Properties.GetTokenValue("refresh_token");

                // Save the tokens to the user's session or database
                HttpContext.Session.SetString("access_token", accessToken);
                HttpContext.Session.SetString("refresh_token", refreshToken);

                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(accessToken);
                    string username = jwtToken.Claims
                        .FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                    string userId = jwtToken.Claims
                        .FirstOrDefault(c => c.Type == "sub")?.Value
                        ?? jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    string email = jwtToken.Claims
                        .FirstOrDefault(c => c.Type == "email")?.Value;

                    string fullName = jwtToken.Claims
                        .FirstOrDefault(c => c.Type == "name")?.Value;
                    keyCloakAuth.Username = fullName;
                    keyCloakAuth.Email = email;
                    keyCloakAuth.UserId = userId;
                    keyCloakAuth.TokenNo = accessToken;
                    keyCloakAuth.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    keyCloakAuth.UserAgent = Request.Headers["User-Agent"].ToString();

                    string realmAccessJson = jwtToken.Claims
                        .FirstOrDefault(c => c.Type == "realm_access")?.Value;

                    if (!string.IsNullOrEmpty(realmAccessJson))
                    {
                        using JsonDocument doc = JsonDocument.Parse(realmAccessJson);
                        var rolesArray = doc.RootElement.GetProperty("roles");
                        var realmRoles = rolesArray.EnumerateArray()
                            .Select(r => r.GetString())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList();

                        keyCloakAuth.Roles.AddRange(realmRoles);
                    }

                    string response = await ApiCall.ApiCallWithObject(url, keyCloakAuth, "Post");
                    if (response != null && response != "Null")
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);
                        if (jsonResponse != null)
                        {
                            if (jsonResponse.status == "SUCCESS")
                            {
                                HttpContext.Session.SetString("UserEmail", keyCloakAuth.Email);
                                if (!string.IsNullOrEmpty((string)jsonResponse.userName))
                                    HttpContext.Session.SetString("UserName", (string)jsonResponse.userName);
                                if (!string.IsNullOrEmpty((string)jsonResponse.refreshToken))
                                    HttpContext.Session.SetString("TokenNo", (string)jsonResponse.refreshToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtToken))
                                    HttpContext.Session.SetString("JWToken", (string)jsonResponse.jwtToken);
                                if (!string.IsNullOrEmpty((string)jsonResponse.jwtRefreshToken))
                                    HttpContext.Session.SetString("JWTRefreshToken", (string)jsonResponse.jwtRefreshToken);

                                TempData["msgtype"] = "LoginSuccess";
                                TempData["message"] = (string)jsonResponse.message;
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else
                            {
                                TempData["msgtype"] = "info";
                                TempData["message"] = (string)jsonResponse.message;
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            TempData["msgtype"] = "info";
                            TempData["message"] = "Error connecting to the authentication service.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["msgtype"] = "info";
                        TempData["message"] = "Error connecting to the authentication service.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {
                    TempData["msgtype"] = "info";
                    TempData["message"] = "Error parsing token";
                    return RedirectToAction("Index");
                }
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
                return true;
            }

            return true;
        }
    }
}
