using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimService _claimService;

        public HomeController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);

            var allClaims = await _claimService.GetAllClaimsAsync();
            var userClaims = await _claimService.GetClaimsByLecturerAsync(userId);

            var model = new DashboardViewModel
            {
                UserRole = userRole,
                TotalClaims = allClaims.Count,
                PendingApproval = allClaims.Count(c => c.Status == "Pending"),
                Approved = allClaims.Count(c => c.Status == "Approved"),
                RecentClaims = allClaims.Take(5).ToList()
            };

            return View(model);
        }

        // Demo action to switch roles without login
        public IActionResult SwitchRole(UserType role)
        {
            var userId = role switch
            {
                UserType.Lecturer => 1,
                UserType.ProgramCoordinator => 2,
                UserType.AcademicManager => 3,
                _ => 1
            };

            var userName = role switch
            {
                UserType.Lecturer => "Dr. John Smith (Demo)",
                UserType.ProgramCoordinator => "Prof. Sarah Johnson (Demo)",
                UserType.AcademicManager => "Dr. Michael Brown (Demo)",
                _ => "Demo User"
            };

            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetInt32("UserRole", (int)role);
            HttpContext.Session.SetString("UserName", userName);

            TempData["SuccessMessage"] = $"Switched to {role} role";
            return RedirectToAction("Dashboard");
        }
    }
}