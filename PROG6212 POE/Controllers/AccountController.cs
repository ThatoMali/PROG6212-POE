using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Non-functional prototype - just redirect to dashboard with selected role
            return RedirectToAction("Dashboard", "Home", new { role = model.Role });
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}