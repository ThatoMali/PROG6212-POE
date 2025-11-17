using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IClaimService claimService, ILogger<AccountController> logger)
        {
            _claimService = claimService;
            _logger = logger;
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

            // Enhanced session management with role-based authentication
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetInt32("UserRole", (int)user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            _logger.LogInformation($"User {user.Username} logged in successfully as {user.Role}");

            // Create claims identity for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out successfully");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}