using Microsoft.AspNetCore.Mvc;
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
                    if (UserName == "")
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
        public Task<IActionResult> SaveUserData([FromBody] UserDataModel userData)
        {
            try
            {
                string session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return Task.FromResult<IActionResult>(Unauthorized("Session expired. Please log in again."));
                }

                return Task.FromResult<IActionResult>(Ok(new { message = "Data saved successfully." }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                return Task.FromResult<IActionResult>(StatusCode(500, "An error occurred while saving data."));
            }
        }


        [HttpGet]
        public Task<IActionResult> GetUserData()
        {
            try
            {
                string session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return Task.FromResult<IActionResult>(Unauthorized("Session expired. Please log in again."));
                }
                else
                {
                    return Task.FromResult<IActionResult>(NotFound("No data found for the user."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return Task.FromResult<IActionResult>(StatusCode(500, "An error occurred while fetching data."));
            }
        }
    }
}
