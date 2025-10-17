using PROG6212_POE.Models;
using PROG6212_POE.Models.Data.Entities;

namespace PROG6212_POE.Services
{
    public interface IClaimService
    {
        Task<List<Claim>> GetAllClaimsAsync();
        Task<List<Claim>> GetClaimsByLecturerAsync(int lecturerId);
        Task<List<Claim>> GetPendingClaimsAsync();
        Task<Claim> GetClaimByIdAsync(int id);
        Task<Claim> CreateClaimAsync(ClaimViewModel model, int lecturerId);
        Task<bool> UpdateClaimStatusAsync(int claimId, string status, int approvedById);
        Task<bool> DeleteClaimAsync(int id);
        Task<bool> UploadDocumentAsync(int claimId, IFormFile file);
        Task<Document> GetDocumentAsync(int claimId);
    }
}