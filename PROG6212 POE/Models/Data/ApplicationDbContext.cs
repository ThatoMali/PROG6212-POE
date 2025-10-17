using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE.Models.Data
{
    public class ApplicationDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
