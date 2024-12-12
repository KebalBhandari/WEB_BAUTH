using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WEB_BA.DataProvider;
using WEB_BA.Models;
using WEB_BA.Services;

namespace WEB_BA.Controllers
{
    public class SignupController : Controller
    {

        private readonly IConfiguration _configuration;

        public SignupController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        { 
            TempData["msgtype"] = "";
            TempData["message"] = "";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignUpModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msgtype"] = "error";
                TempData["message"] = "Please correct the errors and try again.";
                return View(model);
            }
            try
            {
                if (model.Password.ToString() == model.ConfirmPassword.ToString())
                {
                    var authProvider = new AuthDataProvider(_configuration);
                    var (status, message) = await authProvider.SelfCreateUser(
                        model.Username,
                        model.Email,
                        model.Password
                    );

                    if (status == "SUCCESS")
                    {
                        TempData["msgtype"] = "LoginSuccess";
                        TempData["message"] = "User Created Successfully !!!";
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        TempData["msgtype"] = "info";
                        TempData["message"] = message;
                        return View(model);
                    }
                }
                else
                {
                    TempData["msgtype"] = "info";
                    TempData["message"] = "Password Doesnt Match !!!";
                    return View();
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
        //public async Task<IActionResult> Index(SignUpModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["msgtype"] = "error";
        //        TempData["message"] = "Please correct the errors and try again.";
        //        return View(model);
        //    }
        //    try
        //    {
        //        if (model.Password.ToString() == model.ConfirmPassword.ToString())
        //        {
        //            var idToken = await _firebaseService.RegisterWithEmailAndPasswordAsync(model.Email, model.Password, model.Username);
        //            if (idToken is null)
        //            {
        //                TempData["msgtype"] = "info";
        //                TempData["message"] = "SignUp Failed, Try Again !!!";
        //                return View();
        //            }
        //            else
        //            {
        //                HttpContext.Session.SetString("TokenNo", idToken);
        //                TempData["msgtype"] = "LoginSuccess";
        //                TempData["message"] = "User Created Successfully !!!";
        //                return RedirectToAction("Index", "Login");
        //            }
        //        }
        //        else
        //        {
        //            TempData["msgtype"] = "info";
        //            TempData["message"] = "Password Doesnt Match !!!";
        //            return View();
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
