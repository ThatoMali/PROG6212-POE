using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index(UserRole role)
        {
            var claims = GetSampleClaims();
            ViewBag.UserRole = role;
            return View(claims);
        }

        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(ClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Non-functional prototype - just redirect to claims list
            return RedirectToAction("Index", new { role = UserRole.Lecturer });
        }

        public IActionResult Details(int id)
        {
            var claim = GetSampleClaims().FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        private List<ClaimViewModel> GetSampleClaims()
        {
            return new List<ClaimViewModel>
            {
                new ClaimViewModel { Id = 1, Title = "Research Materials", Description = "Books and journals for research", Amount = 150.50m, Date = DateTime.Now.AddDays(-5), Status = "Approved", FileName = "receipt1.pdf" },
                new ClaimViewModel { Id = 2, Title = "Conference Travel", Description = "Travel expenses for academic conference", Amount = 420.75m, Date = DateTime.Now.AddDays(-12), Status = "Pending", FileName = "conference_receipts.zip" },
                new ClaimViewModel { Id = 3, Title = "Software License", Description = "Annual license for statistical software", Amount = 299.99m, Date = DateTime.Now.AddDays(-18), Status = "Approved", FileName = "license_invoice.pdf" },
                new ClaimViewModel { Id = 4, Title = "Lab Equipment", Description = "Specialized measuring instruments", Amount = 875.00m, Date = DateTime.Now.AddDays(-25), Status = "Rejected", FileName = "equipment_quote.pdf" }
            };
        }
    }
}