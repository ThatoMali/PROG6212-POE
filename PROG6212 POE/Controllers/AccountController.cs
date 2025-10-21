using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IClaimService _claimService;

        public AccountController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _claimService.GetUserByUsernameAsync(model.Username);
            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // Store user info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetInt32("UserRole", (int)user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Dashboard", "Home", new { role = user.Role });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}