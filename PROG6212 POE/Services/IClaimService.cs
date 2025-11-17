using PROG6212_POE.Models;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Services
{
    public interface IClaimService
    {
        // Existing methods
        Task<List<Claim>> GetAllClaimsAsync();
        Task<List<Claim>> GetClaimsByLecturerAsync(int lecturerId);
        Task<List<Claim>> GetPendingClaimsAsync();
        Task<Claim> GetClaimByIdAsync(int id);
        Task<Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId);
        Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById);
        Task<bool> DeleteClaimAsync(int id);
        Task<bool> UploadDocumentAsync(int claimId, IFormFile file);
        Task<Document> GetDocumentAsync(int claimId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int id);

        // New automation methods
        Task<ValidationResult> ValidateClaimAsync(ClaimViewModel model, int lecturerId);
        Task<List<Claim>> PrioritizeClaimsAsync(List<Claim> claims, int userId);
        Task<bool> ProcessClaimApprovalAsync(int claimId, int approvedById, UserType approverRole, string notes = "");
        Task<bool> ProcessClaimRejectionAsync(int claimId, int rejectedById, string reason = "");
        Task NotifyCoordinatorAsync(int claimId);
        Task<DashboardStatistics> GetDashboardStatisticsAsync(int userId, UserType userRole);
        Task RecordClaimViewAsync(int claimId, int userId);
        Task RecordDocumentDownloadAsync(int claimId, int userId);
        Task<List<ClaimReport>> GenerateReportsAsync();
        Task<Document> GenerateInvoiceAsync(int claimId);
        Task<BulkOperationResult> ProcessBulkApprovalAsync();
        Task<bool> AutoApproveClaimsAsync();
    }
}