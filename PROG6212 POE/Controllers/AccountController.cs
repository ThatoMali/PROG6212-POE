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
            _claimService = claimService ?? throw new ArgumentNullException(nameof(claimService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Login()
        {
            // Clear any existing session when showing login page
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if service is available
                if (_claimService == null)
                {
                    ModelState.AddModelError("", "Service unavailable. Please try again.");
                    return View(model);
                }

                var user = await _claimService.GetUserByUsernameAsync(model.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                // Simple password check for demo
                if (user.Password != model.Password)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                // Set session data
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetInt32("UserRole", (int)user.Role);
                HttpContext.Session.SetString("UserName", user.FullName ?? "Unknown User");
                HttpContext.Session.SetString("UserEmail", user.Email ?? "");

                _logger.LogInformation($"User {user.Username} logged in successfully as {user.Role}");

                // Create claims
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username ?? ""),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
                    new System.Security.Claims.Claim("UserId", user.Id.ToString()),
                    new System.Security.Claims.Claim("FullName", user.FullName ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Quick login for testing
        public IActionResult QuickLogin(string role)
        {
            try
            {
                var (userId, userName, userRole) = role?.ToLower() switch
                {
                    "coordinator" => (2, "Prof. Sarah Johnson", UserType.ProgramCoordinator),
                    "manager" => (3, "Dr. Michael Brown", UserType.AcademicManager),
                    _ => (1, "Dr. John Smith", UserType.Lecturer)
                };

                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetInt32("UserRole", (int)userRole);
                HttpContext.Session.SetString("UserName", userName);
                HttpContext.Session.SetString("UserEmail", $"{role}@university.com");

                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userName),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, userRole.ToString()),
                    new System.Security.Claims.Claim("UserId", userId.ToString()),
                    new System.Security.Claims.Claim("FullName", userName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();

                TempData["SuccessMessage"] = $"Quick login as {userRole} successful!";
                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during quick login for role {Role}", role);
                TempData["ErrorMessage"] = "Quick login failed. Please try regular login.";
                return RedirectToAction("Login");
            }
        }
    }
}