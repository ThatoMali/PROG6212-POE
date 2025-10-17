using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE.Services
{
    public class FileService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
