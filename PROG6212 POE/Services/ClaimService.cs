using PROG6212_POE.Models;
using PROG6212_POE.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PROG6212_POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ClaimService> _logger;
        private readonly IWebHostEnvironment _environment;

        // In-memory storage (replace with database in production)
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "lecturer1", Password = "password", Email = "lecturer1@university.com", FullName = "Dr. John Smith", Role = UserType.Lecturer },
            new User { Id = 2, Username = "coordinator1", Password = "password", Email = "coordinator1@university.com", FullName = "Prof. Sarah Johnson", Role = UserType.ProgramCoordinator },
            new User { Id = 3, Username = "manager1", Password = "password", Email = "manager1@university.com", FullName = "Dr. Michael Brown", Role = UserType.AcademicManager }
        };

        private static List<Claim> _claims = new List<Claim>
        {
            new Claim { Id = 1, Title = "Research Materials", Description = "Books and journals for research", HoursWorked = 10, HourlyRate = 150, Date = DateTime.Now.AddDays(-5), Status = "Approved", LecturerId = 1, ApprovedById = 2, ApprovalDate = DateTime.Now.AddDays(-3), Notes = "Approved for research purposes", CreatedDate = DateTime.Now.AddDays(-5) },
            new Claim { Id = 2, Title = "Conference Travel", Description = "Travel expenses for academic conference", HoursWorked = 8, HourlyRate = 120, Date = DateTime.Now.AddDays(-12), Status = "Pending", LecturerId = 1, Notes = "International conference presentation", CreatedDate = DateTime.Now.AddDays(-12) },
            new Claim { Id = 3, Title = "Software License", Description = "Annual license for statistical software", HoursWorked = 5, HourlyRate = 200, Date = DateTime.Now.AddDays(-18), Status = "Approved", LecturerId = 1, ApprovedById = 3, ApprovalDate = DateTime.Now.AddDays(-10), Notes = "Essential research tool", CreatedDate = DateTime.Now.AddDays(-18) }
        };

        private static List<ClaimWorkflow> _workflows = new List<ClaimWorkflow>();
        private static List<ClaimReport> _reports = new List<ClaimReport>();

        private static int _nextClaimId = 4;
        private static int _nextWorkflowId = 1;
        private static int _nextReportId = 1;

        public ClaimService(IFileService fileService, ILogger<ClaimService> logger, IWebHostEnvironment environment)
        {
            _fileService = fileService;
            _logger = logger;
            _environment = environment;
        }

        // Existing methods remain the same...

        // New automation methods implementation:

        public async Task<ValidationResult> ValidateClaimAsync(ClaimViewModel model, int lecturerId)
        {
            var result = new ValidationResult { IsValid = true };

            // Automated business rule validation
            if (model.HoursWorked > 100)
            {
                result.Warnings.Add("Hours worked exceeds typical limit. Please ensure accuracy.");
            }

            if (model.HourlyRate > 300)
            {
                result.Warnings.Add("Hourly rate is above average. May require additional approval.");
            }

            // Check for duplicate claims (same title and date)
            var duplicate = _claims.Any(c =>
                c.LecturerId == lecturerId &&
                c.Title.Equals(model.Title, StringComparison.OrdinalIgnoreCase) &&
                c.Date.Date == model.Date.Date);

            if (duplicate)
            {
                result.Errors.Add("A claim with similar title and date already exists.");
                result.IsValid = false;
            }

            // Monthly limit check
            var monthlyTotal = _claims
                .Where(c => c.LecturerId == lecturerId &&
                           c.Date.Month == model.Date.Month &&
                           c.Date.Year == model.Date.Year)
                .Sum(c => c.TotalAmount);

            if (monthlyTotal + model.TotalAmount > 10000) // R10,000 monthly limit
            {
                result.Errors.Add("Monthly claim limit exceeded. Please contact coordinator.");
                result.IsValid = false;
            }

            return await Task.FromResult(result);
        }

        public async Task<List<Claim>> PrioritizeClaimsAsync(List<Claim> claims, int userId)
        {
            // Automated prioritization algorithm
            foreach (var claim in claims)
            {
                var priority = 1; // Default priority

                // Higher priority for older claims
                var daysPending = (DateTime.Now - claim.CreatedDate).Days;
                if (daysPending > 7) priority += 2;
                else if (daysPending > 3) priority += 1;

                // Higher priority for larger amounts
                if (claim.TotalAmount > 1000) priority += 1;
                if (claim.TotalAmount > 2000) priority += 1;

                // Higher priority if has documents
                var hasDocument = await GetDocumentAsync(claim.Id) != null;
                if (hasDocument) priority += 1;

                claim.Priority = priority;
            }

            return claims.OrderByDescending(c => c.Priority).ThenBy(c => c.CreatedDate).ToList();
        }

        public async Task<bool> ProcessClaimApprovalAsync(int claimId, int approvedById, UserType approverRole, string notes = "")
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim == null) return false;

            var previousStatus = claim.Status;

            // Automated workflow based on approver role
            if (approverRole == UserType.ProgramCoordinator && claim.TotalAmount <= 1000)
            {
                claim.Status = "Approved";
                claim.WorkflowStage = "Coordinator Approved";
            }
            else if (approverRole == UserType.AcademicManager)
            {
                claim.Status = "Approved";
                claim.WorkflowStage = "Manager Approved";
            }
            else if (approverRole == UserType.ProgramCoordinator && claim.TotalAmount > 1000)
            {
                claim.Status = "Pending Manager Review";
                claim.WorkflowStage = "Requires Manager Approval";
            }

            claim.ApprovedById = approvedById;
            claim.ApprovalDate = DateTime.Now;
            claim.LastUpdated = DateTime.Now;
            claim.Notes = string.IsNullOrEmpty(notes) ? claim.Notes : $"{claim.Notes}\nApproval Notes: {notes}";

            // Record workflow
            _workflows.Add(new ClaimWorkflow
            {
                Id = _nextWorkflowId++,
                ClaimId = claimId,
                Action = "Approval",
                PerformedById = approvedById,
                PerformedAt = DateTime.Now,
                Notes = notes,
                PreviousStatus = previousStatus,
                NewStatus = claim.Status
            });

            _logger.LogInformation($"Claim {claimId} approved by user {approvedById}. Status: {claim.Status}");

            // Auto-generate invoice if fully approved
            if (claim.Status == "Approved")
            {
                await GenerateInvoiceAsync(claimId);
            }

            return true;
        }

        public async Task<bool> ProcessClaimRejectionAsync(int claimId, int rejectedById, string reason = "")
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim == null) return false;

            var previousStatus = claim.Status;
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
                PerformedAt = DateTime.Now,
                Notes = reason,
                PreviousStatus = previousStatus,
                NewStatus = claim.Status
            });

            _logger.LogInformation($"Claim {claimId} rejected by user {rejectedById}. Reason: {reason}");

            return true;
        }

        public async Task NotifyCoordinatorAsync(int claimId)
        {
            // Simulate notification (in real implementation, this would send email/notification)
            var claim = await GetClaimByIdAsync(claimId);
            if (claim != null)
            {
                _logger.LogInformation($"Notification: New claim #{claimId} '{claim.Title}' submitted by {claim.LecturerName} for amount R{claim.TotalAmount}");

                // In a real application, you would:
                // 1. Send email to coordinators
                // 2. Create notification in database
                // 3. Trigger real-time notification
            }
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(int userId, UserType userRole)
        {
            var allClaims = await GetAllClaimsAsync();
            var userClaims = await GetClaimsByLecturerAsync(userId);

            var stats = new DashboardStatistics
            {
                TotalClaims = userRole == UserType.Lecturer ? userClaims.Count : allClaims.Count,
                PendingClaims = allClaims.Count(c => c.Status == "Pending"),
                ApprovedClaims = allClaims.Count(c => c.Status == "Approved"),
                RejectedClaims = allClaims.Count(c => c.Status == "Rejected"),
                MonthlyTotal = allClaims
                    .Where(c => c.Date.Month == DateTime.Now.Month && c.Date.Year == DateTime.Now.Year)
                    .Sum(c => c.TotalAmount),
                RecentClaims = allClaims
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .ToList(),
                HighPriorityClaims = allClaims
                    .Where(c => c.Status == "Pending" && c.Priority >= 3)
                    .OrderByDescending(c => c.Priority)
                    .Take(5)
                    .ToList()
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
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.ViewCount++;
                claim.LastViewed = DateTime.Now;
            }
            await Task.CompletedTask;
        }

        public async Task RecordDocumentDownloadAsync(int claimId, int userId)
        {
            // Record document download (could be stored in database)
            _logger.LogInformation($"Document for claim {claimId} downloaded by user {userId}");
            await Task.CompletedTask;
        }

        public async Task<List<ClaimReport>> GenerateReportsAsync()
        {
            // Generate automated reports
            var reports = new List<ClaimReport>();

            // Monthly summary report
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
                GeneratedById = 3, // Assuming HR user
                ReportData = System.Text.Json.JsonSerializer.Serialize(monthlyData),
                FileName = $"Monthly_Report_{DateTime.Now:yyyyMMdd}.json"
            };

            reports.Add(monthlyReport);
            _reports.Add(monthlyReport);

            return await Task.FromResult(reports);
        }

        public async Task<Document> GenerateInvoiceAsync(int claimId)
        {
            var claim = await GetClaimByIdAsync(claimId);
            if (claim == null) return null;

            // Simulate invoice generation (in real implementation, use a PDF library)
            var invoiceContent = $@"
                INVOICE - CLAIM #{claim.Id}
                ==============================
                Lecturer: {claim.LecturerName}
                Title: {claim.Title}
                Date: {claim.Date:yyyy-MM-dd}
                Hours Worked: {claim.HoursWorked}
                Hourly Rate: R{claim.HourlyRate}
                Total Amount: R{claim.TotalAmount}
                Status: {claim.Status}
                Approved By: {claim.ApprovedByName}
                Approval Date: {claim.ApprovalDate:yyyy-MM-dd}
                ==============================
                Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
            ";

            var invoiceBytes = System.Text.Encoding.UTF8.GetBytes(invoiceContent);

            return new Document
            {
                FileName = $"Invoice_Claim_{claimId}_{DateTime.Now:yyyyMMdd}.txt",
                ContentType = "text/plain",
                FileData = invoiceBytes,
                FileSize = invoiceBytes.Length
            };
        }

        public async Task<BulkOperationResult> ProcessBulkApprovalAsync()
        {
            var result = new BulkOperationResult();
            var pendingClaims = await GetPendingClaimsAsync();

            foreach (var claim in pendingClaims.Where(c => c.TotalAmount <= 500)) // Auto-approve small claims
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

        public async Task<bool> AutoApproveClaimsAsync()
        {
            // Automated approval for claims meeting certain criteria
            var autoApproveClaims = _claims
                .Where(c => c.Status == "Pending" &&
                           c.TotalAmount <= 300 &&
                           (DateTime.Now - c.CreatedDate).Days >= 1)
                .ToList();

            foreach (var claim in autoApproveClaims)
            {
                await ProcessClaimApprovalAsync(claim.Id, 3, UserType.AcademicManager, "Auto-approved by system");
            }

            _logger.LogInformation($"Auto-approved {autoApproveClaims.Count} claims");
            return autoApproveClaims.Count > 0;
        }
    }
}