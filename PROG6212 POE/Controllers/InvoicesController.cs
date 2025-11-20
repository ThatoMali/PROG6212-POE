using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Controllers
{
    [Authorize(Roles = "AcademicManager,ProgramCoordinator")]
    public class InvoicesController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IClaimService claimService, ILogger<InvoicesController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _claimService.GetInvoicesAsync();
            return View(invoices);
        }

        // GET: Generate invoice for a claim
        [HttpPost]
        public async Task<IActionResult> Generate(int claimId)
        {
            try
            {
                var invoice = await _claimService.GenerateInvoiceAsync(claimId);
                if (invoice != null)
                {
                    _logger.LogInformation($"Invoice generated successfully: {invoice.InvoiceNumber}");
                    TempData["SuccessMessage"] = $"Invoice {invoice.InvoiceNumber} generated successfully!";
                    return RedirectToAction("Index");
                }
                return Json(new { success = false, message = "Failed to generate invoice" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice for claim {claimId}");
                return Json(new { success = false, message = "An error occurred while generating invoice" });
            }
        }

        // GET: Download invoice
        public async Task<IActionResult> Download(int id)
        {
            var invoice = await _claimService.GetInvoiceAsync(id);
            if (invoice == null || invoice.FileData == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"Invoice {invoice.InvoiceNumber} downloaded by user");
            return File(invoice.FileData, invoice.ContentType, invoice.FileName);
        }

        // GET: View invoice details
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _claimService.GetInvoiceAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }
    }
}