using Microsoft.AspNetCore.Mvc;
using WEB_BA.Models;
using WEB_BA.Services;

namespace WEB_BA.Controllers
{
    public class LoginController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public LoginController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
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
                var idToken = await _firebaseService.LoginWithEmailAndPasswordAsync(model.Email, model.Password);
                if (idToken is null)
                {
                    TempData["msgtype"] = "info";
                    TempData["message"] = "Login Failed, Try Again !!!";
                    return View();
                }
                else
                {
                    var userProfile = _firebaseService.DecodeIdToken(idToken);

                    HttpContext.Session.SetString("TokenNo", idToken);
                    HttpContext.Session.SetString("UserName", userProfile.DisplayName ?? "");
                    HttpContext.Session.SetString("UserEmail", userProfile.Email ?? "");

                    TempData["msgtype"] = "LoginSuccess";
                    TempData["message"] = "Login Successfull !!!";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (FirebaseAuthenticationException ex)
            {
                TempData["msgtype"] = "info";
                TempData["message"] = ex.FirebaseMessage;
                return View();
            }
            catch (Exception ex)
            {
                TempData["msgtype"] = "info";
                TempData["message"] = ex.ToString();
                return View();
            }
        }
    }
}



