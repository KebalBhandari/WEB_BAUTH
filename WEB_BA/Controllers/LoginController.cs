using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WEB_BA.DataProvider;
using WEB_BA.Models;
using WEB_BA.Services;

namespace WEB_BA.Controllers
{
    public class LoginController : Controller
    {
        //private readonly FirebaseService _firebaseService;

        //public LoginController(FirebaseService firebaseService)
        //{
        //    _firebaseService = firebaseService;
        //}

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
                if (Request.HttpContext.Connection.RemoteIpAddress?.ToString()==null || Request.Headers["User-Agent"].ToString()== null)
                {
                    TempData["msgtype"] = "ERROR";
                    TempData["message"] = "Invalid Request Setting, Contact Admin";
                    return View(model);
                }
                else
                {
                    var authProvider = new AuthDataProvider(_configuration);
                    var (status, message, refreshToken) = await authProvider.ValidateLoginAndUpdateSession(
                        model.Email,
                        model.Password,
                        Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    if (status == "SUCCESS")
                    {
                        HttpContext.Session.SetString("UserEmail", model.Email);
                        if (!string.IsNullOrEmpty(refreshToken))
                            HttpContext.Session.SetString("TokenNo", refreshToken);

                        TempData["msgtype"] = "LoginSuccess";
                        TempData["message"] = message;
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        TempData["msgtype"] = "info";
                        TempData["message"] = message;
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Index(LoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["msgtype"] = "error";
        //        TempData["message"] = "Please correct the errors and try again.";
        //        return View(model);
        //    }
        //    try
        //    {
        //        var idToken = await _firebaseService.LoginWithEmailAndPasswordAsync(model.Email, model.Password);
        //        if (idToken is null)
        //        {
        //            TempData["msgtype"] = "info";
        //            TempData["message"] = "Login Failed, Try Again !!!";
        //            return View();
        //        }
        //        else
        //        {
        //            var userProfile = _firebaseService.DecodeIdToken(idToken);

        //            HttpContext.Session.SetString("TokenNo", idToken);
        //            HttpContext.Session.SetString("UserName", userProfile.DisplayName ?? "");
        //            HttpContext.Session.SetString("UserEmail", userProfile.Email ?? "");

        //            TempData["msgtype"] = "LoginSuccess";
        //            TempData["message"] = "Login Successfull !!!";
        //            return RedirectToAction("Index", "Dashboard");
        //        }
        //    }
        //    catch (FirebaseAuthenticationException ex)
        //    {
        //        TempData["msgtype"] = "info";
        //        TempData["message"] = ex.FirebaseMessage;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["msgtype"] = "info";
        //        TempData["message"] = ex.ToString();
        //        return View();
        //    }
        //}
    }
}



