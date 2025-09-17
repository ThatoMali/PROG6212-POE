using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Dashboard(UserRole role)
        {
            var model = new DashboardViewModel
            {
                UserRole = role,
                TotalClaims = 12,
                PendingApproval = 3,
                Approved = 7,
                RecentClaims = GetSampleClaims()
            };

            return View(model);
        }

        private List<ClaimViewModel> GetSampleClaims()
        {
            return new List<ClaimViewModel>
            {
                new ClaimViewModel { Id = 1, Title = "Research Materials", Amount = 150.50m, Date = DateTime.Now.AddDays(-5), Status = "Approved" },
                new ClaimViewModel { Id = 2, Title = "Conference Travel", Amount = 420.75m, Date = DateTime.Now.AddDays(-12), Status = "Pending" },
                new ClaimViewModel { Id = 3, Title = "Software License", Amount = 299.99m, Date = DateTime.Now.AddDays(-18), Status = "Approved" }
            };
        }
    }
}