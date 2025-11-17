using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(IClaimService claimService, IFileService fileService, ILogger<ClaimsController> logger)
        {
            _claimService = claimService;
            _fileService = fileService;
            _logger = logger;
        }

        // GET: Claims for lecturers to view their claims
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);

            List<Claim> claims;

            if (userRole == UserType.Lecturer)
            {
                claims = await _claimService.GetClaimsByLecturerAsync(userId);
            }
            else
            {
                claims = await _claimService.GetAllClaimsAsync();
            }

            ViewBag.UserRole = userRole;
            return View(claims);
        }

        // GET: Submit claim form
        [Authorize(Roles = "Lecturer")]
        public IActionResult Submit()
        {
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);
            return View();
        }

        // POST: Submit claim with enhanced automation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Submit(ClaimViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the validation errors below.";
                    return View(model);
                }

                var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

                // Enhanced validation with business rules
                var validationResult = await _claimService.ValidateClaimAsync(model, userId);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                // Auto-calculation feature (already implemented in model but verified here)
                if (model.HoursWorked <= 0 || model.HourlyRate <= 0)
                {
                    ModelState.AddModelError("", "Hours worked and hourly rate must be greater than 0.");
                    return View(model);
                }

                // Create the claim with automated status assignment
                var claim = await _claimService.CreateClaimAsync(model, userId);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Failed to create claim. Please try again.";
                    return View(model);
                }

                // Automated document processing
                if (model.Document != null)
                {
                    if (!_fileService.ValidateFile(model.Document))
                    {
                        TempData["ErrorMessage"] = "Invalid file. Please check file type and size (max 5MB).";
                        return View(model);
                    }

                    var uploadResult = await _claimService.UploadDocumentAsync(claim.Id, model.Document);
                    if (!uploadResult)
                    {
                        TempData["WarningMessage"] = "Claim submitted but file upload failed.";
                    }
                }

                // Automated notification (simulated)
                await _claimService.NotifyCoordinatorAsync(claim.Id);

                _logger.LogInformation($"Claim {claim.Id} submitted successfully by user {userId}");
                TempData["SuccessMessage"] = $"Claim '{model.Title}' submitted successfully! Claim ID: {claim.Id}. Total Amount: R{model.TotalAmount}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                TempData["ErrorMessage"] = $"An error occurred while submitting the claim: {ex.Message}";
                return View(model);
            }
        }

        // GET: Manage claims for coordinators and managers with automated workflows
        [Authorize(Roles = "ProgramCoordinator,AcademicManager")]
        public async Task<IActionResult> Manage()
        {
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);
            var userId = HttpContext.Session.GetInt32("UserId") ?? 2;

            var pendingClaims = await _claimService.GetPendingClaimsAsync();

            // Automated claim prioritization
            var prioritizedClaims = await _claimService.PrioritizeClaimsAsync(pendingClaims, userId);

            ViewBag.UserRole = userRole;
            return View(prioritizedClaims);
        }

        // POST: Approve claim with automated workflow
        [HttpPost]
        [Authorize(Roles = "ProgramCoordinator,AcademicManager")]
        public async Task<IActionResult> Approve(int id, string notes = "")
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 2;
                var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.ProgramCoordinator);

                // Automated approval workflow
                var success = await _claimService.ProcessClaimApprovalAsync(id, userId, userRole, notes);

                if (success)
                {
                    _logger.LogInformation($"Claim {id} approved by user {userId}");
                    return Json(new { success = true, message = "Claim approved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to approve claim" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving claim {id}");
                return Json(new { success = false, message = "An error occurred while approving the claim" });
            }
        }

        // POST: Reject claim with automated workflow
        [HttpPost]
        [Authorize(Roles = "ProgramCoordinator,AcademicManager")]
        public async Task<IActionResult> Reject(int id, string reason = "")
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 2;
                var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.ProgramCoordinator);

                // Automated rejection workflow
                var success = await _claimService.ProcessClaimRejectionAsync(id, userId, reason);

                if (success)
                {
                    _logger.LogInformation($"Claim {id} rejected by user {userId}");
                    return Json(new { success = true, message = "Claim rejected successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to reject claim" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim {id}");
                return Json(new { success = false, message = "An error occurred while rejecting the claim" });
            }
        }

        // GET: Automated reports for HR
        [Authorize(Roles = "AcademicManager")]
        public async Task<IActionResult> Reports()
        {
            var reports = await _claimService.GenerateReportsAsync();
            return View(reports);
        }

        // POST: Generate automated invoice
        [HttpPost]
        [Authorize(Roles = "AcademicManager")]
        public async Task<IActionResult> GenerateInvoice(int claimId)
        {
            try
            {
                var invoice = await _claimService.GenerateInvoiceAsync(claimId);
                if (invoice != null)
                {
                    return File(invoice.FileData, invoice.ContentType, invoice.FileName);
                }
                return Json(new { success = false, message = "Failed to generate invoice" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice for claim {claimId}");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // GET: Claim details with enhanced automation
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            // Automated audit trail
            await _claimService.RecordClaimViewAsync(id, HttpContext.Session.GetInt32("UserId") ?? 0);

            return View(claim);
        }

        // GET: Download document
        [Authorize]
        public async Task<IActionResult> DownloadDocument(int claimId)
        {
            var document = await _claimService.GetDocumentAsync(claimId);
            if (document == null || document.FileData == null || document.FileData.Length == 0)
            {
                return NotFound();
            }

            // Automated download tracking
            await _claimService.RecordDocumentDownloadAsync(claimId, HttpContext.Session.GetInt32("UserId") ?? 0);

            return File(document.FileData, document.ContentType, document.FileName);
        }

        // GET: Bulk actions for HR
        [Authorize(Roles = "AcademicManager")]
        public async Task<IActionResult> BulkApprove()
        {
            var result = await _claimService.ProcessBulkApprovalAsync();
            return Json(new
            {
                success = true,
                message = $"Bulk approval completed. {result.ApprovedCount} claims approved, {result.SkippedCount} skipped."
            });
        }
    }
}