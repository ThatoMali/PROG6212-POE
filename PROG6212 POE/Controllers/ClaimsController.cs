using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;

        public ClaimsController(IClaimService claimService, IFileService fileService)
        {
            _claimService = claimService;
            _fileService = fileService;
        }

        // GET: Claims for lecturers to view their claims
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
        public IActionResult Submit()
        {
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);

            // Allow all roles to submit claims for demo purposes
            // if (userRole != UserType.Lecturer)
            // {
            //     return RedirectToAction("Index");
            // }

            return View();
        }

        // POST: Submit claim
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                // Validate required fields
                if (model.HoursWorked <= 0)
                {
                    ModelState.AddModelError("HoursWorked", "Hours worked must be greater than 0.");
                    return View(model);
                }

                if (model.HourlyRate <= 0)
                {
                    ModelState.AddModelError("HourlyRate", "Hourly rate must be greater than 0.");
                    return View(model);
                }

                // Create the claim
                var claim = await _claimService.CreateClaimAsync(model, userId);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Failed to create claim. Please try again.";
                    return View(model);
                }

                // Handle file upload if provided
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
                        // Continue even if file upload fails, but log it
                        TempData["WarningMessage"] = "Claim submitted but file upload failed.";
                    }
                }

                TempData["SuccessMessage"] = $"Claim '{model.Title}' submitted successfully! Claim ID: {claim.Id}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the actual exception
                TempData["ErrorMessage"] = $"An error occurred while submitting the claim: {ex.Message}";
                return View(model);
            }
        }

        // GET: Manage claims for coordinators and managers
        public async Task<IActionResult> Manage()
        {
            var userRole = (UserType)(HttpContext.Session.GetInt32("UserRole") ?? (int)UserType.Lecturer);

            // Allow all roles to manage claims for demo purposes
            // if (userRole != UserType.ProgramCoordinator && userRole != UserType.AcademicManager)
            // {
            //     return RedirectToAction("Index");
            // }

            var pendingClaims = await _claimService.GetPendingClaimsAsync();
            return View(pendingClaims);
        }

        // POST: Approve claim
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 2; // Default to coordinator for demo
                var success = await _claimService.UpdateClaimStatusAsync(id, "Approved", userId); // Removed .Value
                return Json(new { success = success, message = success ? "Claim approved successfully" : "Failed to approve claim" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: Reject claim
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 2; // Default to coordinator for demo
                var success = await _claimService.UpdateClaimStatusAsync(id, "Rejected", userId); // Removed .Value
                return Json(new { success = success, message = success ? "Claim rejected successfully" : "Failed to reject claim" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // GET: Claim details
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // GET: Download document
        public async Task<IActionResult> DownloadDocument(int claimId)
        {
            var document = await _claimService.GetDocumentAsync(claimId);
            if (document == null || document.FileData == null || document.FileData.Length == 0)
            {
                return NotFound();
            }

            return File(document.FileData, document.ContentType, document.FileName);
        }
    }
}