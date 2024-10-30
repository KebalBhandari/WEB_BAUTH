using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WEB_BA.Models;

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
            ViewBag.msgtype = "";
            ViewBag.message = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var idToken = await _firebaseService.LoginWithEmailAndPasswordAsync(model.Email, model.Password);
                if (idToken is null)
                {
                    ViewBag.msgtype = "Error";
                    ViewBag.message = "The username or password you entered is incorrect.";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetString("TokenNo", idToken);
                    return RedirectToAction("Index", "Dashboard");
                    
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
            }

            return View();
        }
    }
}



