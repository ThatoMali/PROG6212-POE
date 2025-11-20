using PROG6212_POE.Models;
using PROG6212_POE.Models.Entities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace PROG6212_POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ClaimService> _logger;

        // In-memory storage
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "lecturer1", Password = "password", Email = "lecturer1@university.com", FullName = "Dr. John Smith", Role = UserType.Lecturer },
            new User { Id = 2, Username = "coordinator1", Password = "password", Email = "coordinator1@university.com", FullName = "Prof. Sarah Johnson", Role = UserType.ProgramCoordinator },
            new User { Id = 3, Username = "manager1", Password = "password", Email = "manager1@university.com", FullName = "Dr. Michael Brown", Role = UserType.AcademicManager }
        };

        private static List<Claim> _claims = new List<Claim>
        {
            new Claim { Id = 1, Title = "Research Materials", Description = "Books and journals for research", HoursWorked = 10, HourlyRate = 150, Date = DateTime.Now.AddDays(-5), Status = "Approved", LecturerId = 1, ApprovedById = 2, ApprovalDate = DateTime.Now.AddDays(-3), Notes = "Approved for research purposes", CreatedDate = DateTime.Now.AddDays(-5), WorkflowStage = "Coordinator Approved" },
            new Claim { Id = 2, Title = "Conference Travel", Description = "Travel expenses for academic conference", HoursWorked = 8, HourlyRate = 120, Date = DateTime.Now.AddDays(-12), Status = "Pending", LecturerId = 1, Notes = "International conference presentation", CreatedDate = DateTime.Now.AddDays(-12), WorkflowStage = "Submitted" },
            new Claim { Id = 3, Title = "Software License", Description = "Annual license for statistical software", HoursWorked = 5, HourlyRate = 200, Date = DateTime.Now.AddDays(-18), Status = "Approved", LecturerId = 1, ApprovedById = 3, ApprovalDate = DateTime.Now.AddDays(-10), Notes = "Essential research tool", CreatedDate = DateTime.Now.AddDays(-18), WorkflowStage = "Manager Approved" }
        };

        private static List<ClaimWorkflow> _workflows = new List<ClaimWorkflow>();
        private static List<ClaimReport> _reports = new List<ClaimReport>();
        private static List<Invoice> _invoices = new List<Invoice>();

        private static int _nextClaimId = 4;
        private static int _nextWorkflowId = 1;
        private static int _nextReportId = 1;
        private static int _nextInvoiceId = 1;

        public ClaimService(IFileService fileService, ILogger<ClaimService> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Initialize some workflow history for demo
            if (!_workflows.Any())
            {
                _workflows.Add(new ClaimWorkflow
                {
                    Id = _nextWorkflowId++,
                    ClaimId = 1,
                    Action = "Submission",
                    PerformedById = 1,
                    PerformedByName = "Dr. John Smith",
                    PerformedAt = DateTime.Now.AddDays(-5),
                    Notes = "Claim submitted for research materials",
                    PreviousStatus = "None",
                    NewStatus = "Pending",
                    WorkflowStage = "Submitted"
                });
                _workflows.Add(new ClaimWorkflow
                {
                    Id = _nextWorkflowId++,
                    ClaimId = 1,
                    Action = "Approval",
                    PerformedById = 2,
                    PerformedByName = "Prof. Sarah Johnson",
                    PerformedAt = DateTime.Now.AddDays(-3),
                    Notes = "Approved for research purposes",
                    PreviousStatus = "Pending",
                    NewStatus = "Approved",
                    WorkflowStage = "Coordinator Approved"
                });
            }
        }

        // Basic CRUD Methods
        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            var claims = _claims
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => {
                PopulateNavigationProperties(c);
                PopulateWorkflowHistory(c);
            });
            return await Task.FromResult(claims);
        }

        public async Task<List<Claim>> GetClaimsByLecturerAsync(int lecturerId)
        {
            var claims = _claims
                .Where(c => c.LecturerId == lecturerId)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => {
                PopulateNavigationProperties(c);
                PopulateWorkflowHistory(c);
            });
            return await Task.FromResult(claims);
        }

        public async Task<List<Claim>> GetPendingClaimsAsync()
        {
            var claims = _claims
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.CreatedDate)
                .ToList();

            claims.ForEach(c => {
                PopulateNavigationProperties(c);
                PopulateWorkflowHistory(c);
            });
            return await Task.FromResult(claims);
        }

        public async Task<Claim> GetClaimByIdAsync(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                PopulateNavigationProperties(claim);
                PopulateWorkflowHistory(claim);
            }
            return await Task.FromResult(claim);
        }

        public async Task<Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId)
        {
            try
            {
                var claim = new Claim
                {
                    Id = _nextClaimId++,
                    Title = model.Title?.Trim() ?? "Untitled Claim",
                    Description = model.Description?.Trim(),
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Date = model.Date,
                    Notes = model.Notes?.Trim(),
                    Status = "Pending",
                    LecturerId = lecturerId,
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    WorkflowStage = "Submitted"
                };

                _claims.Add(claim);
                PopulateNavigationProperties(claim);

                // Record workflow for submission
                var lecturer = await GetUserByIdAsync(lecturerId);
                _workflows.Add(new ClaimWorkflow
                {
                    Id = _nextWorkflowId++,
                    ClaimId = claim.Id,
                    Action = "Submission",
                    PerformedById = lecturerId,
                    PerformedByName = lecturer?.FullName ?? "Unknown User",
                    PerformedAt = DateTime.Now,
                    Notes = "Claim submitted",
                    PreviousStatus = "None",
                    NewStatus = "Pending",
                    WorkflowStage = "Submitted"
                });

                return await Task.FromResult(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating claim: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim == null) return await Task.FromResult(false);

            var previousStatus = claim.Status;
            claim.Status = status;
            claim.ApprovedById = approvedById;
            claim.ApprovalDate = DateTime.Now;
            claim.LastUpdated = DateTime.Now;
            PopulateNavigationProperties(claim);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null) return await Task.FromResult(false);

            _claims.Remove(claim);
            return await Task.FromResult(true);
        }

        public async Task<bool> UploadDocumentAsync(int claimId, IFormFile file)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.Id == claimId);
                if (claim == null) return await Task.FromResult(false);

                var document = await _fileService.SaveFileAsync(file, claimId);
                return await Task.FromResult(document != null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading document: {ex.Message}");
                return await Task.FromResult(false);
            }
        }

        public async Task<Document> GetDocumentAsync(int claimId)
        {
            var result = await _fileService.GetFileAsync(claimId);
            if (result.fileData == null)
                return null;

            return new Document
            {
                FileName = result.fileName,
                ContentType = result.contentType,
                FileData = result.fileData,
                FileSize = result.fileData.Length,
                ClaimId = claimId
            };
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(user);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        // Automation Methods
        public async Task<ValidationResult> ValidateClaimAsync(ClaimViewModel model, int lecturerId)
        {
            var result = new ValidationResult { IsValid = true };

            // Required field validation
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                result.Errors.Add("Claim title is required.");
                result.IsValid = false;
            }

            if (model.HoursWorked <= 0)
            {
                result.Errors.Add("Hours worked must be greater than 0.");
                result.IsValid = false;
            }

            if (model.HourlyRate <= 0)
            {
                result.Errors.Add("Hourly rate must be greater than 0.");
                result.IsValid = false;
            }

            if (model.Date > DateTime.Now)
            {
                result.Errors.Add("Claim date cannot be in the future.");
                result.IsValid = false;
            }

            // Business rule validation
            if (model.HoursWorked > 100)
            {
                result.Warnings.Add("Hours worked exceeds typical limit. Please ensure accuracy.");
            }

            if (model.HourlyRate > 300)
            {
                result.Warnings.Add("Hourly rate is above average. May require additional approval.");
            }

            if (model.HoursWorked * model.HourlyRate > 5000)
            {
                result.Warnings.Add("Claim amount exceeds R5000. Will require manager approval.");
            }

            // Check for duplicate claims (same title and date within 7 days)
            var duplicate = _claims.Any(c =>
                c.LecturerId == lecturerId &&
                c.Title.Equals(model.Title, StringComparison.OrdinalIgnoreCase) &&
                Math.Abs((c.Date - model.Date).Days) <= 7);

            if (duplicate)
            {
                result.Errors.Add("A similar claim was submitted recently. Please check for duplicates.");
                result.IsValid = false;
            }

            // Monthly limit check (R10,000 per month)
            var monthlyTotal = _claims
                .Where(c => c.LecturerId == lecturerId &&
                           c.Date.Month == model.Date.Month &&
                           c.Date.Year == model.Date.Year)
                .Sum(c => c.TotalAmount);

            if (monthlyTotal + (model.HoursWorked * model.HourlyRate) > 10000)
            {
                result.Errors.Add("Monthly claim limit (R10,000) exceeded. Please contact coordinator.");
                result.IsValid = false;
            }

            // Weekend work validation
            if (model.Date.DayOfWeek == DayOfWeek.Saturday || model.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                result.Warnings.Add("Weekend work detected. Please ensure this is authorized.");
            }

            return await Task.FromResult(result);
        }

        public async Task<List<Claim>> PrioritizeClaimsAsync(List<Claim> claims, int userId)
        {
            foreach (var claim in claims)
            {
                var priority = 1;

                var daysPending = (DateTime.Now - claim.CreatedDate).Days;
                if (daysPending > 7) priority += 2;
                else if (daysPending > 3) priority += 1;

                if (claim.TotalAmount > 1000) priority += 1;
                if (claim.TotalAmount > 2000) priority += 1;

                var hasDocument = await GetDocumentAsync(claim.Id) != null;
                if (hasDocument) priority += 1;

                claim.Priority = priority;
            }

            return claims.OrderByDescending(c => c.Priority).ThenBy(c => c.CreatedDate).ToList();
        }

        public async Task<bool> ProcessClaimApprovalAsync(int claimId, int approvedById, UserType approverRole, string notes = "")
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return false;

            var previousStatus = claim.Status;
            var approver = await GetUserByIdAsync(approvedById);

            // Smart Approval Workflow
            if (approverRole == UserType.ProgramCoordinator)
            {
                if (claim.TotalAmount <= 500) // Auto-approve small claims
                {
                    claim.Status = "Approved";
                    claim.WorkflowStage = "Auto-Approved by Coordinator";
                    claim.ApprovedById = approvedById;
                    claim.ApprovalDate = DateTime.Now;
                    notes = string.IsNullOrEmpty(notes) ? "Auto-approved (under R500)" : notes;
                }
                else if (claim.TotalAmount <= 1000) // Coordinator can approve up to R1000
                {
                    claim.Status = "Approved";
                    claim.WorkflowStage = "Coordinator Approved";
                    claim.ApprovedById = approvedById;
                    claim.ApprovalDate = DateTime.Now;
                }
                else // Over R1000 requires manager approval
                {
                    claim.Status = "Pending Manager Review";
                    claim.WorkflowStage = "Requires Manager Approval";
                    claim.ApprovedById = null; // Not approved yet
                    claim.ApprovalDate = null;
                }
            }
            else if (approverRole == UserType.AcademicManager)
            {
                // Manager can approve any amount
                claim.Status = "Approved";
                claim.WorkflowStage = "Manager Approved";
                claim.ApprovedById = approvedById;
                claim.ApprovalDate = DateTime.Now;
            }

            claim.LastUpdated = DateTime.Now;
            claim.Notes = string.IsNullOrEmpty(notes) ? claim.Notes : $"{claim.Notes}\nApproval Notes: {notes}";

            // Record workflow
            _workflows.Add(new ClaimWorkflow
            {
                Id = _nextWorkflowId++,
                ClaimId = claimId,
                Action = "Approval",
                PerformedById = approvedById,
                PerformedByName = approver?.FullName ?? "Unknown Approver",
                PerformedAt = DateTime.Now,
                Notes = notes,
                PreviousStatus = previousStatus,
                NewStatus = claim.Status,
                WorkflowStage = claim.WorkflowStage
            });

            _logger.LogInformation($"Claim {claimId} processed by {approverRole}. Status: {claim.Status}, Stage: {claim.WorkflowStage}");

            // Auto-generate invoice when claim is approved
            if (claim.Status == "Approved")
            {
                await GenerateInvoiceAsync(claimId);
            }

            return true;
        }

        public async Task<bool> ProcessClaimRejectionAsync(int claimId, int rejectedById, string reason = "")
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return false;

            var previousStatus = claim.Status;
            var rejecter = await GetUserByIdAsync(rejectedById);

            claim.Status = "Rejected";
            claim.WorkflowStage = "Rejected";
            claim.LastUpdated = DateTime.Now;
            claim.Notes = string.IsNullOrEmpty(reason) ? claim.Notes : $"{claim.Notes}\nRejection Reason: {reason}";

            // Record workflow
            _workflows.Add(new ClaimWorkflow
            {
                Id = _nextWorkflowId++,
                ClaimId = claimId,
                Action = "Rejection",
                PerformedById = rejectedById,
                PerformedByName = rejecter?.FullName ?? "Unknown Rejecter",
                PerformedAt = DateTime.Now,
                Notes = reason,
                PreviousStatus = previousStatus,
                NewStatus = claim.Status,
                WorkflowStage = claim.WorkflowStage
            });

            _logger.LogInformation($"Claim {claimId} rejected by user {rejectedById}. Reason: {reason}");
            return true;
        }

        public async Task NotifyCoordinatorAsync(int claimId)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim != null)
            {
                _logger.LogInformation($"Notification: New claim #{claimId} '{claim.Title}' submitted by {claim.LecturerName} for amount R{claim.TotalAmount}");
            }
            await Task.CompletedTask;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(int userId, UserType userRole)
        {
            var allClaims = await GetAllClaimsAsync();
            var userClaims = await GetClaimsByLecturerAsync(userId);

            // Calculate monthly total as sum of TotalAmount for CURRENT MONTH only
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyTotal = allClaims
                .Where(c => c.Date.Month == currentMonth && c.Date.Year == currentYear)
                .Sum(c => c.TotalAmount);

            // Calculate all-time total separately
            var allTimeTotal = allClaims.Sum(c => c.TotalAmount);

            // FIXED: Ensure lists are never null
            var stats = new DashboardStatistics
            {
                TotalClaims = userRole == UserType.Lecturer ? userClaims.Count : allClaims.Count,
                PendingClaims = allClaims.Count(c => c.Status == "Pending"),
                ApprovedClaims = allClaims.Count(c => c.Status == "Approved"),
                RejectedClaims = allClaims.Count(c => c.Status == "Rejected"),
                MonthlyTotal = monthlyTotal,
                AllTimeTotal = allTimeTotal,
                RecentClaims = allClaims
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .ToList() ?? new List<Claim>(), // Ensure not null
                HighPriorityClaims = allClaims
                    .Where(c => c.Status == "Pending" && c.Priority >= 3)
                    .OrderByDescending(c => c.Priority)
                    .Take(5)
                    .ToList() ?? new List<Claim>() // Ensure not null
            };

            // Calculate average processing time for approved claims
            var approvedClaims = allClaims.Where(c => c.Status == "Approved" && c.ApprovalDate.HasValue);
            if (approvedClaims.Any())
            {
                stats.AverageProcessingTime = approvedClaims
                    .Average(c => (c.ApprovalDate.Value - c.CreatedDate).TotalHours);
            }

            return stats;
        }

        public async Task RecordClaimViewAsync(int claimId, int userId)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim != null)
            {
                claim.ViewCount++;
                claim.LastViewed = DateTime.Now;
            }
            await Task.CompletedTask;
        }

        public async Task RecordDocumentDownloadAsync(int claimId, int userId)
        {
            _logger.LogInformation($"Document for claim {claimId} downloaded by user {userId}");
            await Task.CompletedTask;
        }

        public async Task<List<ClaimReport>> GenerateReportsAsync()
        {
            var reports = new List<ClaimReport>();

            var monthlyData = _claims
                .Where(c => c.Date.Month == DateTime.Now.Month && c.Date.Year == DateTime.Now.Year)
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Total = g.Sum(c => c.TotalAmount) })
                .ToList();

            var monthlyReport = new ClaimReport
            {
                Id = _nextReportId++,
                ReportType = "Monthly Summary",
                GeneratedDate = DateTime.Now,
                GeneratedById = 3,
                ReportData = System.Text.Json.JsonSerializer.Serialize(monthlyData),
                FileName = $"Monthly_Report_{DateTime.Now:yyyyMMdd}.json"
            };

            reports.Add(monthlyReport);
            _reports.Add(monthlyReport);

            return await Task.FromResult(reports);
        }

        // Invoice Generation Feature
        public async Task<Invoice> GenerateInvoiceAsync(int claimId)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return null;

            // Create professional invoice content
            var invoiceContent = $@"
                UNIVERSITY OF TECHNOLOGY - PAYMENT INVOICE
                ===========================================
                
                INVOICE NUMBER: INV-{claimId:D4}
                INVOICE DATE: {DateTime.Now:dd MMMM yyyy}
                DUE DATE: {DateTime.Now.AddDays(30):dd MMMM yyyy}
                
                BILL TO:
                Lecturer: {claim.LecturerName}
                Department: Academic Faculty
                Email: {_users.FirstOrDefault(u => u.Id == claim.LecturerId)?.Email ?? "N/A"}
                
                CLAIM DETAILS:
                ==============
                Claim ID: #{claim.Id}
                Title: {claim.Title}
                Description: {claim.Description ?? "No description provided"}
                Date of Service: {claim.Date:dd MMMM yyyy}
                
                PAYMENT BREAKDOWN:
                =================
                Hours Worked: {claim.HoursWorked} hours
                Hourly Rate: R{claim.HourlyRate:N2}/hour
                Subtotal: R{(claim.HoursWorked * claim.HourlyRate):N2}
                
                TOTAL AMOUNT DUE: R{claim.TotalAmount:N2}
                
                APPROVAL INFORMATION:
                ====================
                Status: {claim.Status}
                Approved By: {claim.ApprovedByName ?? "Pending Approval"}
                Approval Date: {claim.ApprovalDate?.ToString("dd MMMM yyyy") ?? "N/A"}
                Workflow Stage: {claim.WorkflowStage}
                
                PAYMENT INSTRUCTIONS:
                ====================
                Please process this payment within 30 days of the invoice date.
                Bank: University Central Bank
                Account: 123456789
                Reference: INV-{claimId:D4}-{claim.LecturerName.Replace(" ", "")}
                
                ===========================================
                Generated automatically by Contract Monthly Claim System
                Generated on: {DateTime.Now:dd MMMM yyyy HH:mm:ss}
            ";

            var invoiceBytes = Encoding.UTF8.GetBytes(invoiceContent);

            var invoice = new Invoice
            {
                Id = _nextInvoiceId++,
                ClaimId = claimId,
                InvoiceNumber = $"INV-{claimId:D4}",
                GeneratedDate = DateTime.Now,
                Amount = claim.TotalAmount,
                Status = "Generated",
                FileName = $"Invoice_INV-{claimId:D4}_{DateTime.Now:yyyyMMdd}.txt",
                ContentType = "text/plain",
                FileData = invoiceBytes
            };

            _invoices.Add(invoice);
            _logger.LogInformation($"Invoice generated for claim {claimId}: {invoice.InvoiceNumber}");

            return invoice;
        }

        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await Task.FromResult(_invoices.OrderByDescending(i => i.GeneratedDate).ToList());
        }

        public async Task<Invoice> GetInvoiceAsync(int invoiceId)
        {
            return await Task.FromResult(_invoices.FirstOrDefault(i => i.Id == invoiceId));
        }

        public async Task<BulkOperationResult> ProcessBulkApprovalAsync()
        {
            var result = new BulkOperationResult();
            var pendingClaims = await GetPendingClaimsAsync();

            foreach (var claim in pendingClaims.Where(c => c.TotalAmount <= 500))
            {
                var success = await ProcessClaimApprovalAsync(claim.Id, 3, UserType.AcademicManager, "Auto-approved via bulk processing");
                if (success)
                {
                    result.ApprovedCount++;
                    result.Messages.Add($"Claim #{claim.Id} auto-approved");
                }
                else
                {
                    result.SkippedCount++;
                    result.Messages.Add($"Claim #{claim.Id} skipped - approval failed");
                }
            }

            return result;
        }

        // Background Auto-Approval Feature
        public async Task<bool> AutoApproveClaimsAsync()
        {
            var autoApproveClaims = _claims
                .Where(c => c.Status == "Pending" &&
                           c.TotalAmount <= 300 &&
                           (DateTime.Now - c.CreatedDate).TotalHours >= 24 && // At least 24 hours old
                           !c.Title.ToLower().Contains("urgent") && // Exclude urgent claims
                           c.HoursWorked <= 40) // Reasonable hours
                .ToList();

            foreach (var claim in autoApproveClaims)
            {
                await ProcessClaimApprovalAsync(claim.Id, 3, UserType.AcademicManager, "Auto-approved by system (met criteria)");
                _logger.LogInformation($"Auto-approved claim {claim.Id} - {claim.Title} for R{claim.TotalAmount}");
            }

            _logger.LogInformation($"Auto-approval completed: {autoApproveClaims.Count} claims approved");
            return autoApproveClaims.Count > 0;
        }

        private Claim PopulateNavigationProperties(Claim claim)
        {
            var lecturer = _users.FirstOrDefault(u => u.Id == claim.LecturerId);
            claim.LecturerName = lecturer?.FullName ?? "Unknown Lecturer";

            if (claim.ApprovedById.HasValue)
            {
                var approver = _users.FirstOrDefault(u => u.Id == claim.ApprovedById.Value);
                claim.ApprovedByName = approver?.FullName ?? "Unknown Approver";
            }

            return claim;
        }

        private void PopulateWorkflowHistory(Claim claim)
        {
            claim.WorkflowHistory = _workflows
                .Where(w => w.ClaimId == claim.Id)
                .OrderBy(w => w.PerformedAt)
                .ToList();
        }
    }
}