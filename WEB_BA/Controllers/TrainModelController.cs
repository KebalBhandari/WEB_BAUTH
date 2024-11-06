using Microsoft.AspNetCore.Mvc;
using WEB_BA.Models;
using WEB_BA.Services;

namespace WEB_BA.Controllers
{
    public class TrainModelController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public TrainModelController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            try
            {
                string? session = HttpContext.Session.GetString("TokenNo");
                if (session == null || session == "")
                {
                    return RedirectToAction("Index", "SessionExpired");
                }
                else
                {
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
                string? session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return Unauthorized("Session expired. Please log in again.");
                }

                var userProfile = _firebaseService.DecodeIdToken(session);

                await _firebaseService.SaveUserDataAsync(userProfile.LocalId, userData);

                return Ok(new { message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                return StatusCode(500, "An error occurred while saving data.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                string? session = HttpContext.Session.GetString("TokenNo");
                if (string.IsNullOrEmpty(session))
                {
                    return Unauthorized("Session expired. Please log in again.");
                }

                var userProfile = _firebaseService.DecodeIdToken(session);

                var userData = await _firebaseService.GetUserDataAsync(userProfile.LocalId);

                if (userData != null)
                {
                    return Json(userData);
                }
                else
                {
                    return NotFound("No data found for the user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }
    }
}
