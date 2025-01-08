using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WEB_BA.DataProvider;
using WEB_BA.Models;

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
                    var url = "api/Auth/Register";
                    var payload = new
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password
                    };

                    string response = await ApiCall.ApiCallWithObject(url, payload, "Post");
                    if (response.StartsWith("<") || response.Contains("html"))
                    {
                        TempData["msgtype"] = "error";
                        TempData["message"] = "Unexpected response from the server. Please contact support.";
                        return View(model);
                    }
                    else if (!string.IsNullOrEmpty(response) && response != "Null")
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);

                        if (jsonResponse != null && jsonResponse?.status == "SUCCESS")
                        {
                            TempData["msgtype"] = "LoginSuccess";
                            TempData["message"] = "User Created Successfully !!!";
                            return RedirectToAction("Index", "Login");
                        }
                        else if (jsonResponse != null)
                        {
                            TempData["msgtype"] = "info";
                            TempData["message"] = jsonResponse.Message;
                            return View(model);
                        }
                        else
                        {
                            TempData["msgtype"] = "error";
                            TempData["message"] = "Unexpected error occurred while processing the response.";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["msgtype"] = "error";
                        TempData["message"] = "Error connecting to the registration service.";
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

    }
}
