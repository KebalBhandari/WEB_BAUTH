using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using WEB_BA.DataProvider;
using WEB_BA.Models;


namespace WEB_BA.Controllers
{
    public class TrainModelController : Controller
    {

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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

        [HttpPost]
        public async Task<IActionResult> SaveUserData([FromBody] UserDataModel userData)
        {
            try
            {
                if (userData == null)
                {
                    return StatusCode(200, new { Status = "ERROR", Message = "Data received Null, Try Again" });
                }
                string session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return RedirectToAction("Index", "Login");
                }

                else
                {
                    string JWToken = HttpContext.Session.GetString("JWToken");
                    var url = "api/TrainModel/SaveData";
                    userData.TokenNo = session;
                    string response = await ApiCall.JWTApiCallWithObject(url, userData, "Post", JWToken);
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

        
        [HttpPost]
        public async Task<IActionResult> PredictData([FromBody] UserDataModel userData)
        {
            try
            {
                string session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return RedirectToAction("Index", "Login");
                }

                else
                {
                    string JWToken = HttpContext.Session.GetString("JWToken");
                    var url = "api/TrainModel/Predict";
                    userData.TokenNo = session;
                    string response = await ApiCall.JWTApiCallWithObject(url, userData, "Post", JWToken);
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
