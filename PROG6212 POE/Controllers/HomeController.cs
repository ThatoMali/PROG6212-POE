using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IClaimService claimService, ILogger<HomeController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
                var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);

                var statistics = await _claimService.GetDashboardStatisticsAsync(userId, userRole);

                // Ensure no null values in the view model
                var model = new DashboardViewModel
                {
                    UserRole = userRole,
                    TotalClaims = statistics.TotalClaims,
                    PendingApproval = statistics.PendingClaims,
                    Approved = statistics.ApprovedClaims,
                    MonthlyTotal = statistics.MonthlyTotal,
                    AllTimeTotal = statistics.AllTimeTotal,
                    AverageProcessingTime = statistics.AverageProcessingTime,
                    RecentClaims = statistics.RecentClaims ?? new List<Claim>(),
                    HighPriorityClaims = statistics.HighPriorityClaims ?? new List<Claim>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                // Return a safe default model if there's an error
                return View(new DashboardViewModel
                {
                    UserRole = UserType.Lecturer,
                    RecentClaims = new List<Claim>(),
                    HighPriorityClaims = new List<Claim>()
                });
            }
        }

        // Enhanced role switching with automation
        [Authorize]
        public IActionResult SwitchRole(UserType role)
        {
            var (userId, userName) = role switch
            {
                UserType.Lecturer => (1, "Dr. John Smith (Demo)"),
                UserType.ProgramCoordinator => (2, "Prof. Sarah Johnson (Demo)"),
                UserType.AcademicManager => (3, "Dr. Michael Brown (Demo)"),
                _ => (1, "Demo User")
            };

            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetInt32("UserRole", (int)role);
            HttpContext.Session.SetString("UserName", userName);

            _logger.LogInformation($"User switched to {role} role");

            TempData["SuccessMessage"] = $"Switched to {role} role. Automated features for this role are now active.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}